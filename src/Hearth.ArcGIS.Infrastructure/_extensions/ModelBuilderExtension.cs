using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Hearth.ArcGIS.Infrastructure
{
    /// <summary>
    /// <see cref="ModelBuilder"/> 扩展
    /// </summary>
    public static class ModelBuilderExtension
    {
        /// <summary>
        /// 自动配置用户隔离、软删除业务逻辑
        /// </summary>
        /// <typeparam name="T">活动用户数据库上下文类型</typeparam>
        /// <param name="modelBuilder"><see cref="ModelBuilder"/> 实例</param>
        /// <param name="context"><see cref="IActiveUserContext"/> 实例</param>
        /// <returns><see cref="ModelBuilder"/> 实例</returns>
        public static ModelBuilder AutoConfigureEntity<T>(this ModelBuilder modelBuilder, T context) where T:DbContext, IActiveUserContext
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var clrType = entityType.ClrType;
                var parameter = Expression.Parameter(clrType, "e");
                List<Expression> andExpressions = new();

                // 1. 软删除
                if (typeof(ISoftDeleteEntity).IsAssignableFrom(clrType))
                {
                    andExpressions.Add(Expression.Equal(
                        Expression.Property(parameter, nameof(ISoftDeleteEntity.IsDeleted)),
                        Expression.Constant(false)));
                }

                // 2. 用户隔离
                if (typeof(IIdentifyUserEntity).IsAssignableFrom(clrType))
                {
                    var userIdProp = Expression.Property(parameter, nameof(IIdentifyUserEntity.UserId));

                    var contextExpr = Expression.Constant(context);
                    Expression activeUserExpr = Expression.Property(contextExpr, nameof(IActiveUserContext.CurrentActiveUser));

                    var isNull = Expression.Equal(userIdProp, Expression.Constant(null, typeof(string)));
                    var isEmpty = Expression.Equal(userIdProp, Expression.Constant(string.Empty, typeof(string)));
                    var isMatch = Expression.Equal(userIdProp, activeUserExpr);

                    var userOrBlock = Expression.OrElse(Expression.OrElse(isNull, isEmpty), isMatch);

                    andExpressions.Add(userOrBlock);
                }

                if (andExpressions.Count > 0)
                {
                    var finalBody = andExpressions.Aggregate(Expression.AndAlso);
                    modelBuilder.Entity(clrType).HasQueryFilter(Expression.Lambda(finalBody, parameter));
                }
            }
            return modelBuilder;
        }
    }
}