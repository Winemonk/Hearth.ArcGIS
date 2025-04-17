using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace Hearth.ArcGIS.Infrastructure
{
    /// <summary>
    /// 仓储基类。
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {
        /// <summary>
        /// 数据库上下文。
        /// </summary>
        protected readonly DbContext _baseDbContext;
        /// <summary>
        /// 实体集。
        /// </summary>
        protected readonly DbSet<TEntity> _baseDbSet;

        private readonly ILogger<RepositoryBase<TEntity>> _logger;

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="context">数据库上下文。</param>
        /// <param name="logger">日志记录器。</param>
        public RepositoryBase(DbContext context, ILogger<RepositoryBase<TEntity>> logger)
        {
            _baseDbContext = context;
            _logger = logger;
            _baseDbSet = _baseDbContext.Set<TEntity>();
        }

        /// <summary>
        /// 删除指定主键的实体。
        /// </summary>
        /// <param name="key">主键值</param>
        /// <returns>是否删除成功</returns>
        public virtual async Task<bool> DeleteByKey(object key)
        {
            TEntity? entity = await QueryByKey(key);
            if (entity == null)
            {
                _logger.LogWarning("未找到实体：Type={type}，Key={key}。", typeof(TEntity).FullName, key);
                return false;
            }
            try
            {
                _baseDbSet.Remove(entity);
                int changes = await _baseDbContext.SaveChangesAsync();
                return changes > 0;
            }
            catch (Exception ex)
            {
                await _baseDbSet.AddAsync(entity);
                _logger.LogError(ex, "删除实体失败：Type={type}，Key={key}。", typeof(TEntity).FullName, key);
                return false;
            }
        }
        /// <summary>
        /// 删除指定主键的实体。
        /// </summary>
        /// <param name="keys">主键值集合</param>
        /// <returns>是否删除成功</returns>
        public virtual async Task<bool> DeleteByKeys(params object[] keys)
        {
            IReadOnlyList<TEntity> entities = await QueryByKeys(keys);
            if (entities.Count == 0)
            {
                _logger.LogWarning("未找到实体：Type={type}，Keys={keys}。", typeof(TEntity).FullName, string.Join(",", keys));
                return false;
            }
            try
            {
                _baseDbSet.RemoveRange(entities);
                int changes = await _baseDbContext.SaveChangesAsync();
                return changes > 0;
            }
            catch (Exception ex)
            {
                await _baseDbSet.AddRangeAsync(entities);
                _logger.LogError(ex, "删除实体失败：Type={type}，Keys={keys}。", typeof(TEntity).FullName, string.Join(",", keys));
                return false;
            }
        }
        /// <summary>
        /// 插入实体。
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>插入成功的实体</returns>
        public virtual async Task<TEntity?> Insert(TEntity entity)
        {
            try
            {
                EntityEntry<TEntity> entityEntry = await _baseDbSet.AddAsync(entity);
                int changes = await _baseDbContext.SaveChangesAsync();
                return changes > 0 ? entityEntry.Entity : null;
            }
            catch (Exception ex)
            {
                _baseDbSet.Remove(entity);
                _logger.LogError(ex, "删除实体失败：Type={type}，Entity={@entity}。", typeof(TEntity).FullName, entity);
                return null;
            }
        }
        /// <summary>
        /// 批量插入实体。
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns>插入成功的实体集合</returns>
        public virtual async Task<IReadOnlyList<TEntity>> InsertRange(IList<TEntity> entities)
        {
            List<TEntity> insertedEntities = new List<TEntity>();
            foreach (TEntity entity in entities)
            {
                try
                {
                    TEntity? insertedEntity = await Insert(entity) ?? throw new Exception("插入实体失败。");
                    insertedEntities.Add(insertedEntity);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "插入实体失败：Type={type}，Entity={@entity}。", typeof(TEntity).FullName, entity);
                }
            }
            return insertedEntities;
        }
        /// <summary>
        /// 查询实体。
        /// </summary>
        /// <param name="queryAction">查询条件</param>
        /// <returns>查询结果</returns>
        public virtual async Task<IReadOnlyList<TEntity>> Query(Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryAction = null)
        {
            IQueryable<TEntity> query = _baseDbSet.AsQueryable();
            if (queryAction != null)
                query = queryAction.Invoke(query);
            return await query.ToListAsync();
        }
        /// <summary>
        /// 查询实体。
        /// </summary>
        /// <param name="key">主键值</param>
        /// <returns>查询结果</returns>
        public virtual async Task<TEntity?> QueryByKey(object key)
        {
            TEntity? entity = await _baseDbSet.FindAsync(key);
            return entity;
        }
        /// <summary>
        /// 查询实体。
        /// </summary>
        /// <param name="keys">主键值集合</param>
        /// <returns>查询结果</returns>
        public virtual async Task<IReadOnlyList<TEntity>> QueryByKeys(params object[] keys)
        {
            List<TEntity> entities = new List<TEntity>();
            foreach (object key in keys)
            {
                TEntity? entity = await _baseDbSet.FindAsync(key);
                if (entity != null)
                    entities.Add(entity);
            }
            return entities;
        }
        /// <summary>
        /// 分页查询实体。
        /// </summary>
        /// <param name="pageNumber">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="queryAction">查询条件</param>
        /// <returns>查询结果</returns>
        public virtual async Task<PagedResult<TEntity>> QueryPage(
            int pageNumber, int pageSize, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryAction = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 1;
            IQueryable<TEntity> query = _baseDbSet.AsQueryable();
            if (queryAction != null)
                query = queryAction.Invoke(query);
            int totalCount = await query.CountAsync();
            IEnumerable<TEntity> entities = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return new PagedResult<TEntity>(pageNumber, pageSize, totalCount, entities);
        }
        /// <summary>
        /// 更新实体。
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>更新成功的实体</returns>
        public virtual async Task<TEntity?> Update(TEntity entity)
        {
            _baseDbSet.Update(entity);
            int changes = await _baseDbContext.SaveChangesAsync();
            return changes > 0 ? entity : null;
        }
        /// <summary>
        /// 批量更新实体。
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns>更新成功的实体集合</returns>
        public virtual async Task<IReadOnlyList<TEntity>> UpdateRange(IList<TEntity> entities)
        {
            _baseDbSet.UpdateRange(entities);
            int changes = await _baseDbContext.SaveChangesAsync();
            return changes > 0 ? new List<TEntity>(entities) : new List<TEntity>();
        }
    }
}