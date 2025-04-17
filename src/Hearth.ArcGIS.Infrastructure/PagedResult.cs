namespace Hearth.ArcGIS.Infrastructure
{
    /// <summary>
    /// 表示数据的分页结果。
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class PagedResult<TEntity>
    {
        /// <summary>
        /// 初始化一个新的分页结果实例。
        /// </summary>
        /// <param name="pageNumber">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="totalCount">全部数据数量</param>
        /// <param name="data">查询结果数据</param>
        public PagedResult(int pageNumber, int pageSize, int totalCount, IEnumerable<TEntity> data)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
            Data = data;
        }
        /// <summary>
        /// 页码
        /// </summary>
        public int PageNumber { get; set; }
        /// <summary>
        /// 页容量
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 全部数据数量
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 查询结果数据
        /// </summary>
        public IEnumerable<TEntity> Data { get; set; }
    }
}
