# AddDbContext&lt;TContext&gt;(IContainer, Action&lt;DbContextOptionsBuilder&gt;, ServiceLifetime, ServiceLifetime) 方法


注册 DbContext 到 IContainer 容器。



## Definition
**命名空间：** <a href="N_Hearth_ArcGIS_Infrastructure">Hearth.ArcGIS.Infrastructure</a>  
**程序集：** Hearth.ArcGIS.Infrastructure (在 Hearth.ArcGIS.Infrastructure.dll 中) 版本：1.2.0+b41cd873c7cefaa2ff35e931610670fab36c4094

**C#**
``` C#
public static IContainer AddDbContext<TContext>(
	this IContainer container,
	Action<DbContextOptionsBuilder>? optionsAction = null,
	ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
	ServiceLifetime optionsLifetime = ServiceLifetime.Scoped
)
where TContext : DbContext

```



#### 参数
<dl><dt>  IContainer</dt><dd>IContainer 容器</dd><dt>  Action(DbContextOptionsBuilder)  (Optional)</dt><dd><p>为上下文配置 DbContextOptions 的可选操作。 这提供了一种在派生上下文中重写 OnConfiguring(DbContextOptionsBuilder) 方法来执行上下文配置的替代方法。</p><p>

如果此处提供了一个操作，则如果在派生上下文上重写了 OnConfiguring(DbContextOptionsBuilder) 方法， 则该方法仍将运行 OnConfiguring(DbContextOptionsBuilder) 除了在此处执行的配置外，还将应用配置。</p><p>

为了将选项传递到上下文中，您需要在上下文中公开一个构造函数，该构造函数接受 DbContextOptions 并将其传递给 DbContext 的基构造函数。</p></dd><dt>

  ServiceLifetime  (Optional)</dt><dd>在容器中注册 DbContext 服务的生存期。</dd><dt>  ServiceLifetime  (Optional)</dt><dd>在容器中注册 DbContextOptions 上下文选项服务的生存期。</dd></dl>

#### 类型参数
<dl><dt /><dd>DbContext 类型</dd></dl>

#### 返回值
IContainer  
相同的容器，以便可以链接多个调用。

#### 备注
在 Visual Basic 和 C# 中，这个方法可以当成为类型 IContainer 的实例方法来调用。在采用实例方法语法调用这个方法时，请省略第一个参数。请参考 <a href="https://docs.microsoft.com/dotnet/visual-basic/programming-guide/language-features/procedures/extension-methods" target="_blank" rel="noopener noreferrer">

扩展方法 (Visual Basic)</a> 或 <a href="https://docs.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/extension-methods" target="_blank" rel="noopener noreferrer">

扩展方法 (C# 编程指南)</a> 获取更多信息。

## 参见


#### 引用
<a href="T_Hearth_ArcGIS_Infrastructure_InfrastructureContainerExtensions">InfrastructureContainerExtensions 类</a>  
<a href="Overload_Hearth_ArcGIS_Infrastructure_InfrastructureContainerExtensions_AddDbContext">AddDbContext 重载</a>  
<a href="N_Hearth_ArcGIS_Infrastructure">Hearth.ArcGIS.Infrastructure 命名空间</a>  
