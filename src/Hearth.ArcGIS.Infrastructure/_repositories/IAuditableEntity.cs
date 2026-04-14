namespace Hearth.ArcGIS.Infrastructure
{
    /// <summary>
    /// 审计信息实体接口
    /// </summary>
    public interface IAuditableEntity
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreatedAt { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        string? CreatedBy { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        DateTime? UpdatedAt { get; set; }
        /// <summary>
        /// 更新人
        /// </summary>
        string? UpdatedBy { get; set; }
    }
}