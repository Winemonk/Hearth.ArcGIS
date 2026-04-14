namespace Hearth.ArcGIS.Infrastructure
{
    /// <summary>
    /// 用户标识实体接口
    /// </summary>
    public interface IIdentifyUserEntity
    {
        /// <summary>
        /// 用户标识，用于用户隔离
        /// </summary>
        string? UserId { get; set; }
    }
}