using System.Reflection;
using System.Runtime.CompilerServices;

namespace Hearth.ArcGIS.Extensions
{
    internal static class TypeExtensions
    {
        internal static bool IsDirectImplementation(this Type type, Type interfaceType)
        {
            return type.GetInterfaces().Any(t => type.GetInterfaceMap(interfaceType).TargetMethods.All(method => method.DeclaringType == type));
        }
    }
}
