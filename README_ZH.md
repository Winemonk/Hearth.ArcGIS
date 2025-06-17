[English Documentation](README.md)

# Hearth ArcGIS 框架扩展（DryIoC、Options、Nlog、AutoMapper...）

## 1 使用IoC、DI

### 1.1 服务注册

#### 1.1.1 标记服务 

**方式一：使用特性标记**

需要注册服务类型时，在服务类型上添加`[Service]`标记：

```csharp
public interface IHelloService
{
    void SayHello();
}
```

```csharp
[Service]
public class HelloService : IHelloService
{
    public void SayHello()
    {
        Console.WriteLine("Hello, World!");
    }
}
```

`ServiceAttribute`服务标记特性

```csharp
/// <summary>
/// 服务标记特性
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ServiceAttribute : Attribute
{
    /// <summary>
    /// 服务注册键
    /// </summary>
    public string? ServiceKey { get; set; }

    /// <summary>
    /// 服务注册类型
    /// </summary>
    public Type? ServiceType { get; set; }

    /// <summary>
    /// 服务重用模式
    /// </summary>
    public ReuseEnum Reuse { get; set; }

    /// <summary>
    /// 服务特性
    /// </summary>
    /// <param name="serviceType"> 服务注册类型 </param>
    /// <param name="serviceKey"> 服务注册键 </param>
    /// <param name="reuse"> 服务重用模式 </param>
    public ServiceAttribute(Type? serviceType = null, string? serviceKey = null, ReuseEnum reuse = ReuseEnum.Default)
    {
        ServiceType = serviceType;
        ServiceKey = serviceKey;
        Reuse = reuse;
    }
}
```


`ReuseEnum`服务重用模式：

```csharp
/// <summary>
/// 重用模式枚举
/// </summary>
public enum ReuseEnum
{
    /// <summary>
    /// 默认。
    /// </summary>
    Default,

    /// <summary>
    /// 与作用域相同，但需要 <see cref="ThreadScopeContext"/> 。
    /// </summary>
    InThread,

    /// <summary>
    /// 作用域为任何作用域，可以有名称也可以没有名称。
    /// </summary>
    Scoped,

    /// <summary>
    /// 与 <see cref="Scoped"/> 相同，但在没有可用作用域的情况下，将回退到 <see cref="Singleton"/> 重用。
    /// </summary>
    ScopedOrSingleton,

    /// <summary>
    /// 容器中单例。
    /// </summary>
    Singleton,

    /// <summary>
    /// 瞬态，即不会重复使用。
    /// </summary>
    Transient,
}
```

**方式二：继承服务接口**

使需要注册服务类型实现`ITransientService`、`ISingletonService`、`IScopedService`、`IScopedOrSingletonService`、`IInThreadService`接口：

```csharp
public class HelloService : IHelloService, ITransientService
{
    public void SayHello()
    {
        Console.WriteLine("Hello, World!");
    }
}
```

#### 1.1.2 注册服务

在模块加载时调用`HearthApp.Container.RegisterAssemblyAndRefrencedAssembliesTypes(Assembly assembly)`方法，自动注册模块`Assembly`及所引用的全部`Assembly`中的服务类型。

注册程序集类型：

```csharp
internal class Module1 : Module
{
    private static Module1 _this = null;

    public static Module1 Current => _this ??= (Module1)FrameworkApplication.FindModule("Hearth_ArcGIS_Samples_Module");

    public Module1()
    {
        HearthApp.App.RegisterAssemblyAndRefrencedAssembliesTypes(this.GetType().Assembly);
    }
}
```

### 1.2 依赖注入

#### 1.2.1 特性注入


```csharp
// IInjectable 默认注入
// IScopeInjectable 使用作用域注入
// INamedScopeInjectable 使用命名的作用域注入
internal class SampleButton1 : Button, IInjectable 
{
    [Inject]
    private readonly IHelloService? _helloService;

    public SampleButton1()
    {
        this.InjectServices();
        // 不需要[Inject]特性标注注入字段/属性，但字段/属性也不能使用 readonly/init
        // this.InjectPropertiesAndFields(); 
    }

    protected override void OnClick()
    {
        _helloService?.SayHello();
    }
}
```


##### 1.2.1.1 `InjectAttribute`特性

```csharp
/// <summary>
/// 自动注入特性
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public sealed class InjectAttribute : Attribute
{
    /// <summary>
    /// 服务注册键
    /// </summary>
    public object? Key { get; set; }

    /// <summary>
    /// 服务类型
    /// </summary>
    public Type? ServiceType { get; set; }

    /// <summary>
    /// 注入服务特性
    /// </summary>
    /// <param name="key"> 服务注册键 </param>
    /// <param name="serviceType"> 服务类型 </param>
    public InjectAttribute(object? key = null, Type? serviceType = null)
    {
        Key = key;
        ServiceType = serviceType;
    }
}
```

#### 1.2.2 构造函数注入

##### 1.2.2.1 默认注入

服务注册：

```csharp
public interface IHelloService
{
    void SayHello();
}

[Service]
public class HelloHearthAService : IHelloService
{
    public void SayHello()
    {
        Console.WriteLine("Hello, Hearth A!");
    }
}
```

构造函数注入：

```csharp
public class HelloTest
{
    private readonly IHelloService _service;
    public HelloTest(IHelloService service)
    {
        this._service = service;
    }
}
```

##### 1.2.2.2 根据键注入

服务注册：

```csharp
public interface IHelloService
{
    void SayHello();
}

[Service(typeof(IHelloService),"A",ReuseEnum.Transient)]
public class HelloHearthAService : IHelloService
{
    public void SayHello()
    {
        Console.WriteLine("Hello, Hearth A!");
    }
}

[Service(typeof(IHelloService),"B",ReuseEnum.Transient)]
public class HelloHearthBService : IHelloService
{
    public void SayHello()
    {
        Console.WriteLine("Hello, Hearth B!");
    }
}
```

构造函数注入：

```csharp
public class HelloTest
{
    private readonly IHelloService _aService;
    private readonly IHelloService _bService;
    public HelloTest([InjectParam("A")]IHelloService aService, [InjectParam("B")]IHelloService bService)
    {
        this._aService = aService;
        this._bService = bService;
    }
}
```


#### 1.2.3 视图模型定位器

对于自定义的View组件，可以在View的`xaml`中使用`ViewModelLocator`（视图模型定位器）来绑定View的视图模型。视图模型类型也可以使用`Service`标记来进行自定义注册，对于未注册的视图模型类型，`Hearth`会对绑定的视图模型进行默认注册、注入。

```csharp
using ArcGIS.Desktop.Framework.Contracts;

namespace Hearth.ArcGIS.Samples.Dialogs
{
    public class SampleWindow1ViewModel : ViewModelBase
    {
        private string _sampleText = "Sample Text";
        public string SampleText
        {
            get => _sampleText;
            set => SetProperty(ref _sampleText, value, () => SampleText);
        }
    }
}
```

```xml
<Window
    x:Class="Hearth.ArcGIS.Samples.Dialogs.SampleWindow1"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:ha="clr-namespace:Hearth.ArcGIS;assembly=Hearth.ArcGIS"
    xmlns:local="clr-namespace:Hearth.ArcGIS.Samples.Dialogs"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    Title="SampleWindow1"
    Width="800"
    Height="450"
    ha:ViewModelLocator.AutoWireViewModel="True"
    mc:Ignorable="d">
    <Grid>
        <TextBlock
            HorizontalAlignment="Center"
            VerticalAlignment="Center"
            FontSize="24"
            FontWeight="Bold"
            Text="{Binding SampleText}" />
    </Grid>
</Window>
```

### 1.3 自定义容器初始化

HearthApp已经内置了DryIoc容器初始化、Nlog、ViewModelLocationProvider集成，当然也支持自定义初始化。

实现`ContainerBuilderBase`与`HearthAppBase`：

```csharp
using DryIoc;

public class CustomContainerBuilder : ContainerBuilder
{
    public override Container Build()
    {
        Container container = new Container(
            rules => rules
                .With(
                    FactoryMethod.ConstructorWithResolvableArgumentsIncludingNonPublic,
                    null,
                    PropertiesAndFields.All()));
        AddNlog(container);
        AddAutoMapper(container);
        AddViewModelLocationProvider(container);
        return container;
    }
}
```

```csharp
public class CustomHearthApp : HearthAppBase
{
    private static CustomHearthApp? _instance;
    public static CustomHearthApp Instance => _instance ??= new CustomHearthApp(new CustomContainerBuilder());
    public CustomHearthApp(IContainerBuilder containerBuilder) : base(containerBuilder)
    {

    }
}
```

在使用依赖注入之前完成容器初始化、服务注册。

```csharp
CustomHearthApp.Instance.Container.RegisterAssemblyAndRefrencedAssembliesTypes(this.GetType().Assembly);
```


## 2 使用Options配置

Options使用详见: [Options Documentation](https://learn.microsoft.com/zh-cn/dotnet/core/extensions/options#options-interfaces)

### 2.1 创建配置类

```csharp
namespace Hearth.ArcGIS.Samples.Configs
{
    public class SampleSettings
    {
        public string Value1 { get; set; }
        public int Value2 { get; set; }
        public double Value3 { get; set; }
        public string[] Value4 { get; set; }
    }
}
```

### 2.2 在模块初始化时注册配置

```csharp
internal class Module1 : Module
{
    private static Module1 _this = null;

    public static Module1 Current => _this ??= (Module1)FrameworkApplication.FindModule("Hearth_ArcGIS_Samples_Module");

    public Module1()
    {
        // samplesettings.json文件内容
        // "SampleSettings": {
        //     "Value1": "asd",
        //     "Value2": 123,
        //     "Value3": 123.456,
        //     "Value4": [
        //         "asd",
        //         "zxc",
        //         "qwe"
        //     ]
        // }
        IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("samplesettings.json", true, true).Build();
        HearthApp.App.Configure<SampleSettings>(configuration.GetSection(typeof(SampleSettings).Name));

        HearthApp.App.RegisterAssemblyAndRefrencedAssembliesTypes(this.GetType().Assembly);
    }
}
```

### 2.3 实现 `IConfigurationProvider<T>` 自动配置

```csharp
// 实现此接口后，程序初始化时会自动扫描并注册配置，不需要主动注册
public class SampleSettingsConfigurationProvider : IConfigurationProvider<SampleSettings>
{
    public IConfiguration GetConfiguration()
    {
        IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("samplesettings.json", true, true).Build();
        return configuration.GetSection(typeof(SampleSettings).Name);
    }
}
```


## 3 使用NLog日志

### 3.1 日志配置

配置文件存储位置：...\Pro\bin\nlog.config （Pro安装目录的bin文件夹中）

有关详细信息，请参阅[NLog 教程](https://github.com/NLog/NLog/wiki/Tutorial)、[高级 NLog 配置文件](https://github.com/NLog/NLog/wiki/Configuration-file)和[NLog 配置选项](https://nlog-project.org/config/)。

配置文件样例：

```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      xsi:schemaLocation="http://www.nlog-project.org/schemas/NLog.xsd NLog.xsd"
      autoReload="true"
      throwExceptions="false"
      internalLogLevel="OFF" internalLogFile="c:\temp\nlog-internal.log">
	<variable name="logDirectory" value="${basedir}/GeoApp/logs"/>
	<targets>
		<target xsi:type="File" name="f_all" fileName="${logDirectory}/${shortdate}.log" archiveNumbering="Sequence" archiveEvery="Day" maxArchiveDays="30" archiveAboveSize="104857600"
				layout="[${longdate}] ${threadid} ${level} ${callsite} ${callsite-linenumber} ${message} ${exception}" />
	</targets>
	<rules>
		<logger name="Hearth.*" minlevel="Debug" writeTo="f_all" />
		<logger name="*" minlevel="Warn" writeTo="f_all" />
	</rules>
</nlog>
```

### 3.2 日志记录器使用

```csharp
[Service]
public class TestLogService
{
    private readonly ILogger<TestLogService> _logger;
    public TestLogService(ILogger<TestLogService> logger)
    {
        _logger = logger;
    }

    public void WriteLog()
    {
        _logger?.LogTrace("Configured Type Logger Class LogTrace");
        _logger?.LogDebug("Configured Type Logger Class LogDebug");
        _logger?.LogInformation("Configured Type Logger Class LogInformation");
        _logger?.LogWarning("Configured Type Logger Class LogWarning");
        _logger?.LogError("Configured Type Logger Class LogError");
        _logger?.LogCritical("Configured Type Logger Class LogCritical");
    }
}
```

日志输出：
```bash
[2025-02-19 15:34:02.0141] 1 Trace Hearth.ArcGIS.Samples.Services.TestLogService.WriteLog 16 Configured Type Logger Class LogTrace 
[2025-02-19 15:34:02.0141] 1 Debug Hearth.ArcGIS.Samples.Services.TestLogService.WriteLog 17 Configured Type Logger Class LogDebug 
[2025-02-19 15:34:02.0141] 1 Info Hearth.ArcGIS.Samples.Services.TestLogService.WriteLog 18 Configured Type Logger Class LogInformation 
[2025-02-19 15:34:02.0141] 1 Warn Hearth.ArcGIS.Samples.Services.TestLogService.WriteLog 19 Configured Type Logger Class LogWarning 
[2025-02-19 15:34:02.0141] 1 Error Hearth.ArcGIS.Samples.Services.TestLogService.WriteLog 20 Configured Type Logger Class LogError 
[2025-02-19 15:34:02.0141] 1 Fatal Hearth.ArcGIS.Samples.Services.TestLogService.WriteLog 21 Configured Type Logger Class LogCritical 
```

## 4 使用AutoMapper

AutoMapper使用详见：[AutoMapper Documentation](https://docs.automapper.org/en/stable/)

### 4.1 模型

```csharp
public class Person
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public DateTime Birthday { get; set; }
}

public class PersonVO : ViewModelBase
{
    private Guid _id;
    public Guid Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }
    private string _name;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }
    private int _age;
    public int Age
    {
        get => _age;
        set => SetProperty(ref _age, value);
    }
    private DateTime _birthday;
    public DateTime Birthday
    {
        get => _birthday;
        set => SetProperty(ref _birthday, value);
    }
}
```

### 4.2 配置

**方式一：使用Profile配置**

```csharp
using AutoMapper;

namespace Hearth.ArcGIS.Samples
{
    public class PersonProfile : Profile
    {
        public PersonProfile()
        {
            CreateMap<Person, PersonVO>().ReverseMap();
        }
    }
}
```

**方式二：使用[AutoMap]特性标记**

```csharp
[AutoMap(typeof(PersonVO))]
public class Person
{
    // ...
}

[AutoMap(typeof(Person))]
public class PersonVO : ViewModelBase
{
    // ...
}
```

### 4.3 使用

```csharp
public class SomeSample
{
    private readonly IMapper _mapper;
    public SomeSample(IMapper mapper)
    {
        _mapper = mapper;
    }

    public void DoSomeThings(PersonVO personVO)
    {
        // ...
        Person person = _mapper.Map<Person>(personVO);
    }
}
```