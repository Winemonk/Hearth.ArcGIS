namespace Hearth.ArcGIS
{
    /// <summary>
    /// 服务特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ServiceAttribute : Attribute
    {
        /// <summary>
        /// 服务注册键
        /// </summary>
        public string? ServiceKey { get; set; }

        /// <summary>
        /// 服务注册类型
        /// </summary>
        public Type? ServiceType { get; set; }

        /// <summary>
        /// 服务重用模式
        /// </summary>
        public ReuseEnum Reuse { get; set; }

        /// <summary>
        /// 服务特性
        /// </summary>
        /// <param name="serviceType"> 服务注册类型 </param>
        /// <param name="serviceKey"> 服务注册键 </param>
        /// <param name="reuse"> 服务重用模式 </param>
        public ServiceAttribute(Type? serviceType, string? serviceKey = null, ReuseEnum reuse = ReuseEnum.Default)
        {
            ServiceType = serviceType;
            ServiceKey = serviceKey;
            Reuse = reuse;
        }
    }
}