namespace Hearth.ArcGIS.Infrastructure
{
    /// <summary>
    /// 软删除实体接口
    /// </summary>
    public interface ISoftDeleteEntity
    {
        /// <summary>
        /// 已删除
        /// </summary>
        bool IsDeleted { get; set; }
        /// <summary>
        /// 删除时间
        /// </summary>
        DateTime? DeletedAt { get; set; }
    }
}