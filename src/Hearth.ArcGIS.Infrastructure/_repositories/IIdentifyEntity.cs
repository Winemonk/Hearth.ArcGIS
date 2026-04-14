namespace Hearth.ArcGIS.Infrastructure
{
    /// <summary>
    /// 唯一标识实体接口
    /// </summary>
    /// <typeparam name="T">表示类型</typeparam>
    public interface IIdentifyEntity<T>
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        T Id { get; set; }
    }
}