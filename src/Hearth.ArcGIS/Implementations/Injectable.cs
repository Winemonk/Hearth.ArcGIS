namespace Hearth.ArcGIS
{
    /// <summary>
    /// 可注入实例
    /// </summary>
    public class Injectable : IInjectable
    {
        /// <summary>
        /// 可注入实例，自动调用 <see cref="IInjectableExtensions.InjectServices(IInjectable)"/> 方法注入服务。
        /// </summary>
        public Injectable()
        {
            this.InjectServices();
        }
    }
}