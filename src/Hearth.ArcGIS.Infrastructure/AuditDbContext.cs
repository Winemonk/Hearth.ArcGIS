using Microsoft.EntityFrameworkCore;

namespace Hearth.ArcGIS.Infrastructure
{
    /// <summary>
    /// 审计DbContext基类
    /// </summary>
    public abstract class AuditDbContext : DbContext
    {
        /// <summary>
        /// 审计DbContext基类
        /// </summary>
        public AuditDbContext(DbContextOptions options) : base(options)
        {
        }

        /// <summary>
        /// 获取当前用户
        /// </summary>
        /// <returns>返回当前用户标识</returns>
        public abstract string GetActiveUser();

        /// <summary>
        /// 获取当前时间
        /// </summary>
        /// <returns>返回当前时间</returns>
        public abstract DateTime GetCurrentTime();

        /// <summary>
        /// 保存更改
        /// </summary>
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ConfigureAuditing();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        /// <summary>
        /// 保存更改
        /// </summary>
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            ConfigureAuditing();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void ConfigureAuditing()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added
                    && entry.Entity is IAuditEntity addEntity)
                {
                    addEntity.CreatedBy = GetActiveUser();
                    addEntity.CreatedAt = GetCurrentTime();
                }
                else if (entry.State == EntityState.Modified
                    && entry.Entity is IAuditEntity updateEntity)
                {
                    updateEntity.UpdatedBy = GetActiveUser();
                    updateEntity.UpdatedAt = GetCurrentTime();
                }
            }
        }
    }
}