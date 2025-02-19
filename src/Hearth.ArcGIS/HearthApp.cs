using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System.Reflection;

namespace Hearth.ArcGIS
{
    /// <summary>
    /// 主程序
    /// </summary>
    public class HearthApp
    {
        /// <summary>
        /// 主容器
        /// </summary>
        public Container Container => _CONTAINER;

        /// <summary>
        /// 获取当前 <see cref="HearthApp"/> 实例
        /// </summary>
        /// <returns>当前 <see cref="HearthApp"/> 实例</returns>
        public static HearthApp App => _CONTAINER.Resolve<HearthApp>();

        /// <summary>
        /// 获取当前当前 <see cref="HearthApp"/> 实例容器 <see cref="DryIoc.Container"/>
        /// </summary>
        public static Container AppContainer => App.Container;

        /// <summary>
        /// 配置
        /// </summary>
        /// <typeparam name="TOptions">配置类型</typeparam>
        /// <param name="configuration">IConfiguration实例</param>
        /// <returns>程序实例</returns>
        public HearthApp Configure<TOptions>(IConfiguration configuration) where TOptions : class, new()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddOptions();
            services.Configure<TOptions>(configuration);
            Container.Populate(services, (r, sd) => r.IsRegistered(sd.ServiceType));
            return this;
        }

        /// <summary>
        /// 注册程序集及其引用程序集中所有 <see cref="ServiceAttribute"/> 特性标记的服务类型。
        /// </summary>
        /// <param name="assembly">程序集</param>
        public void RegisterAssemblyAndRefrencedAssembliesTypes(Assembly assembly)
        {
            RegisterAssemblyTypes(assembly);

            AssemblyName[] referencedAssemblyNames = assembly.GetReferencedAssemblies();
            foreach (AssemblyName referencedAssemblyName in referencedAssemblyNames)
            {
                try
                {
                    Assembly referencedAssembly = Assembly.Load(referencedAssemblyName);
                    RegisterAssemblyTypes(referencedAssembly);
                }
                catch { }
            }
        }

        /// <summary>
        /// 注册程序集中所有 <see cref="ServiceAttribute"/> 特性标记的服务类型。
        /// </summary>
        /// <param name="assembly">程序集</param>
        public void RegisterAssemblyTypes(Assembly assembly)
        {
            IEnumerable<Type> classes = assembly.GetLoadableTypes().Where(t => t.IsClass && !t.IsAbstract);
            foreach (Type type in classes)
            {
                RegisterAssemblyType(type);
            }
        }

        private void RegisterAssemblyType(Type type)
        {
            // 根据类型特性注册服务
            ServiceAttribute? registerAttribute = type.GetCustomAttribute<ServiceAttribute>();
            if (registerAttribute != null)
            {
                IReuse? reuse = GetIReuseByEnum(registerAttribute.Reuse);
                Type serviceType = registerAttribute.ServiceType ?? type;
                Container.Register(serviceType, type, serviceKey: registerAttribute.ServiceKey, reuse: reuse, ifAlreadyRegistered: IfAlreadyRegistered.Keep);
                return;
            }
            // 根据接口注册服务
            if (typeof(IService).IsAssignableFrom(type))
            {
                IReuse? reuse = GetReuseByType(type);
                IEnumerable<Type> interfaces = type.GetInterfaces().Where(i => !IsServiceBaseType(i));
                foreach (Type interfaceType in interfaces)
                {
                    // 注册接口和类型
                    Container.Register(interfaceType, type, reuse: reuse, ifAlreadyRegistered: IfAlreadyRegistered.Keep);
                }
                // 注册类型
                Container.Register(type, reuse: reuse, ifAlreadyRegistered: IfAlreadyRegistered.Keep);
                return;
            }
        }

        private bool IsServiceBaseType(Type type)
        {
            return type == typeof(IService) 
                || type == typeof(ITransientService) 
                || type == typeof(ISingletonService) 
                || type == typeof(IScopedService) 
                || type == typeof(IScopedOrSingletonService) 
                || type == typeof(IInThreadService);
        }

        private IReuse? GetReuseByType(Type type)
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

        internal HearthApp()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddNLog());
            Container.RegisterInstance<ILoggerFactory>(loggerFactory);
            Container.Register(typeof(ILogger<>), typeof(Logger<>));
            Container.RegisterDelegate<ILoggerFactory, ILogger>(lf => lf.CreateLogger("."));
            ConfigureViewModelLocationProviderDefaultViewModelFactory();
        }

        private void ConfigureViewModelLocationProviderDefaultViewModelFactory()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, viewModelType) =>
            {
                if (!Container.IsRegistered(viewModelType))
                    Container.Register(viewModelType);
                return Container.Resolve(viewModelType, new object[] { view });
            });
        }

        private static Container _CONTAINER { get; } = new Container(rules => rules.With(FactoryMethod.ConstructorWithResolvableArgumentsIncludingNonPublic));

        static HearthApp()
        {
            _CONTAINER.Register<HearthApp>(Reuse.Singleton);
        }
    }
}