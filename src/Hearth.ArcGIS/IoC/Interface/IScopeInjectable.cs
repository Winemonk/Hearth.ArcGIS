using DryIoc;

namespace Hearth.ArcGIS
{
    /// <summary>
    /// 作用域注入接口
    /// </summary>
    public interface IScopeInjectable : IInjectable
    {
        /// <summary>
        /// 作用域
        /// </summary>
        IResolverContext Scope { get; set; }
    }
}