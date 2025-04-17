using DryIoc;

namespace Hearth.ArcGIS
{
    /// <summary>
    /// 主程序
    /// </summary>
    public class HearthApp : HearthAppBase
    {
        private static object _lock = new object();
        private static HearthApp? _app;

        /// <summary>
        /// 获取当前 <see cref="HearthApp"/> 实例
        /// </summary>
        public static HearthApp APP
        {
            get 
            {
                lock (_lock)
                {
                    return _app ??= new HearthApp(new ContainerBuilder());
                }
            }
        }

        /// <summary>
        /// 获取 IoC 容器 <see cref="Container"/> 实例
        /// </summary>
        public static Container CONTAINER => APP.Container;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="containerBuilder">IoC 容器构建器</param>
        protected HearthApp(IContainerBuilder containerBuilder) : base(containerBuilder)
        {
        }
    }
}