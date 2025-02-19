using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace Hearth.ArcGIS
{
    /// <summary>
    /// “视图模型位置提供程序”类定位附加了“自动连接视图模型更改”属性设置为true的视图的视图模型。
    /// </summary>
    public static class ViewModelLocationProvider
    {
        /// <summary>
        /// 重置视图模型位置提供程序以进行单元测试。
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Reset()
        {
            _factories = new Dictionary<string, Func<object>>();
            _typeFactories = new Dictionary<string, Type>();
            _defaultViewModelFactory = type => Activator.CreateInstance(type);
            _defaultViewModelFactoryWithViewParameter = null;
            _defaultViewTypeToViewModelTypeResolver = DefaultViewTypeToViewModel;
        }

        /// <summary>
        /// 包含视图所有已注册工厂的词典。
        /// </summary>
        private static Dictionary<string, Func<object>> _factories = new Dictionary<string, Func<object>>();

        /// <summary>
        /// 包含视图的所有已注册视图模型类型的字典。
        /// </summary>
        private static Dictionary<string, Type> _typeFactories = new Dictionary<string, Type>();

        /// <summary>
        /// 默认视图模型工厂，提供视图模型类型作为参数。
        /// </summary>
        private static Func<Type, object> _defaultViewModelFactory = type => Activator.CreateInstance(type);

        /// <summary>
        /// 视图模型工厂，提供视图实例和视图模型类型作为参数。
        /// </summary>
        private static Func<object, Type, object>? _defaultViewModelFactoryWithViewParameter;

        /// <summary>
        /// 视图模型类型解析器的默认视图类型假定视图模型与视图类型位于同一程序集中，但位于“视图模型”命名空间中。
        /// </summary>
        private static Func<Type, Type?> _defaultViewTypeToViewModelTypeResolver = DefaultViewTypeToViewModel;

        private static Type? DefaultViewTypeToViewModel(Type viewType)
        {
            // 策略1
            var viewName = viewType.FullName;
            var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            var suffix = viewName != null && viewName.EndsWith("View") ? "Model" : "ViewModel";
            var viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}{1}, {2}", viewName, suffix, viewAssemblyName);

            // 策略2
            var replacedNamespacedViewName = viewName?.Replace(".Views.", ".ViewModels.");
            var replacedNamespacedViewModelName = string.Format(CultureInfo.InvariantCulture, "{0}{1}, {2}", viewName, suffix, viewAssemblyName);

            return Type.GetType(viewModelName) ?? Type.GetType(replacedNamespacedViewModelName);
        }

        private static Func<object, Type?> _defaultViewToViewModelTypeResolver = view => null;

        /// <summary>
        /// 设置默认视图模型工厂。
        /// </summary>
        /// <param name="viewModelFactory">提供视图模型类型作为参数的视图模型工厂。</param>
        public static void SetDefaultViewModelFactory(Func<Type, object> viewModelFactory)
        {
            _defaultViewModelFactory = viewModelFactory;
        }

        /// <summary>
        /// 设置默认视图模型工厂。
        /// </summary>
        /// <param name="viewModelFactory">提供视图实例和视图模型类型作为参数的视图模型工厂。</param>
        public static void SetDefaultViewModelFactory(Func<object, Type, object> viewModelFactory)
        {
            _defaultViewModelFactoryWithViewParameter = viewModelFactory;
        }

        /// <summary>
        /// 将默认视图类型设置为视图模型类型解析器。
        /// </summary>
        /// <param name="viewTypeToViewModelTypeResolver">要查看模型类型解析器的视图类型。</param>
        public static void SetDefaultViewTypeToViewModelTypeResolver(Func<Type, Type?> viewTypeToViewModelTypeResolver)
        {
            _defaultViewTypeToViewModelTypeResolver = viewTypeToViewModelTypeResolver;
        }

        /// <summary>
        /// 设置给定视图实例的默认视图模型类型解析器。这可用于评估视图的自定义属性或附着属性，以确定视图模型类型。
        /// </summary>
        public static void SetDefaultViewToViewModelTypeResolver(Func<object, Type?> viewToViewModelTypeResolver) =>
            _defaultViewToViewModelTypeResolver = viewToViewModelTypeResolver;

        /// <summary>
        /// 视图模型将被定位并注入到视图的数据上下文中。为了定位视图模型，使用了两种策略：
        /// 首先，视图模型位置提供程序将查看是否有为该视图注册的视图模型工厂，如果没有，它将尝试使用基于约定的方法推断视图模型。
        /// 该类还提供了用于注册视图模型工厂的方法，并且还覆盖默认视图模型工厂和默认视图类型以查看模型类型解析器。
        /// </summary>
        /// <remarks>
        /// 约定的方法推断（按优先级排序）：
        /// 1. 视图模型与视图类型位于同一命名空间中，但以“ViewModel”结尾：
        /// <code>App.Test/TestView -> App.TestViewModel</code>
        /// <para>
        /// 2. 视图模型与视图类型位于同一程序集中，但位于“ViewModels”命名空间中：
        /// <code>App.Views.Test/TestView -> App.ViewModels.TestViewModel</code>
        /// </para>
        /// </remarks>
        /// <param name="view">依赖对象，通常是视图。</param>
        /// <param name="setDataContextCallback">用于创建视图和视图模型之间绑定的回调</param>
        public static void AutoWireViewModelChanged(object view, Action<object, object> setDataContextCallback)
        {
            // 先尝试映射
            object? viewModel = GetViewModelForView(view);

            // 尝试使用视图模型类型
            if (viewModel == null)
            {
                // 检查类型映射
                Type? viewModelType = GetViewModelTypeForView(view.GetType());

                // 检查平台视图到视图模型解析器
                if (viewModelType == null)
                    viewModelType = _defaultViewToViewModelTypeResolver(view);

                // 回到基于惯例的模式
                if (viewModelType == null)
                    viewModelType = _defaultViewTypeToViewModelTypeResolver(view.GetType());

                if (viewModelType == null)
                    return;

                viewModel = _defaultViewModelFactoryWithViewParameter != null ? _defaultViewModelFactoryWithViewParameter(view, viewModelType) : _defaultViewModelFactory(viewModelType);
            }

            setDataContextCallback(view, viewModel);
        }

        /// <summary>
        /// 根据视图模型类型 <see cref="Type"/> 自动查找与当前视图对应的视图模型实例，并将其绑定到视图的数据上下文中。
        /// </summary>
        /// <param name="view">依赖对象，通常是视图。</param>
        /// <param name="viewModelType">视图模型类型</param>
        /// <param name="setDataContextCallback">用于创建视图和视图模型之间绑定的回调</param>
        public static void AutoWireViewModelChanged(object view, Type? viewModelType, Action<object, object> setDataContextCallback)
        {
            if (viewModelType == null)
            {
                AutoWireViewModelChanged(view, setDataContextCallback);
                return;
            }

            var viewModel = _defaultViewModelFactoryWithViewParameter != null ? _defaultViewModelFactoryWithViewParameter(view, viewModelType) : _defaultViewModelFactory(viewModelType);

            setDataContextCallback(view, viewModel);
        }

        /// <summary>
        /// 获取指定视图的视图模型。
        /// </summary>
        /// <param name="view">视图模型想要的视图。</param>
        /// <returns>与作为参数传递的视图相对应的视图模型。</returns>
        private static object? GetViewModelForView(object view)
        {
            var viewKey = view.GetType().ToString();

            // 基于视图类型（或实例）的视图模型映射如下
            return _factories.ContainsKey(viewKey) ? _factories[viewKey]() : null;
        }

        /// <summary>
        /// 获取指定视图的视图模型类型。
        /// </summary>
        /// <param name="view">视图模型所需的视图。</param>
        /// <returns>与视图对应的视图模型类型。</returns>
        private static Type? GetViewModelTypeForView(Type view)
        {
            var viewKey = view.ToString();

            return _typeFactories.ContainsKey(viewKey) ? _typeFactories[viewKey] : null;
        }

        /// <summary>
        /// 为指定的视图类型注册视图模型工厂。
        /// </summary>
        /// <typeparam name="T">视图</typeparam>
        /// <param name="factory">视图模型工厂。</param>
        public static void Register<T>(Func<object> factory)
        {
            Register(typeof(T).ToString(), factory);
        }

        /// <summary>
        /// 为指定的视图类型名称注册视图模型工厂。
        /// </summary>
        /// <param name="viewTypeName">视图类型的名称。</param>
        /// <param name="factory">视图模型工厂。</param>
        public static void Register(string viewTypeName, Func<object> factory)
        {
            _factories[viewTypeName] = factory;
        }

        /// <summary>
        /// 为指定的视图类型注册视图模型类型。
        /// </summary>
        /// <typeparam name="T">视图</typeparam>
        /// <typeparam name="VM">视图模型</typeparam>
        public static void Register<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] T, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] VM>()
        {
            var viewType = typeof(T);
            var viewModelType = typeof(VM);

            Register(viewType.ToString(), viewModelType);
        }

        /// <summary>
        /// 为指定的视图注册视图模型类型。
        /// </summary>
        /// <param name="viewTypeName">视图类型名称</param>
        /// <param name="viewModelType">视图模型类型</param>
        public static void Register(string viewTypeName, Type viewModelType)
        {
            _typeFactories[viewTypeName] = viewModelType;
        }
    }
}