namespace Hearth.ArcGIS
{
    /// <summary>
    /// 命名的作用域注入接口
    /// </summary>
    public interface INamedScopeInjectable : IScopeInjectable
    {
        /// <summary>
        /// 作用域名称
        /// </summary>
        object Name { get; }
    }
}