namespace Hearth.ArcGIS
{
    internal static class TypeExtensions
    {
        /// <summary>
        /// 检查指定的类型是否为指定的服务类型。
        /// </summary>
        internal static bool IsSpecifiedService(this Type type)
            => type == typeof(IService)
            || type == typeof(ITransientService)
            || type == typeof(ISingletonService)
            || type == typeof(IScopedService)
            || type == typeof(IScopedOrSingletonService)
            || type == typeof(IInThreadService);
    }
}