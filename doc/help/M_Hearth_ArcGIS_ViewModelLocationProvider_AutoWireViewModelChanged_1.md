# AutoWireViewModelChanged(Object, Type, Action&lt;Object, Object&gt;) 方法


根据视图模型类型 Type 自动查找与当前视图对应的视图模型实例，并将其绑定到视图的数据上下文中。



## Definition
**命名空间：** <a href="N_Hearth_ArcGIS">Hearth.ArcGIS</a>  
**程序集：** Hearth.ArcGIS (在 Hearth.ArcGIS.dll 中) 版本：1.1.0+b41cd873c7cefaa2ff35e931610670fab36c4094

**C#**
``` C#
public static void AutoWireViewModelChanged(
	Object view,
	Type? viewModelType,
	Action<Object, Object> setDataContextCallback
)
```



#### 参数
<dl><dt>  Object</dt><dd>依赖对象，通常是视图。</dd><dt>  Type</dt><dd>视图模型类型</dd><dt>  Action(Object, Object)</dt><dd>用于创建视图和视图模型之间绑定的回调</dd></dl>

## 参见


#### 引用
<a href="T_Hearth_ArcGIS_ViewModelLocationProvider">ViewModelLocationProvider 类</a>  
<a href="Overload_Hearth_ArcGIS_ViewModelLocationProvider_AutoWireViewModelChanged">AutoWireViewModelChanged 重载</a>  
<a href="N_Hearth_ArcGIS">Hearth.ArcGIS 命名空间</a>  
