using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Hearth.ArcGIS.Infrastructure
{
    internal class UserContextQueryInterceptor : DbCommandInterceptor
    {
        private readonly IActiveUserContext _userContext;

        public UserContextQueryInterceptor(IActiveUserContext userContext)
        {
            _userContext = userContext;
        }

        public Expression QueryCompilationStarting(Expression queryExpression, QueryExpressionEventData eventData)
        {
            // 使用我们的 Visitor 修改表达式树
            var visitor = new GlobalFilterExpressionVisitor(_userContext);
            return visitor.Visit(queryExpression);
        }
    }

    internal class GlobalFilterExpressionVisitor : ExpressionVisitor
    {
        private readonly IActiveUserContext _userContext;

        public GlobalFilterExpressionVisitor(IActiveUserContext userContext)
        {
            _userContext = userContext;
        }

        protected override Expression VisitExtension(Expression node)
        {
            // 寻找查询根（即数据库表）
            if (node is QueryRootExpression queryRoot)
            {
                return AddFilters(queryRoot);
            }
            return base.VisitExtension(node);
        }

        private Expression AddFilters(QueryRootExpression root)
        {
            var entityType = root.Type;
            var parameter = Expression.Parameter(entityType, "e");
            Expression? filter = null;

            // 1. 处理软删除
            if (typeof(ISoftDeleteEntity).IsAssignableFrom(entityType))
            {
                filter = Expression.Equal(
                    Expression.Property(parameter, nameof(ISoftDeleteEntity.IsDeleted)),
                    Expression.Constant(false)
                );
            }

            // 2. 处理用户隔离
            if (typeof(IIdentifyUserEntity).IsAssignableFrom(entityType))
            {
                var userIdProp = Expression.Property(parameter, nameof(IIdentifyUserEntity.UserId));

                // 重点：这里需要获取当前用户的值
                // 为了避免缓存问题，建议通过闭包引用获取
                var currentActiveUser = _userContext.CurrentActiveUser;

                var isNull = Expression.Equal(userIdProp, Expression.Constant(null, typeof(string)));
                var isEmpty = Expression.Equal(userIdProp, Expression.Constant(string.Empty, typeof(string)));
                var isMatch = Expression.Equal(userIdProp, Expression.Constant(currentActiveUser, typeof(string)));

                var userFilter = Expression.OrElse(Expression.OrElse(isNull, isEmpty), isMatch);

                filter = filter == null ? userFilter : Expression.AndAlso(filter, userFilter);
            }

            if (filter != null)
            {
                // 将过滤条件包装成 .Where(e => ...)
                var lambda = Expression.Lambda(filter, parameter);
                var whereMethod = typeof(Queryable).GetMethods()
                    .First(m => m.Name == "Where" && m.GetParameters().Length == 2)
                    .MakeGenericMethod(entityType);

                return Expression.Call(null, whereMethod, root, lambda);
            }

            return root;
        }
    }

}

