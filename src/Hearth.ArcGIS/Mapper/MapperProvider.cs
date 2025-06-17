using AutoMapper;
using DryIoc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;

namespace Hearth.ArcGIS
{
    /// <summary>
    /// Mapper分发器
    /// </summary>
    internal class MapperProvider : IMapperConfigurator
    {
        private readonly object _syncLock = new object();
        private readonly HashSet<Type> _profileTypes = new HashSet<Type>();
        private readonly HashSet<Assembly> _assemblies = new HashSet<Assembly>();

        private readonly ILogger<MapperProvider>? _logger;

        /// <summary>
        /// 用于执行映射的配置提供程序
        /// </summary>
        public IConfigurationProvider ConfigurationProvider { get; private set; }

        /// <summary>
        /// Mapper分发器
        /// </summary>
        public MapperProvider(ILogger<MapperProvider> logger)
        {
            _logger = logger;
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        private void UpdateMapper()
        {
            ConfigurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(_assemblies);
                _logger?.LogDebug("配置映射程序集：{@assemblies}", _assemblies.Select(a => a.FullName));
                foreach (var profileType in _profileTypes)
                {
                    cfg.AddProfile(profileType);
                    _logger?.LogDebug("配置映射类型：{@profileType}", profileType.FullName);
                }
            });
            HearthAppBase.CurrentApp.Container.RegisterInstance(typeof(IMapper), ConfigurationProvider.CreateMapper(), IfAlreadyRegistered.Replace);
        }

        /// <summary>
        /// 添加映射配置
        /// </summary>
        /// <param name="profileTypes">映射配置类型</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public void AddProfiles(params Type[] profileTypes)
        {
            lock (_syncLock)
            {
                int count = _profileTypes.Count;
                foreach (var profileType in profileTypes)
                    _profileTypes.Add(profileType);
                if (_profileTypes.Count > count)
                    UpdateMapper();
            }
        }

        /// <summary>
        /// 添加映射配置
        /// </summary>
        /// <param name="assemblies">扫描程序集</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public void AddProfiles(params Assembly[] assemblies)
        {
            lock (_syncLock)
            {
                int count = _profileTypes.Count;
                int assemblyCount = _assemblies.Count;
                foreach (var assembly in assemblies)
                {
                    if (assembly == typeof(Profile).Assembly || assembly.IsDynamic)
                        continue;
                    _assemblies.Add(assembly);
                    try
                    {
                        IEnumerable<Type> profileTypes = assembly.GetTypes().Where(type => type.IsClass
                                && !type.IsAbstract
                                && typeof(Profile).IsAssignableFrom(type));
                        foreach (var profileType in profileTypes)
                            _profileTypes.Add(profileType);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "未能从 {assemblyName} 程序集中加载配置文件。", assembly.FullName);
                    }
                }
                if (_profileTypes.Count > count || _assemblies.Count > assemblyCount)
                    UpdateMapper();
            }
        }
    }
}