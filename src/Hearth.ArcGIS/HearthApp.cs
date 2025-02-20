using DryIoc;

namespace Hearth.ArcGIS
{
    /// <summary>
    /// 主程序
    /// </summary>
    public class HearthApp
    {
        private static Container? _container;
        private static HearthApp? _app;

        /// <summary>
        /// 获取当前 <see cref="HearthApp"/> 实例
        /// </summary>
        /// <returns>当前 <see cref="HearthApp"/> 实例</returns>
        public static HearthApp App => _app ??= Container.Resolve<HearthApp>();

        /// <summary>
        /// 获取当前当前 <see cref="HearthApp"/> 实例容器 <see cref="DryIoc.Container"/>
        /// </summary>
        public static Container Container => _container ??= ContainerBuilder.Build();
    }
}