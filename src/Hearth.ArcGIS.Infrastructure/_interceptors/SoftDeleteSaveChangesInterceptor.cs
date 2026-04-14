using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Hearth.ArcGIS.Infrastructure
{
    internal class SoftDeleteSaveChangesInterceptor : SaveChangesInterceptor, ISingletonService
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            if (eventData.Context != null)
            {
                ApplySoftDelete(eventData.Context);
            }
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (eventData.Context != null)
            {
                ApplySoftDelete(eventData.Context);
            }
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private static void ApplySoftDelete(DbContext context)
        {
            foreach (var entry in context.ChangeTracker.Entries<ISoftDeleteEntity>())
            {
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = DateTime.UtcNow;

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
}