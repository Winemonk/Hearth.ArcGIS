using System.Windows;

namespace Hearth.ArcGIS
{
    /// <summary>
    /// 此类定义了调用视图模型定位器的附加属性和相关更改处理程序。
    /// </summary>
    public static class ViewModelLocator
    {
        /// <summary>
        /// “自动线视图模型”附加属性。
        /// </summary>
        public static DependencyProperty AutoWireViewModelProperty = DependencyProperty.RegisterAttached("AutoWireViewModel", typeof(bool?), typeof(ViewModelLocator), new PropertyMetadata(defaultValue: null, propertyChangedCallback: AutoWireViewModelChanged));

        /// <summary>
        /// 获取 <see cref="AutoWireViewModelProperty"/> 附加属性的值。
        /// </summary>
        /// <param name="obj">目标元素。</param>
        /// <returns>附加到 <paramref name="obj"/> 元素的 <see cref="AutoWireViewModelProperty"/> 。</returns>
        public static bool? GetAutoWireViewModel(DependencyObject obj)
        {
            return (bool?)obj.GetValue(AutoWireViewModelProperty);
        }

        /// <summary>
        /// 设置附加属性 <see cref="AutoWireViewModelProperty"/> 。
        /// </summary>
        /// <param name="obj">目标元素。</param>
        /// <param name="value">附加属性的值。</param>
        public static void SetAutoWireViewModel(DependencyObject obj, bool? value)
        {
            obj.SetValue(AutoWireViewModelProperty, value);
        }

        private static void AutoWireViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var value = (bool?)e.NewValue;
            if (value.HasValue && value.Value)
            {
                d.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Loaded, () =>
                {
                    ViewModelLocationProvider.AutoWireViewModelChanged(
                        d,
                        d.GetValue(ViewModelLocator.ViewModelProperty) as Type,
                        Bind);
                });
            }
        }

        /// <summary>
        /// “视图模型”附加属性。
        /// </summary>
        public static DependencyProperty ViewModelProperty = DependencyProperty.RegisterAttached("ViewModel", typeof(Type), typeof(ViewModelLocator), new PropertyMetadata(defaultValue: null));

        /// <summary>
        /// 获取 <see cref="ViewModelProperty"/> 附加属性的值。
        /// </summary>
        /// <param name="obj">目标元素。</param>
        /// <returns>附加到 <paramref name="obj"/> 元素的 <see cref="ViewModelProperty"/> 。</returns>
        public static Type GetViewModel(DependencyObject obj)
        {
            return (Type)obj.GetValue(ViewModelProperty);
        }

        /// <summary>
        /// 设置附加属性 <see cref="ViewModelProperty"/> 。
        /// </summary>
        /// <param name="obj">目标元素。</param>
        /// <param name="value">附加属性的值。</param>
        public static void SetViewModel(DependencyObject obj, Type value)
        {
            obj.SetValue(ViewModelProperty, value);
        }

        /// <summary>
        /// 设置视图的数据上下文。
        /// </summary>
        /// <param name="view">用于设置数据上下文的视图。</param>
        /// <param name="viewModel">用作视图数据上下文的对象。</param>
        private static void Bind(object view, object viewModel)
        {
            if (view is FrameworkElement element)
                element.DataContext = viewModel;
        }
    }
}