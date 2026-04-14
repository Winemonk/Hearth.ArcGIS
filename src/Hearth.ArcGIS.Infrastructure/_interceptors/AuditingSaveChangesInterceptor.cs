using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Hearth.ArcGIS.Infrastructure
{
    internal class AuditingSaveChangesInterceptor : SaveChangesInterceptor, ISingletonService
    {
        public AuditingSaveChangesInterceptor()
        {
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            if (eventData.Context != null)
            {
                ApplyAuditInfo(eventData.Context);
            }
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context != null)
            {
                ApplyAuditInfo(eventData.Context);
            }
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private static void ApplyAuditInfo(DbContext context)
        {
            var currentTime = DateTime.UtcNow;

            foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = currentTime;
                    entry.Entity.CreatedBy = "";
                    entry.Entity.UpdatedAt = currentTime;
                    entry.Entity.UpdatedBy = "";
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = currentTime;
                    entry.Entity.UpdatedBy = "";
                }
                else
                {
                    continue;
                }

                // 将主键标记为未修改，避免更新主键
                foreach (var property in entry.Properties
                    .Where(p => p.Metadata.IsPrimaryKey()))
                {
                    property.IsModified = false;
                }
            }
        }
    }
}