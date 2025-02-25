namespace Hearth.ArcGIS
{
    internal static class TypeExtensions
    {
        internal static bool IsSpecifiedService(this Type type)
            => type == typeof(IService)
            || type == typeof(ITransientService)
            || type == typeof(ISingletonService)
            || type == typeof(IScopedService)
            || type == typeof(IScopedOrSingletonService)
            || type == typeof(IInThreadService);
    }
}