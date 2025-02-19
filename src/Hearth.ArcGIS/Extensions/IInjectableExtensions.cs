using DryIoc;
using System.Reflection;

namespace Hearth.ArcGIS
{
    /// <summary>
    /// 注入扩展
    /// </summary>
    public static class IInjectableExtensions
    {
        /// <summary>
        /// 注入 <see cref="IInjectable"/> 实例内的 <see cref="InjectAttribute"/> 标注的所有字段。
        /// </summary>
        /// <remarks>
        /// 若 <see cref="InjectAttribute"/> 标注字段为 <c>readonly</c> ，则必须在构造函数中注入。
        /// 若 <see cref="InjectAttribute"/> 标注字段为可写，则可在构造函数或其他方法中注入。
        /// </remarks>
        /// <param name="injectable">可注入实例</param>
        public static void InjectServices(this IInjectable injectable)
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
                object val = HearthApp.App.Container.Resolve(serviceType, serviceKey: autowireAttribute.Key);
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