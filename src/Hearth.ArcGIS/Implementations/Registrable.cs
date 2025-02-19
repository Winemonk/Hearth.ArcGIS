namespace Hearth.ArcGIS
{
    /// <summary>
    /// 可注册实例
    /// </summary>
    public class Registrable : IRegistrable
    {
        /// <summary>
        /// 可注册实例，自动调用 <see cref="IRegistrableExtensions.RegisterServices(IRegistrable)"/> 方法注册服务。
        /// </summary>
        public Registrable()
        {
            this.RegisterServices();
        }
    }
}