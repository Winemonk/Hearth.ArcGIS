using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Hearth.ArcGIS.Infrastructure
{
    internal class IdentityUserSaveChangesInterceptor : SaveChangesInterceptor, ISingletonService
    {
        private readonly Func<string> _getCurrentUser;

        public IdentityUserSaveChangesInterceptor(Func<string> getCurrentUser)
        {
            _getCurrentUser = getCurrentUser;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            if (eventData.Context != null)
            {
                ApplyIdentityUser(eventData.Context);
            }
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (eventData.Context != null)
            {
                ApplyIdentityUser(eventData.Context);
            }
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void ApplyIdentityUser(DbContext context)
        {
            if (_getCurrentUser == null)
                return;
            foreach (var entry in context.ChangeTracker.Entries<IIdentifyUserEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.UserId = _getCurrentUser();

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