using DryIoc;

namespace Hearth.ArcGIS
{
    /// <summary>
    /// 重用模式枚举
    /// </summary>
    public enum ReuseEnum
    {
        /// <summary>
        /// 默认。
        /// </summary>
        Default,

        /// <summary>
        /// 与 <see cref="Scoped"/> 作用域相同，但需要 <see cref="ThreadScopeContext"/> 。
        /// </summary>
        InThread,

        /// <summary>
        /// 作用域为任何作用域，可以有名称也可以没有名称。
        /// </summary>
        Scoped,

        /// <summary>
        /// 与 <see cref="Scoped"/> 相同，但在没有可用作用域的情况下，将回退到 <see cref="Singleton"/> 重用。
        /// </summary>
        ScopedOrSingleton,

        /// <summary>
        /// 容器中单例。
        /// </summary>
        Singleton,

        /// <summary>
        /// 瞬态，即不会重复使用。
        /// </summary>
        Transient,
    }
}