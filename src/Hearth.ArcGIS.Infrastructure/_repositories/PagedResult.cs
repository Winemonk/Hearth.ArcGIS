namespace Hearth.ArcGIS.Infrastructure
{
    /// <summary>
    /// 分页结果
    /// </summary>
    public class PagedResult
    {
        /// <summary>
        /// 空分页结果
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns>空分页结果</returns>
        public static PagedResult<TEntity> Empty<TEntity>() => new(0, 0, 0, Array.Empty<TEntity>());
    }

    /// <summary>
    /// 表示数据的分页结果。
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class PagedResult<TEntity>
    {
        public static PagedResult<TEntity> Empty => new PagedResult<TEntity>(0, 0, 0, Array.Empty<TEntity>());

        /// <summary>
        /// 初始化一个新的分页结果实例。
        /// </summary>
        /// <param name="currentPage">页码</param>
        /// <param name="perPageSize">页容量</param>
        /// <param name="totalSize">全部数据数量</param>
        /// <param name="result">查询结果数据</param>
        public PagedResult(int currentPage, int perPageSize, int totalSize, IEnumerable<TEntity> result)
        {
            CurrentPage = currentPage;
            PerPageSize = perPageSize;
            TotalSize = totalSize;
            Results = result;
        }

        /// <summary>
        /// 页码
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// 页容量
        /// </summary>
        public int PerPageSize { get; set; }

        /// <summary>
        /// 全部数据数量
        /// </summary>
        public int TotalSize { get; set; }

        /// <summary>
        /// 查询结果数据
        /// </summary>
        public IEnumerable<TEntity> Results { get; set; }
    }
}