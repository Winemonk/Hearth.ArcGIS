namespace Hearth.ArcGIS.Infrastructure
{
    /// <summary>
    /// 仓储基类的接口。
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public interface IRepositoryBase<TEntity> where TEntity : class
    {
        /// <summary>
        /// 删除指定主键的实体。
        /// </summary>
        /// <param name="key">主键值</param>
        /// <returns>是否删除成功</returns>
        Task<bool> DeleteByKey(object key);
        /// <summary>
        /// 删除指定主键的实体。
        /// </summary>
        /// <param name="keys">主键值集合</param>
        /// <returns>是否删除成功</returns>
        Task<bool> DeleteByKeys(params object[] keys);
        /// <summary>
        /// 插入实体。
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>插入成功的实体</returns>
        Task<TEntity?> Insert(TEntity entity);
        /// <summary>
        /// 批量插入实体。
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns>插入成功的实体集合</returns>
        Task<IReadOnlyList<TEntity>> InsertRange(IList<TEntity> entities);
        /// <summary>
        /// 查询实体。
        /// </summary>
        /// <param name="queryAction">查询条件</param>
        /// <returns>查询结果</returns>
        Task<IReadOnlyList<TEntity>> Query(Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryAction = null);
        /// <summary>
        /// 查询实体。
        /// </summary>
        /// <param name="key">主键值</param>
        /// <returns>查询结果</returns>
        Task<TEntity?> QueryByKey(object key);
        /// <summary>
        /// 查询实体。
        /// </summary>
        /// <param name="keys">主键值集合</param>
        /// <returns>查询结果</returns>
        Task<IReadOnlyList<TEntity>> QueryByKeys(params object[] keys);
        /// <summary>
        /// 分页查询实体。
        /// </summary>
        /// <param name="pageNumber">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="queryAction">查询条件</param>
        /// <returns>查询结果</returns>
        Task<PagedResult<TEntity>> QueryPage(int pageNumber, int pageSize, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryAction = null);
        /// <summary>
        /// 更新实体。
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>更新成功的实体</returns>
        Task<TEntity?> Update(TEntity entity);
        /// <summary>
        /// 批量更新实体。
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns>更新成功的实体集合</returns>
        Task<IReadOnlyList<TEntity>> UpdateRange(IList<TEntity> entities);
    }
}