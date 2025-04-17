# RegisterAssemblyTypes 方法


注册程序集中所有 <a href="T_Hearth_ArcGIS_ServiceAttribute">ServiceAttribute</a> 特性标记的服务类型， 和实现 <a href="T_Hearth_ArcGIS_IService">IService</a> 的服务类型。



## Definition
**命名空间：** <a href="N_Hearth_ArcGIS">Hearth.ArcGIS</a>  
**程序集：** Hearth.ArcGIS (在 Hearth.ArcGIS.dll 中) 版本：1.1.0+1226e772e9a2b3048c2f3c2f269f27f8f1fb249e

**C#**
``` C#
public static void RegisterAssemblyTypes(
	this Container container,
	params Assembly[] assemblies
)
```



#### 参数
<dl><dt>  Container</dt><dd>Container 实例</dd><dt>  Assembly[]</dt><dd>程序集</dd></dl>

#### 备注
在 Visual Basic 和 C# 中，这个方法可以当成为类型 Container 的实例方法来调用。在采用实例方法语法调用这个方法时，请省略第一个参数。请参考 <a href="https://docs.microsoft.com/dotnet/visual-basic/programming-guide/language-features/procedures/extension-methods" target="_blank" rel="noopener noreferrer">

扩展方法 (Visual Basic)</a> 或 <a href="https://docs.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/extension-methods" target="_blank" rel="noopener noreferrer">

扩展方法 (C# 编程指南)</a> 获取更多信息。

## 参见


#### 引用
<a href="T_Hearth_ArcGIS_ContainerExtensions">ContainerExtensions 类</a>  
<a href="N_Hearth_ArcGIS">Hearth.ArcGIS 命名空间</a>  
