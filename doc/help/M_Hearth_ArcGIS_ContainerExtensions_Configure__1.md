# Configure&lt;TOptions&gt; 方法


注册配置



## Definition
**命名空间：** <a href="N_Hearth_ArcGIS">Hearth.ArcGIS</a>  
**程序集：** Hearth.ArcGIS (在 Hearth.ArcGIS.dll 中) 版本：1.1.0+b41cd873c7cefaa2ff35e931610670fab36c4094

**C#**
``` C#
public static Container Configure<TOptions>(
	this Container container,
	IConfiguration configuration
)
where TOptions : class, new()

```



#### 参数
<dl><dt>  Container</dt><dd>Container 实例</dd><dt>  IConfiguration</dt><dd>IConfiguration 实例</dd></dl>

#### 类型参数
<dl><dt /><dd>配置类型</dd></dl>

#### 返回值
Container  
Container 实例

#### 备注
在 Visual Basic 和 C# 中，这个方法可以当成为类型 Container 的实例方法来调用。在采用实例方法语法调用这个方法时，请省略第一个参数。请参考 <a href="https://docs.microsoft.com/dotnet/visual-basic/programming-guide/language-features/procedures/extension-methods" target="_blank" rel="noopener noreferrer">

扩展方法 (Visual Basic)</a> 或 <a href="https://docs.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/extension-methods" target="_blank" rel="noopener noreferrer">

扩展方法 (C# 编程指南)</a> 获取更多信息。

## 参见


#### 引用
<a href="T_Hearth_ArcGIS_ContainerExtensions">ContainerExtensions 类</a>  
<a href="N_Hearth_ArcGIS">Hearth.ArcGIS 命名空间</a>  
