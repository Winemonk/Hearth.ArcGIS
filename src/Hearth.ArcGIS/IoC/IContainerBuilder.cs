using DryIoc;

namespace Hearth.ArcGIS
{
    /// <summary>
    /// 容器构建器接口
    /// </summary>
    public interface IContainerBuilder
    {
        /// <summary>
        /// 构建容器
        /// </summary>
        /// <returns><see cref="Container"/> 实例</returns>
        public abstract Container Build();
    }
}