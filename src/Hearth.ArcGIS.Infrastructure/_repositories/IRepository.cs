namespace Hearth.ArcGIS.Infrastructure
{
    /// <summary>
    /// 基础仓储接口
    /// </summary>
    /// <typeparam name="TEntity">仓储实体类型</typeparam>
    public interface IRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="key">主键</param>
        /// <returns>结果</returns>
        Task<bool> Delete(object key);
        /// <summary>
        /// 删除实体集合
        /// </summary>
        /// <param name="keys">主键集合</param>
        /// <returns>结果</returns>
        Task<bool> DeleteRange(params object[] keys);
        /// <summary>
        /// 插入实体
        /// </summary>
        /// <param name="entity">待插入实体</param>
        /// <returns>结果实体</returns>
        Task<TEntity?> Insert(TEntity entity);
        /// <summary>
        /// 插入实体集合
        /// </summary>
        /// <param name="entities">待插入实体集合</param>
        /// <returns>结果实体集合</returns>
        Task<IEnumerable<TEntity>> InsertRange(IEnumerable<TEntity> entities);
        /// <summary>
        /// 分页查询实体
        /// </summary>
        /// <param name="pageNumber">当前页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="queryAction">查询方法</param>
        /// <returns>查询结果</returns>
        Task<PagedResult<TEntity>> PagedQuery(int pageNumber, int pageSize, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryAction = null);

        /// <summary>
        /// 查询实体
        /// </summary>
        /// <param name="key">主键</param>
        /// <returns>查询结果</returns>
        Task<TEntity?> Query(object key);
        /// <summary>
        /// 查询实体集合
        /// </summary>
        /// <param name="queryAction">查询方法</param>
        /// <returns>查询结果</returns>
        Task<IEnumerable<TEntity>> QueryRange(Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryAction = null);
        /// <summary>
        /// 查询实体集合
        /// </summary>
        /// <param name="keys">主键集合</param>
        /// <returns>查询结果</returns>
        Task<IEnumerable<TEntity>> QueryRange(params object[] keys);
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">待更新实体</param>
        /// <returns>结果实体</returns>
        Task<TEntity> Update(TEntity entity);
        /// <summary>
        /// 更新实体集合
        /// </summary>
        /// <param name="entities">待更新实体集合</param>
        /// <returns>结果实体集合</returns>
        Task<IEnumerable<TEntity>> UpdateRange(IEnumerable<TEntity> entities);
    }
}