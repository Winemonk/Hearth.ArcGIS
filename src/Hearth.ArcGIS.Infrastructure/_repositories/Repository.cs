using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.ObjectModel;

namespace Hearth.ArcGIS.Infrastructure
{
    /// <summary>
    /// 基础仓储抽象类
    /// </summary>
    /// <typeparam name="TEntity">仓储实体类型</typeparam>
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        protected readonly DbContext _baseDbContext;
        /// <summary>
        /// 实体集合
        /// </summary>
        protected readonly DbSet<TEntity> _baseDbSet;

        /// <summary>
        /// 基础仓储抽象类
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public Repository(DbContext context)
        {
            _baseDbContext = context;
            _baseDbSet = _baseDbContext.Set<TEntity>();
        }
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="key">主键</param>
        /// <returns>结果</returns>
        public virtual async Task<bool> Delete(object key) =>
            await DbContextQueuedTask.Run(async () =>
            {
                TEntity? entity = await Query(key).ConfigureAwait(false) ?? throw new ArgumentException("Entity not found");
                _baseDbSet.Remove(entity);
                int changes = 0;
                try
                {
                    changes = await _baseDbContext.SaveChangesAsync().ConfigureAwait(false);
                }
                catch
                {
                    _baseDbSet.Add(entity);
                    throw;
                }
                return changes > 0;
            }).ConfigureAwait(false);
        /// <summary>
        /// 删除实体集合
        /// </summary>
        /// <param name="keys">主键集合</param>
        /// <returns>结果</returns>
        public virtual async Task<bool> DeleteRange(params object[] keys) =>
            await DbContextQueuedTask.Run(async () =>
            {
                IEnumerable<TEntity> entities = _baseDbSet;
                if (keys != null && keys.Length > 0)
                {
                    entities = await QueryRange(keys).ConfigureAwait(false);
                    if (entities == null || !entities.Any())
                    {
                        throw new ArgumentException("Entities not found");
                    }
                }
                _baseDbSet.RemoveRange(entities);

                int changes = 0;
                try
                {
                    changes = await _baseDbContext.SaveChangesAsync().ConfigureAwait(false);
                }
                catch
                {
                    _baseDbSet.AddRange(entities);
                    throw;
                }
                return changes > 0;
            }).ConfigureAwait(false);
        /// <summary>
        /// 插入实体
        /// </summary>
        /// <param name="entity">待插入实体</param>
        /// <returns>结果实体</returns>
        public virtual async Task<TEntity?> Insert(TEntity entity) =>
            await DbContextQueuedTask.Run(async () =>
            {
                EntityEntry<TEntity> entityEntry = _baseDbSet.Add(entity);
                int changes = 0;
                try
                {
                    changes = await _baseDbContext.SaveChangesAsync().ConfigureAwait(false);
                }
                catch
                {
                    _baseDbSet.Remove(entity);
                    throw;
                }
                if (changes == 0)
                {
                    throw new DbUpdateException("Failed to insert entity", new ReadOnlyCollection<EntityEntry<TEntity>>(new List<EntityEntry<TEntity>> { entityEntry }));
                }
                return entity;
            }).ConfigureAwait(false);
        /// <summary>
        /// 插入实体集合
        /// </summary>
        /// <param name="entities">待插入实体集合</param>
        /// <returns>结果实体集合</returns>
        public virtual async Task<IEnumerable<TEntity>> InsertRange(IEnumerable<TEntity> entities) =>
            await DbContextQueuedTask.Run(async () =>
            {
                _baseDbSet.AddRange(entities);
                int changes = 0;
                try
                {
                    changes = await _baseDbContext.SaveChangesAsync().ConfigureAwait(false);
                }
                catch
                {
                    _baseDbSet.RemoveRange(entities);
                    throw;
                }
                return entities;
            }).ConfigureAwait(false);
        /// <summary>
        /// 分页查询实体
        /// </summary>
        /// <param name="pageNumber">当前页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="queryAction">查询方法</param>
        /// <returns>查询结果</returns>
        public virtual async Task<PagedResult<TEntity>> PagedQuery(int pageNumber, int pageSize, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryAction = null) =>
            await DbContextQueuedTask.Run(async () =>
            {
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 25;
                IQueryable<TEntity> query = _baseDbSet.AsQueryable();
                if (queryAction != null)
                    query = queryAction.Invoke(query);
                int totalCount = query.Count();
                if (totalCount == 0)
                    return PagedResult<TEntity>.Empty;
                TEntity[] items = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToArrayAsync().ConfigureAwait(false);
                return new PagedResult<TEntity>(pageNumber, pageSize, totalCount, items);
            }).ConfigureAwait(false);
        /// <summary>
        /// 查询实体
        /// </summary>
        /// <param name="key">主键</param>
        /// <returns>查询结果</returns>
        public virtual async Task<TEntity?> Query(object key) =>
            await DbContextQueuedTask.Run(async () => await _baseDbSet.FindAsync(key).ConfigureAwait(false)).ConfigureAwait(false);
        /// <summary>
        /// 查询实体集合
        /// </summary>
        /// <param name="queryAction">查询方法</param>
        /// <returns>查询结果</returns>
        public async Task<IEnumerable<TEntity>> QueryRange(Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryAction = null) =>
                    await DbContextQueuedTask.Run(async () =>
            {
                IQueryable<TEntity> query = _baseDbSet.AsQueryable();
                if (queryAction != null)
                    query = queryAction.Invoke(query);
                var sql = query.ToQueryString();
                return await query.ToListAsync().ConfigureAwait(false);
            }).ConfigureAwait(false);
        /// <summary>
        /// 查询实体集合
        /// </summary>
        /// <param name="keys">主键集合</param>
        /// <returns>查询结果</returns>
        public virtual async Task<IEnumerable<TEntity>> QueryRange(params object[] keys) =>
            await DbContextQueuedTask.Run(async () =>
            {
                if (keys == null || keys.Length == 0)
                    return await _baseDbSet.ToListAsync().ConfigureAwait(false);
                return keys.Select(key => _baseDbSet.Find(key)!).Where(e => e != null);
            });
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">待更新实体</param>
        /// <returns>结果实体</returns>
        public virtual async Task<TEntity> Update(TEntity entity) =>
            await DbContextQueuedTask.Run(async () =>
            {
                EntityEntry<TEntity> entityEntry = _baseDbSet.Update(entity);
                int changes = await _baseDbContext.SaveChangesAsync().ConfigureAwait(false);
                if (changes == 0)
                {
                    throw new DbUpdateException("Failed to update entity", new ReadOnlyCollection<EntityEntry<TEntity>>(new List<EntityEntry<TEntity>> { entityEntry }));
                }
                return entity;
            }).ConfigureAwait(false);
        /// <summary>
        /// 更新实体集合
        /// </summary>
        /// <param name="entities">待更新实体集合</param>
        /// <returns>结果实体集合</returns>
        public virtual async Task<IEnumerable<TEntity>> UpdateRange(IEnumerable<TEntity> entities) =>
            await DbContextQueuedTask.Run(async () =>
            {
                _baseDbSet.UpdateRange(entities);
                int changes = await _baseDbContext.SaveChangesAsync().ConfigureAwait(false);
                return entities;
            }).ConfigureAwait(false);
    }
}