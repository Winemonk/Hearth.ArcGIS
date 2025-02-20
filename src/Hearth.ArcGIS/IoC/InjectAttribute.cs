namespace Hearth.ArcGIS
{
    /// <summary>
    /// 自动注入特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class InjectAttribute : Attribute
    {
        /// <summary>
        /// 服务注册键
        /// </summary>
        public object? Key { get; set; }

        /// <summary>
        /// 服务类型
        /// </summary>
        public Type? ServiceType { get; set; }

        /// <summary>
        /// 注入服务特性
        /// </summary>
        /// <param name="key"> 服务注册键 </param>
        /// <param name="serviceType"> 服务类型 </param>
        public InjectAttribute(object? key = null, Type? serviceType = null)
        {
            Key = key;
            ServiceType = serviceType;
        }
    }
}