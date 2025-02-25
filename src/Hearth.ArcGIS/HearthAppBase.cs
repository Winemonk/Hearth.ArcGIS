using AutoMapper;
using DryIoc;

namespace Hearth.ArcGIS
{
    /// <summary>
    /// HearthApp基类
    /// </summary>
    public abstract class HearthAppBase
    {
        private static HearthAppBase? _currentApp;
        private Container? _container;

        /// <summary>
        /// HearthAppBase 构造函数
        /// </summary>
        /// <param name="containerBuilder">容器构建器</param>
        public HearthAppBase(IContainerBuilder containerBuilder)
        {
            _container = containerBuilder.Build();
            _currentApp = this;
        }

        /// <summary>
        /// 当前运行的HearthApp
        /// </summary>
        internal static HearthAppBase CurrentApp => _currentApp ?? throw new InvalidOperationException("No HearthApp is running.");

        /// <summary>
        /// IoC容器
        /// </summary>
        public Container Container => _container ?? throw new InvalidOperationException("IoC container is not initialized.");
    }
}