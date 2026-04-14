using Microsoft.EntityFrameworkCore;

namespace Hearth.ArcGIS.Infrastructure
{
    /// <summary>
    /// <see cref="DbContextOptionsBuilder"/> 扩展
    /// </summary>
    public static class DbContextOptionsBuilderExtension
    {
        /// <summary>
        /// 自动配置实体用户隔离<see cref="IdentityUserSaveChangesInterceptor" />、软删除<see cref="SoftDeleteSaveChangesInterceptor" />、审计信息<see cref="AuditingSaveChangesInterceptor" />注入切片
        /// </summary>
        /// <param name="builder"><see cref="DbContextOptionsBuilder"/> 实例</param>
        /// <param name="getActiveUserFunc">获取活动用户委托</param>
        /// <returns><see cref="DbContextOptionsBuilder"/> 实例</returns>
        public static DbContextOptionsBuilder AddInterceptors(this DbContextOptionsBuilder builder, Func<string>? getActiveUserFunc = null)
        {
            if (getActiveUserFunc != null)
                builder.AddInterceptors(new IdentityUserSaveChangesInterceptor(getActiveUserFunc));
            return builder.AddInterceptors(new SoftDeleteSaveChangesInterceptor(),
                                           new AuditingSaveChangesInterceptor());
        }
    }
}