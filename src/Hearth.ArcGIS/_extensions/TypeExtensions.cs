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

        internal static string GetFullName(this Type type)
        {
            ArgumentNullException.ThrowIfNull(type);

            if (!type.IsGenericType)
                return type.FullName ?? $"{type.Namespace}.{type.Name}";

            string namespacePrefix = type.Namespace != null
                ? type.Namespace + "."
                : string.Empty;

            string typeName = type.Name;
            int backtickIndex = typeName.IndexOf('`');
            if (backtickIndex > 0)
                typeName = typeName[..backtickIndex];

            string[] genericArgs = type.GetGenericArguments()
                .Select(t =>
                    t.IsGenericParameter
                        ? t.Name
                        : GetFullName(t))
                .ToArray();

            return $"{namespacePrefix}{typeName}<{string.Join(", ", genericArgs)}>";
        }
    }
}