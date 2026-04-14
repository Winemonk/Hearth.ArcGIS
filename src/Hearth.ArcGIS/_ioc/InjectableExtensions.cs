using DryIoc;
using System.Reflection;

namespace Hearth.ArcGIS
{
    /// <summary>
    /// 注入扩展
    /// </summary>
    public static class InjectableExtensions
    {
        /// <summary>
        /// 解析并注入 <paramref name="injectable"/> 实例内的 <see cref="InjectAttribute"/> 标注的所有字段。
        /// </summary>
        /// <remarks>
        /// 若 <see cref="InjectAttribute"/> 标注字段为 <c>readonly</c> ，则必须在构造函数中注入。
        /// 若 <see cref="InjectAttribute"/> 标注字段为可写，则可在构造函数或其他方法中注入。
        /// </remarks>
        /// <param name="injectable">可注入实例</param>
        public static void InjectServices(this IInjectable injectable)
        {
            Container container = HearthAppBase.CurrentApp.Container;
            if (injectable is IScopeInjectable scopeInjectable)
                ScopeInjectServices(container, scopeInjectable);
            else
                InjectServices(container, injectable);
        }

        /// <summary>
        /// 解析并注入 <paramref name="injectable"/> 实例内的所有字段和属性。
        /// </summary>
        /// <remarks>
        /// 需要注入的字段或属性，不可标注为 <c>readonly</c>、<c>init</c>。
        /// </remarks>
        /// <param name="injectable">可注入实例</param>
        /// <param name="propertyAndFieldNames">指定需要注入的属性和字段名称</param>
        public static void InjectPropertiesAndFields(this IInjectable injectable, params string[] propertyAndFieldNames)
        {
            Container container = HearthAppBase.CurrentApp.Container;
            if (injectable is IScopeInjectable scopeInjectable)
            {
                IResolverContext scope;
                if (scopeInjectable is INamedScopeInjectable namedScope)
                    scope = container.OpenScope(namedScope.Name);
                else
                    scope = container.OpenScope();
                scopeInjectable.Scope = scope;
                scope.InjectPropertiesAndFields(injectable, propertyAndFieldNames);
            }
            else
            {
                container.InjectPropertiesAndFields(injectable, propertyAndFieldNames);
            }
        }

        private static void ScopeInjectServices(Container container, IScopeInjectable injectable)
        {
            IResolverContext scope;
            if (injectable is INamedScopeInjectable namedScope)
                scope = container.OpenScope(namedScope.Name);
            else
                scope = container.OpenScope();
            injectable.Scope = scope;
            InjectServices(scope, injectable);
        }

        private static void InjectServices(IResolver resolver, IInjectable injectable)
        {
            List<FieldInfo> fieldInfos = GetAllFields(injectable.GetType());
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                InjectAttribute? autowireAttribute = fieldInfo.GetCustomAttribute<InjectAttribute>();
                if (autowireAttribute == null)
                {
                    continue;
                }
                Type serviceType = autowireAttribute.ServiceType ?? fieldInfo.FieldType;
                object val = resolver.Resolve(serviceType, serviceKey: autowireAttribute.Key);
                fieldInfo.SetValue(injectable, val);
            }
        }

        private static List<FieldInfo> GetAllFields(Type type)
        {
            List<FieldInfo> fieldInfos = new List<FieldInfo>(
                type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance));
            Type? baseType = type.BaseType;
            while (baseType != null)
            {
                fieldInfos.AddRange(baseType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance));
                baseType = baseType.BaseType;
            }
            return fieldInfos;
        }
    }
}