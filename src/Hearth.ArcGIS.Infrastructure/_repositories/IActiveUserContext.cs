namespace Hearth.ArcGIS.Infrastructure
{
    /// <summary>
    /// 活动用户上下文信息
    /// </summary>
    public interface IActiveUserContext
    {
        /// <summary>
        /// 当前活动用户
        /// </summary>
        string? CurrentActiveUser { get; }
    }
}