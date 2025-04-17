# AutoWireViewModelChanged 方法


## 重载列表
<table>
<tr>
<td><a href="M_Hearth_ArcGIS_ViewModelLocationProvider_AutoWireViewModelChanged">AutoWireViewModelChanged(Object, Action(Object, Object))</a></td>
<td>视图模型将被定位并注入到视图的数据上下文中。为了定位视图模型，使用了两种策略： 首先，视图模型位置提供程序将查看是否有为该视图注册的视图模型工厂，如果没有，它将尝试使用基于约定的方法推断视图模型。 该类还提供了用于注册视图模型工厂的方法，并且还覆盖默认视图模型工厂和默认视图类型以查看模型类型解析器。</td></tr>
<tr>
<td><a href="M_Hearth_ArcGIS_ViewModelLocationProvider_AutoWireViewModelChanged_1">AutoWireViewModelChanged(Object, Type, Action(Object, Object))</a></td>
<td>根据视图模型类型 Type 自动查找与当前视图对应的视图模型实例，并将其绑定到视图的数据上下文中。</td></tr>
</table>

## 参见


#### 引用
<a href="T_Hearth_ArcGIS_ViewModelLocationProvider">ViewModelLocationProvider 类</a>  
<a href="N_Hearth_ArcGIS">Hearth.ArcGIS 命名空间</a>  
