namespace Hearth.ArcGIS
{
    /// <summary>
    /// 范围或单例服务接口
    /// </summary>
    /// <remarks>
    /// 与 <see cref="IScopedService"/> 相同，但在没有可用范围的情况下，将回退到 <see cref="ISingletonService"/> 重用。
    /// </remarks>
    internal interface IScopedOrSingletonService : IService
    {
    }
}