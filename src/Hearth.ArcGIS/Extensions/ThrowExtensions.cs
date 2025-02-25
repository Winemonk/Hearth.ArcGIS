namespace Hearth.ArcGIS
{
    internal static class ThrowExtensions
    {
        public static T ThrowIfNull<T>(this T obj, Exception? exception = null) where T : class
        {
            if (obj != null)
                return obj;
            throw exception ?? new ArgumentNullException(nameof(obj));
        }
    }
}
