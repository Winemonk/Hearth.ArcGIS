namespace Hearth.ArcGIS.Infrastructure
{
    /// <summary>
    /// 审计实体接口
    /// </summary>
    public interface IAuditEntity
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreatedAt { get; set; }
        /// <summary>
        /// 创建用户标识
        /// </summary>
        string CreatedBy { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        DateTime? UpdatedAt { get; set; }
        /// <summary>
        /// 更新用户标识
        /// </summary>
        string? UpdatedBy { get; set; }
    }
}