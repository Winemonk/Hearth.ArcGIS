using AutoMapper;
using System.Linq.Expressions;
using System.Reflection;

namespace Hearth.ArcGIS
{
    /// <summary>
    /// Mapper分发器
    /// </summary>
    public class MapperProvider : IMapperConfigurator, IMapper
    {
        private readonly object _syncLock = new object();
        private readonly HashSet<Type> _profileTypes = new HashSet<Type>();
        private IMapper _currentMapper;
        private readonly Exception _notInitException = new NotSupportedException("Mapper is not initialized.");

        /// <summary>
        /// 用于执行映射的配置提供程序
        /// </summary>
        public IConfigurationProvider ConfigurationProvider { get; private set; }

        /// <summary>
        /// Mapper分发器
        /// </summary>
        public MapperProvider()
        {
        }

        private void UpdateMapper()
        {
            ConfigurationProvider = new MapperConfiguration(cfg =>
            {
                foreach (var profileType in _profileTypes)
                {
                    cfg.AddProfile(profileType);
                }
            });
            _currentMapper = ConfigurationProvider.CreateMapper();
        }

        /// <summary>
        /// 将源实例映射到目标类型。
        /// </summary>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="source">源实例</param>
        /// <returns>目标实例</returns>
        public TDestination Map<TDestination>(object source) => _currentMapper.ThrowIfNull(_notInitException).Map<TDestination>(source);
        /// <summary>
        /// 将源实例映射到目标类型。
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="source">源实例</param>
        /// <returns>目标实例</returns>
        public TDestination Map<TSource, TDestination>(TSource source) => _currentMapper.ThrowIfNull(_notInitException).Map<TSource, TDestination>(source);
        /// <summary>
        /// 将源实例映射到目标类型。
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="source">源实例</param>
        /// <param name="destination">目标实例</param>
        /// <returns>目标实例</returns>
        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination) => _currentMapper.ThrowIfNull(_notInitException).Map(source, destination);
        /// <summary>
        /// 将源实例映射到目标类型。
        /// </summary>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="source">源实例</param>
        /// <param name="opts">映射选项</param>
        /// <returns>目标类型实例</returns>
        public TDestination Map<TDestination>(object source, Action<IMappingOperationOptions<object, TDestination>> opts) => _currentMapper.ThrowIfNull(_notInitException).Map(source, opts);
        /// <summary>
        /// 将源实例映射到目标类型。
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="source">源实例</param>
        /// <param name="opts">映射选项</param>
        /// <returns>目标实例</returns>
        public TDestination Map<TSource, TDestination>(TSource source, Action<IMappingOperationOptions<TSource, TDestination>> opts) => _currentMapper.ThrowIfNull(_notInitException).Map(source, opts);
        /// <summary>
        /// 将源实例映射到目标类型。
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="source">源实例</param>
        /// <param name="destination">目标实例</param>
        /// <param name="opts">映射选项</param>
        /// <returns>目标实例</returns>
        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination, Action<IMappingOperationOptions<TSource, TDestination>> opts) => _currentMapper.ThrowIfNull(_notInitException).Map(source, destination, opts);
        /// <summary>
        /// 将源实例映射到目标类型。
        /// </summary>
        /// <param name="source">源实例</param>
        /// <param name="sourceType">源类型</param>
        /// <param name="destinationType">目标类型</param>
        /// <param name="opts">映射选项</param>
        /// <returns>目标实例</returns>
        public object Map(object source, Type sourceType, Type destinationType, Action<IMappingOperationOptions<object, object>> opts) => _currentMapper.ThrowIfNull(_notInitException).Map(source, sourceType, destinationType, opts);
        /// <summary>
        /// 将源实例映射到目标类型。
        /// </summary>
        /// <param name="source">源实例</param>
        /// <param name="sourceType">源类型</param>
        /// <param name="destination">目标实例</param>
        /// <param name="destinationType">目标类型</param>
        /// <param name="opts">映射选项</param>
        /// <returns>目标实例</returns>
        public object Map(object source, object destination, Type sourceType, Type destinationType, Action<IMappingOperationOptions<object, object>> opts) => _currentMapper.ThrowIfNull(_notInitException).Map(source, destination, sourceType, destinationType, opts);
        /// <summary>
        /// 投影可查询的输入。
        /// </summary>
        /// <remarks>投影只计算一次并缓存</remarks>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="source">可查询源</param>
        /// <param name="parameters">参数化映射表达式的可选参数对象</param>
        /// <param name="membersToExpand">要扩展的明确成员</param>
        /// <returns>可查询的结果，使用可查询的扩展方法来投影和执行结果</returns>
        public IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source, object parameters = null, params Expression<Func<TDestination, object>>[] membersToExpand) => _currentMapper.ThrowIfNull(_notInitException).ProjectTo<TDestination>(source, parameters, membersToExpand);
        /// <summary>
        /// 投影可查询的输入。
        /// </summary>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="source">可查询源</param>
        /// <param name="parameters">参数化映射表达式的可选参数对象</param>
        /// <param name="membersToExpand">要扩展的明确成员</param>
        /// <returns>可查询的结果，使用可查询的扩展方法来投影和执行结果</returns>
        public IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source, IDictionary<string, object> parameters, params string[] membersToExpand) => _currentMapper.ThrowIfNull(_notInitException).ProjectTo<TDestination>(source, parameters, membersToExpand);
        /// <summary>
        /// 投影可查询的输入。
        /// </summary>
        /// <param name="source">可查询源</param>
        /// <param name="destinationType">目标类型</param>
        /// <param name="parameters">参数化映射表达式的可选参数对象</param>
        /// <param name="membersToExpand">要扩展的明确成员</param>
        /// <returns>可查询的结果，使用可查询的扩展方法来投影和执行结果</returns>
        public IQueryable ProjectTo(IQueryable source, Type destinationType, IDictionary<string, object> parameters = null, params string[] membersToExpand) => _currentMapper.ThrowIfNull(_notInitException).ProjectTo(source, destinationType, parameters, membersToExpand);
        /// <summary>
        /// 将源实例映射到目标类型。
        /// </summary>
        /// <param name="source">源实例</param>
        /// <param name="sourceType">源类型</param>
        /// <param name="destinationType">目标类型</param>
        /// <returns>目标实例</returns>
        public object Map(object source, Type sourceType, Type destinationType) => _currentMapper.ThrowIfNull(_notInitException).Map(source, sourceType, destinationType);
        /// <summary>
        /// 将源实例映射到目标类型。
        /// </summary>
        /// <param name="source">源实例</param>
        /// <param name="destination">目标实例</param>
        /// <param name="sourceType">源类型</param>
        /// <param name="destinationType">目标类型</param>
        /// <returns>目标实例</returns>
        public object Map(object source, object destination, Type sourceType, Type destinationType) => _currentMapper.ThrowIfNull(_notInitException).Map(source, destination, sourceType, destinationType);
        /// <summary>
        /// 添加映射配置
        /// </summary>
        /// <param name="profileTypes">映射配置类型</param>
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
        public void AddProfiles(params Assembly[] assemblies)
        {
            IEnumerable<Type> profileTypes = assemblies.SelectMany(a => a.GetTypes().Where(type => type.IsClass && !type.IsAbstract && typeof(Profile).IsAssignableFrom(type)));
            lock (_syncLock)
            {
                int count = _profileTypes.Count;
                foreach (var profileType in profileTypes)
                    _profileTypes.Add(profileType);
                if (_profileTypes.Count > count)
                    UpdateMapper();
            }
        }
    }
}