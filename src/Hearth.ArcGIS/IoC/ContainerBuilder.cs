using AutoMapper;
using DryIoc;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

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
                        null,
                        PropertiesAndFields.All()));
            AddLogger(container);
            AddViewModelLocationProvider(container);
            AddMapperProvidor(container);
            return container;
        }
        
        /// <summary>
        /// 添加 NLog
        /// </summary>
        /// <param name="container"><see cref="Container"/> 实例</param>
        public void AddLogger(Container container)
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
        public void AddViewModelLocationProvider(Container container)
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
        public void AddMapperProvidor(Container container)
        {
            MapperProvider mapperProvider = new MapperProvider();
            container.RegisterInstance(typeof(IMapper), mapperProvider);
            container.RegisterInstance(typeof(IMapperConfigurator), mapperProvider);
        }
    }
}
