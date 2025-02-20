using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Hearth.ArcGIS
{
    /// <summary>
    /// <see cref="Container"/> 扩展方法
    /// </summary>
    public static class ContainerExtensions
    {
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
            return container;
        }

        /// <summary>
        /// 注册程序集及其引用程序集中所有 <see cref="ServiceAttribute"/> 特性标记的服务类型，
        /// 和实现 <see cref="IService"/> 的服务类型。
        /// </summary>
        /// <param name="container"><see cref="Container"/> 实例</param>
        /// <param name="assembly">程序集</param>
        public static void RegisterAssemblyAndRefrencedAssembliesTypes(this Container container, Assembly assembly)
        {
            container.RegisterAssemblyTypes(assembly);

            AssemblyName[] referencedAssemblyNames = assembly.GetReferencedAssemblies();
            foreach (AssemblyName referencedAssemblyName in referencedAssemblyNames)
            {
                try
                {
                    Assembly referencedAssembly = Assembly.Load(referencedAssemblyName);
                    container.RegisterAssemblyTypes(referencedAssembly);
                }
                catch { }
            }
        }

        /// <summary>
        /// 注册程序集中所有 <see cref="ServiceAttribute"/> 特性标记的服务类型，
        /// 和实现 <see cref="IService"/> 的服务类型。
        /// </summary>
        /// <param name="container"><see cref="Container"/> 实例</param>
        /// <param name="assembly">程序集</param>
        public static void RegisterAssemblyTypes(this Container container, Assembly assembly)
        {
            IEnumerable<Type> classes = assembly.GetLoadableTypes().Where(t => t.IsClass && !t.IsAbstract);
            foreach (Type type in classes)
            {
                RegisterAssemblyType(container, type);
            }
        }

        private static void RegisterAssemblyType(Container container, Type implementationType)
        {
            // 根据类型特性注册服务
            ServiceAttribute? registerAttribute = implementationType.GetCustomAttribute<ServiceAttribute>();
            if (registerAttribute != null)
            {
                IReuse? reuse = GetIReuseByEnum(registerAttribute.Reuse);
                if(registerAttribute.ServiceType == null)
                {
                    RegisterAssemblyTypeByInterfaces(container, implementationType, reuse);
                    container.Register(implementationType, reuse: reuse, ifAlreadyRegistered: IfAlreadyRegistered.Keep);
                }
                else
                {
                    container.Register(registerAttribute.ServiceType, implementationType, serviceKey: registerAttribute.ServiceKey, reuse: reuse, ifAlreadyRegistered: IfAlreadyRegistered.Keep);
                }
                return;
            }
            // 根据接口注册服务
            if (typeof(IService).IsAssignableFrom(implementationType))
            {
                IReuse? reuse = GetReuseByType(implementationType);
                RegisterAssemblyTypeByInterfaces(container, implementationType, reuse);
                // 注册类型
                container.Register(implementationType, reuse: reuse, ifAlreadyRegistered: IfAlreadyRegistered.Keep);
                return;
            }
        }

        private static void RegisterAssemblyTypeByInterfaces(Container container, Type implementationType, IReuse? reuse)
        {
            IEnumerable<Type> interfaces = implementationType.GetInterfaces().Where(i => !i.IsSpecifiedService());
            foreach (Type interfaceType in interfaces)
            {
                // 注册接口和类型
                container.Register(interfaceType, implementationType, reuse: reuse, ifAlreadyRegistered: IfAlreadyRegistered.Keep);
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
