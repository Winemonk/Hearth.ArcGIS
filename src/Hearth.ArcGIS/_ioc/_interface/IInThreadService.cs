using DryIoc;

namespace Hearth.ArcGIS
{
    /// <summary>
    /// 线程内服务接口
    /// </summary>
    /// <remarks>
    /// 与 <see cref="IScopedService"/> 作用域相同，但需要 <see cref="ThreadScopeContext"/> 。
    /// </remarks>
    public interface IInThreadService : IService
    {
    }
}