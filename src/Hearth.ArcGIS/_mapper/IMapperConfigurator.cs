using System.Reflection;

namespace Hearth.ArcGIS
{
    /// <summary>
    /// 映射器配置器接口
    /// </summary>
    public interface IMapperConfigurator
    {
        /// <summary>
        /// 添加映射配置
        /// </summary>
        /// <param name="profileTypes">映射配置类型</param>
        void AddProfiles(params Type[] profileTypes);

        /// <summary>
        /// 添加映射配置
        /// </summary>
        /// <param name="assemblies">扫描程序集</param>
        void AddProfiles(params Assembly[] assemblies);
    }
}