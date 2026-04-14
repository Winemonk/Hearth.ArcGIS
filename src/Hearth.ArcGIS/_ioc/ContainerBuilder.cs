using DryIoc;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;

namespace Hearth.ArcGIS
{
    /// <summary>
    /// 容器构建器
    /// </summary>
    public class ContainerBuilder : IContainerBuilder
    {
        /// <summary>
        /// 构建容器
        /// </summary>
        /// <returns><see cref="Container"/> 实例</returns>
        public virtual Container Build()
        {
            Container container = new Container(
                rules => rules
                    .With(
                        FactoryMethod.ConstructorWithResolvableArgumentsIncludingNonPublic,
                        Parameters.Of.Details((r, p) =>
                        {
                            InjectParamAttribute? importOptions = p.GetCustomAttribute<InjectParamAttribute>();
                            return importOptions == null ? null :
                                 ServiceDetails.Of(serviceKey: importOptions.Key, requiredServiceType: importOptions.ServiceType);
                        }),
                        PropertiesAndFields.All())
                    /*.WithTrackingDisposableTransients()*/);
            AddNlog(container);
            AddViewModelLocationProvider(container);
            AddAutoMapper(container);
            return container;
        }

        /// <summary>
        /// 添加 NLog
        /// </summary>
        /// <param name="container"><see cref="Container"/> 实例</param>
        protected void AddNlog(Container container)
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddNLog());
            container.RegisterInstance<ILoggerFactory>(loggerFactory);
            container.Register(typeof(ILogger<>), typeof(Logger<>));
            container.RegisterDelegate<ILoggerFactory, ILogger>(lf => lf.CreateLogger("."));
        }

        /// <summary>
        /// 添加视图模型定位提供程序
        /// </summary>
        /// <param name="container"><see cref="Container"/> 实例</param>
        protected void AddViewModelLocationProvider(Container container)
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, viewModelType) =>
            {
                if (!container.IsRegistered(viewModelType))
                    container.Register(viewModelType);
                return container.Resolve(viewModelType, new object[] { view });
            });
        }

        /// <summary>
        /// 添加映射器提供程序
        /// </summary>
        /// <param name="container"><see cref="Container"/> 实例</param>
        protected void AddAutoMapper(Container container)
        {
            container.Register(typeof(IMapperConfigurator),
                typeof(MapperProvider),
                Reuse.Singleton);
        }
    }
}