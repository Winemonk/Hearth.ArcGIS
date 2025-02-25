using System.Reflection;

namespace Hearth.ArcGIS
{
    /// <summary>
    /// <see cref="Assembly"/> 扩展方法
    /// </summary>
    internal static class AssemblyExtensions
    {
        /// <summary>
        /// 安全地从程序集中返回可加载类型集。
        /// </summary>
        /// <param name="assembly">要从中加载类型的 <see cref="Assembly"/> 。</param>
        /// <returns>
        /// <paramref name="assembly" />中的类型集合，或者发生错误时可以加载的类型子集。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// 如果 <paramref name="assembly" /> 为 null，则引发 <see cref="ArgumentNullException" />。
        /// </exception>
        internal static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }
            try
            {
                return assembly.DefinedTypes.Select(t => t.AsType());
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(t => t is not null)!;
            }
        }
    }
}