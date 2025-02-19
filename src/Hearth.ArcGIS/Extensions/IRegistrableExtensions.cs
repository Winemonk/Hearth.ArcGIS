using DryIoc;
using System.Reflection;

namespace Hearth.ArcGIS
{
    /// <summary>
    /// 注册扩展
    /// </summary>
    public static class IRegistrableExtensions
    {
        /// <summary>
        /// 注册 <see cref="IRegistrable"/> 实例的 <see cref="Assembly"/> 及
        /// <see cref="Assembly.GetReferencedAssemblies"/> 中所有 <see cref="ServiceAttribute"/> 标记的类型服务。
        /// </summary>
        /// <param name="registrable">可注册实例</param>
        public static void RegisterServices(this IRegistrable registrable)
        {
            Container container = HearthApp.AppContainer;
            Assembly assembly = registrable.GetType().Assembly;
            HearthApp.App.RegisterAssemblyAndRefrencedAssembliesTypes(assembly);
        }
    }
}