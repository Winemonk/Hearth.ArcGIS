using DryIoc;

namespace Hearth.ArcGIS
{
    /// <summary>
    /// 主程序
    /// </summary>
    public class HearthApp : HearthAppBase
    {
        private static HearthApp? _app;

        /// <summary>
        /// 获取当前 <see cref="HearthApp"/> 实例
        /// </summary>
        public static HearthApp APP => _app ??= new HearthApp(new ContainerBuilder());

        /// <summary>
        /// 获取 IoC 容器
        /// </summary>
        public static Container CONTAINER => APP.Container;

        private HearthApp(IContainerBuilder containerBuilder) : base(containerBuilder)
        {
        }
    }
}