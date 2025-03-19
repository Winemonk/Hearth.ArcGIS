using DryIoc;

namespace Hearth.ArcGIS
{
    /// <summary>
    /// HearthApp基类
    /// </summary>
    public abstract class HearthAppBase
    {
        private static HearthAppBase? _currentApp;
        private readonly Container? _container;

        /// <summary>
        /// HearthAppBase 构造函数
        /// </summary>
        /// <param name="containerBuilder">容器构建器</param>
        protected HearthAppBase(IContainerBuilder containerBuilder)
        {
            _container = containerBuilder.Build();
            _currentApp = this;
        }

        /// <summary>
        /// 当前运行的 <see cref="HearthAppBase"/> 实例
        /// </summary>
        internal static HearthAppBase CurrentApp => _currentApp ?? throw new InvalidOperationException("No HearthApp is running.");

        /// <summary>
        /// IoC 容器 <see cref="DryIoc.Container"/> 实例
        /// </summary>
        public Container Container => _container ?? throw new InvalidOperationException("IoC container is not initialized.");
    }
}