using DryIoc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Serilog;
using System.Configuration;
using System.IO;

namespace Hearth.ArcGIS
{
    /// <summary>
    /// 容器构建器
    /// </summary>
    internal class ContainerBuilder
    {
        internal static Container Build()
        {
            Container container = new Container(rules => rules.With(FactoryMethod.ConstructorWithResolvableArgumentsIncludingNonPublic));
            container.Register<HearthApp>(Reuse.Singleton);
            //AddNLog(container);
            AddSerilog(container);
            UseLocationProvider(container);
            return container;
        }

        /// <summary>
        /// 添加 NLog
        /// </summary>
        /// <param name="container"><see cref="Container"/> 实例</param>
        private static void AddNLog(Container container)
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddNLog());
            container.RegisterInstance<ILoggerFactory>(loggerFactory);
            container.Register(typeof(ILogger<>), typeof(Logger<>));
            container.RegisterDelegate<ILoggerFactory, Microsoft.Extensions.Logging.ILogger>(lf => lf.CreateLogger("."));
        }

        /// <summary>
        /// 使用视图模型定位提供程序
        /// </summary>
        /// <param name="container"><see cref="Container"/> 实例</param>
        private static void UseLocationProvider(Container container)
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, viewModelType) =>
            {
                if (!container.IsRegistered(viewModelType))
                    container.Register(viewModelType);
                return container.Resolve(viewModelType, new object[] { view });
            });
        }
    }
}
