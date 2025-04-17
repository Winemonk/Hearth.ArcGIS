# InjectServices 方法


解析并注入 *injectable* 实例内的 <a href="T_Hearth_ArcGIS_InjectAttribute">InjectAttribute</a> 标注的所有字段。



## Definition
**命名空间：** <a href="N_Hearth_ArcGIS">Hearth.ArcGIS</a>  
**程序集：** Hearth.ArcGIS (在 Hearth.ArcGIS.dll 中) 版本：1.1.0+1226e772e9a2b3048c2f3c2f269f27f8f1fb249e

**C#**
``` C#
public static void InjectServices(
	this IInjectable injectable
)
```



#### 参数
<dl><dt>  <a href="T_Hearth_ArcGIS_IInjectable">IInjectable</a></dt><dd>可注入实例</dd></dl>

#### 备注
在 Visual Basic 和 C# 中，这个方法可以当成为类型 <a href="T_Hearth_ArcGIS_IInjectable">IInjectable</a> 的实例方法来调用。在采用实例方法语法调用这个方法时，请省略第一个参数。请参考 <a href="https://docs.microsoft.com/dotnet/visual-basic/programming-guide/language-features/procedures/extension-methods" target="_blank" rel="noopener noreferrer">

扩展方法 (Visual Basic)</a> 或 <a href="https://docs.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/extension-methods" target="_blank" rel="noopener noreferrer">

扩展方法 (C# 编程指南)</a> 获取更多信息。

## 备注
若 <a href="T_Hearth_ArcGIS_InjectAttribute">InjectAttribute</a> 标注字段为 `readonly` ，则必须在构造函数中注入。 若 <a href="T_Hearth_ArcGIS_InjectAttribute">InjectAttribute</a> 标注字段为可写，则可在构造函数或其他方法中注入。

## 参见


#### 引用
<a href="T_Hearth_ArcGIS_InjectableExtensions">InjectableExtensions 类</a>  
<a href="N_Hearth_ArcGIS">Hearth.ArcGIS 命名空间</a>  
