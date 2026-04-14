using AutoMapper;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Documents;

namespace Hearth.ArcGIS
{
    /// <summary>
    /// <see cref="Container"/> 扩展方法
    /// </summary>
    public static class ContainerExtensions
    {
        private static readonly ILogger<HearthAppBase>? _logger = HearthAppBase.CurrentApp.Container.Resolve<ILogger<HearthAppBase>>(ifUnresolved: IfUnresolved.ReturnDefault);
        private static readonly HashSet<Assembly> _registeredAssemblies = new();
        private static readonly HashSet<string> _referenceAssemblyNames = new();

        /// <summary>
        /// 配置映射器
        /// </summary>
        /// <typeparam name="T"><see cref="Profile"/> 映射配置类型</typeparam>
        /// <param name="container"><see cref="Container"/> 实例</param>
        /// <returns><see cref="Container"/> 实例</returns>
        public static Container ConfigureMapper<T>(this Container container) where T : Profile
        {
            return container.ConfigureMapper(typeof(T));
        }

        /// <summary>
        /// 配置映射器
        /// </summary>
        /// <param name="container"><see cref="Container"/> 实例</param>
        /// <param name="profileTypes">映射配置类型</param>
        /// <returns><see cref="Container"/> 实例</returns>
        public static Container ConfigureMapper(this Container container, params Type[] profileTypes)
        {
            IMapperConfigurator mapperConfigurator = container.Resolve<IMapperConfigurator>();
            mapperConfigurator.AddProfiles(profileTypes);
            return container;
        }

        /// <summary>
        /// 配置映射器
        /// </summary>
        /// <param name="container"><see cref="Container"/> 实例</param>
        /// <param name="assemblies">扫描程序集</param>
        /// <returns><see cref="Container"/> 实例</returns>
        public static Container ConfigureMapper(this Container container, params Assembly[] assemblies)
        {
            IMapperConfigurator mapperConfigurator = container.Resolve<IMapperConfigurator>();
            assemblies = assemblies.Where(
                a => a != typeof(HearthApp).Assembly
                    && a.GetReferencedAssemblies().Any(
                        ran => ran.FullName.Equals(typeof(Profile).Assembly.FullName)))
                .ToArray();
            if (assemblies?.Length > 0)
                mapperConfigurator.AddProfiles(assemblies);
            return container;
        }

        /// <summary>
        /// 注册配置
        /// </summary>
        /// <typeparam name="TOptions">配置类型</typeparam>
        /// <param name="container"><see cref="Container"/> 实例</param>
        /// <param name="configuration"><see cref="IConfiguration"/> 实例</param>
        /// <returns><see cref="Container"/> 实例</returns>
        public static Container Configure<TOptions>(this Container container, IConfiguration configuration) where TOptions : class, new()
        {
            ServiceCollection services = new();
            services.AddOptions();
            services.Configure<TOptions>(configuration);
            container.Populate(services, (r, sd) => r.IsRegistered(sd.ServiceType));
            _logger?.LogDebug("注册配置 {options} 成功", typeof(TOptions).GetFullName());
            return container;
        }

        /// <summary>
        /// 自动注册应用程序域的程序集中所有 <see cref="ServiceAttribute"/> 特性标记的服务类型，
        /// 和实现 <see cref="IService"/> 的服务类型。
        /// 注册程序集及其引用程序集中所有 <see cref="Profile"/> 实现类的映射配置。
        /// </summary>
        /// <param name="container"><see cref="Container"/> 实例</param>
        public static Container AutoRegisterAssemblyTypes(this Container container)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();
            container.RegisterAssemblyTypes(callingAssembly);
            AssemblyName[] assemblyNames = callingAssembly.GetReferencedAssemblies();
            Assembly[] referencedAssemblies = assemblyNames.Select(Assembly.Load).ToArray();
            container.RegisterAssemblyTypes(referencedAssemblies);
            //Assembly[] runtimeAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            //container.RegisterAssemblyTypes(runtimeAssemblies);
            AppDomain.CurrentDomain.AssemblyLoad += (s, e) =>
            {
                Debug.WriteLine($"HearthApp -> 程序集加载事件：{e.LoadedAssembly.FullName}，开始扫描程序集...");
                container.RegisterAssemblyTypes(e.LoadedAssembly);
            };
            return container;
        }

        /// <summary>
        /// 注册程序集中所有 <see cref="ServiceAttribute"/> 特性标记的服务类型，
        /// 和实现 <see cref="IService"/> 的服务类型。
        /// 注册程序集中所有 <see cref="Profile"/> 实现类的映射配置。
        /// </summary>
        /// <param name="container"><see cref="Container"/> 实例</param>
        /// <param name="assemblies">程序集</param>
        public static Container RegisterAssemblyTypes(this Container container, params Assembly[] assemblies)
        {
            Assembly[] needRegisterAssemblies = assemblies
                .Where(ass =>
                    ass.GetReferencedAssemblies()
                        .Any(ra =>
                            ra.FullName == typeof(HearthApp).Assembly.FullName))
                .ToArray();
            foreach (Assembly assembly in needRegisterAssemblies)
            {
                Debug.WriteLine($"HearthApp -> 开始注册程序集 {assembly.FullName} 中服务...");
                if (!_registeredAssemblies.Add(assembly))
                    continue;
                try
                {
                    IEnumerable<Type> classes = assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract);
                    Type[] types = classes.ToArray();
                    foreach (Type type in classes)
                    {
                        RegisterAssemblyType(container, type);
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "注册程序集 {assembly} 类型失败", assembly.FullName);
                }
            }
            return container.ConfigureMapper(assemblies);
        }

        private static void RegisterAssemblyType(Container container, Type implementationType)
        {
            // 根据类型特性注册服务
            ServiceAttribute? registerAttribute = implementationType.GetCustomAttribute<ServiceAttribute>();
            if (registerAttribute != null)
            {
                IReuse? reuse = GetIReuseByEnum(registerAttribute.Reuse);
                if (registerAttribute.ServiceType == null)
                {
                    RegisterAssemblyTypeByInterfaces(container, implementationType, reuse);
                    container.Register(implementationType, reuse: reuse);
                    _logger?.LogDebug("注册类型 {type} 成功", implementationType.GetFullName());
                }
                else
                {
                    container.Register(registerAttribute.ServiceType, implementationType, serviceKey: registerAttribute.ServiceKey, reuse: reuse);
                    _logger?.LogDebug("注册类型 {type} -> {serviceType} 成功", implementationType.GetFullName(), registerAttribute.ServiceType.GetFullName());
                }
                return;
            }
            // 根据接口注册服务
            if (typeof(IService).IsAssignableFrom(implementationType))
            {
                IReuse? reuse = GetReuseByType(implementationType);
                RegisterAssemblyTypeByInterfaces(container, implementationType, reuse);
                // 注册类型
                container.Register(implementationType, reuse: reuse);
                _logger?.LogDebug("注册类型 {type} 成功", implementationType.GetFullName());
                return;
            }
        }

        private static void RegisterAssemblyTypeByInterfaces(Container container, Type implementationType, IReuse? reuse)
        {
            IEnumerable<Type> interfaces = implementationType.GetInterfaces().Where(i => !i.IsSpecifiedService());
            foreach (Type interfaceType in interfaces)
            {
                // 注册接口和类型
                container.Register(interfaceType, implementationType, reuse: reuse);
                _logger?.LogDebug("注册类型 {type} -> {serviceType} 成功", implementationType.GetFullName(), interfaceType.GetFullName());
            }
        }

        private static IReuse? GetReuseByType(Type type)
        {
            if (typeof(ITransientService).IsAssignableFrom(type))
                return Reuse.Transient;
            else if (typeof(ISingletonService).IsAssignableFrom(type))
                return Reuse.Singleton;
            else if (typeof(IScopedService).IsAssignableFrom(type))
                return Reuse.Scoped;
            else if (typeof(IScopedOrSingletonService).IsAssignableFrom(type))
                return Reuse.ScopedOrSingleton;
            else if (typeof(IInThreadService).IsAssignableFrom(type))
                return Reuse.InThread;
            return null;
        }

        private static IReuse? GetIReuseByEnum(ReuseEnum reuseEnum)
        {
            return reuseEnum switch
            {
                ReuseEnum.InThread => Reuse.InThread,
                ReuseEnum.Scoped => Reuse.Scoped,
                ReuseEnum.ScopedOrSingleton => Reuse.ScopedOrSingleton,
                ReuseEnum.Singleton => Reuse.Singleton,
                ReuseEnum.Transient => Reuse.Transient,
                _ => null,
            };
        }
    }
}