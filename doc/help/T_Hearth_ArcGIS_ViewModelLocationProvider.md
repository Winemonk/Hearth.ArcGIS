# ViewModelLocationProvider 类


“视图模型位置提供程序”类定位附加了“自动连接视图模型更改”属性设置为true的视图的视图模型。



## Definition
**命名空间：** <a href="N_Hearth_ArcGIS">Hearth.ArcGIS</a>  
**程序集：** Hearth.ArcGIS (在 Hearth.ArcGIS.dll 中) 版本：1.1.0+1226e772e9a2b3048c2f3c2f269f27f8f1fb249e

**C#**
``` C#
public static class ViewModelLocationProvider
```

<table><tr><td><strong>Inheritance</strong></td><td>Object  →  ViewModelLocationProvider</td></tr>
</table>



## 方法
<table>
<tr>
<td><a href="M_Hearth_ArcGIS_ViewModelLocationProvider_AutoWireViewModelChanged">AutoWireViewModelChanged(Object, Action(Object, Object))</a></td>
<td>视图模型将被定位并注入到视图的数据上下文中。为了定位视图模型，使用了两种策略： 首先，视图模型位置提供程序将查看是否有为该视图注册的视图模型工厂，如果没有，它将尝试使用基于约定的方法推断视图模型。 该类还提供了用于注册视图模型工厂的方法，并且还覆盖默认视图模型工厂和默认视图类型以查看模型类型解析器。</td></tr>
<tr>
<td><a href="M_Hearth_ArcGIS_ViewModelLocationProvider_AutoWireViewModelChanged_1">AutoWireViewModelChanged(Object, Type, Action(Object, Object))</a></td>
<td>根据视图模型类型 Type 自动查找与当前视图对应的视图模型实例，并将其绑定到视图的数据上下文中。</td></tr>
<tr>
<td><a href="M_Hearth_ArcGIS_ViewModelLocationProvider_Register">Register(String, Func(Object))</a></td>
<td>为指定的视图类型名称注册视图模型工厂。</td></tr>
<tr>
<td><a href="M_Hearth_ArcGIS_ViewModelLocationProvider_Register_1">Register(String, Type)</a></td>
<td>为指定的视图注册视图模型类型。</td></tr>
<tr>
<td><a href="M_Hearth_ArcGIS_ViewModelLocationProvider_Register__1">Register(T)(Func(Object))</a></td>
<td>为指定的视图类型注册视图模型工厂。</td></tr>
<tr>
<td><a href="M_Hearth_ArcGIS_ViewModelLocationProvider_Register__2">Register(T, VM)()</a></td>
<td>为指定的视图类型注册视图模型类型。</td></tr>
<tr>
<td><a href="M_Hearth_ArcGIS_ViewModelLocationProvider_SetDefaultViewModelFactory">SetDefaultViewModelFactory(Func(Type, Object))</a></td>
<td>设置默认视图模型工厂。</td></tr>
<tr>
<td><a href="M_Hearth_ArcGIS_ViewModelLocationProvider_SetDefaultViewModelFactory_1">SetDefaultViewModelFactory(Func(Object, Type, Object))</a></td>
<td>设置默认视图模型工厂。</td></tr>
<tr>
<td><a href="M_Hearth_ArcGIS_ViewModelLocationProvider_SetDefaultViewToViewModelTypeResolver">SetDefaultViewToViewModelTypeResolver</a></td>
<td>设置给定视图实例的默认视图模型类型解析器。这可用于评估视图的自定义属性或附着属性，以确定视图模型类型。</td></tr>
<tr>
<td><a href="M_Hearth_ArcGIS_ViewModelLocationProvider_SetDefaultViewTypeToViewModelTypeResolver">SetDefaultViewTypeToViewModelTypeResolver</a></td>
<td>将默认视图类型设置为视图模型类型解析器。</td></tr>
</table>

## 参见


#### 引用
<a href="N_Hearth_ArcGIS">Hearth.ArcGIS 命名空间</a>  
