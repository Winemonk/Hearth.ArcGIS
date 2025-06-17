[中文文档](README_ZH.md)

# Hearth ArcGIS Framework Extensions (DryIoC, Options, NLog, AutoMapper...)

## 1 Using IoC and DI

### 1.1 Service Registration

#### 1.1.1 Marking Services

**Method 1: Using Attributes**  

Mark service types with the `[Service]` attribute:


```csharp
public interface IHelloService
{
    void SayHello();
}

[Service]
public class HelloService : IHelloService
{
    public void SayHello()
    {
        Console.WriteLine("Hello, World!");
    }
}
```

`ServiceAttribute` definition:

```csharp
/// <summary>
/// Service marker attribute
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ServiceAttribute : Attribute
{
    /// <summary>
    /// Service registration key
    /// </summary>
    public string? ServiceKey { get; set; }

    /// <summary>
    /// Service registration type
    /// </summary>
    public Type? ServiceType { get; set; }

    /// <summary>
    /// Service reuse mode
    /// </summary>
    public ReuseEnum Reuse { get; set; }

    /// <summary>
    /// Service attribute
    /// </summary>
    /// <param name="serviceType"> Service registration type </param>
    /// <param name="serviceKey"> Service registration key </param>
    /// <param name="reuse"> Service reuse mode </param>
    public ServiceAttribute(Type? serviceType = null, string? serviceKey = null, ReuseEnum reuse = ReuseEnum.Default)
    {
        ServiceType = serviceType;
        ServiceKey = serviceKey;
        Reuse = reuse;
    }
}
```

`ReuseEnum` options:

```csharp
/// <summary>
/// Reuse mode enumeration
/// </summary>
public enum ReuseEnum
{
    /// <summary>
    /// Default.
    /// </summary>
    Default,

    /// <summary>
    /// Same as scoped but requires <see cref="ThreadScopeContext"/>.
    /// </summary>
    InThread,

    /// <summary>
    /// Scoped to any scope (named or unnamed).
    /// </summary>
    Scoped,

    /// <summary>
    /// Same as <see cref="Scoped"/>, but falls back to <see cref="Singleton"/> when no scope is available.
    /// </summary>
    ScopedOrSingleton,

    /// <summary>
    /// Singleton in container.
    /// </summary>
    Singleton,

    /// <summary>
    /// Transient (not reused).
    /// </summary>
    Transient,
}
```

**Method 2: Implement Service Interfaces**  
Implement `ITransientService`, `ISingletonService`, `IScopedService`, `IScopedOrSingletonService`, or `IInThreadService`:

```csharp
public class HelloService : IHelloService, ITransientService
{
    public void SayHello()
    {
        Console.WriteLine("Hello, World!");
    }
}
```

#### 1.1.2 Registering Services

Register services during module initialization using `HearthApp.Container.RegisterAssemblyAndRefrencedAssembliesTypes(Assembly assembly)`:

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

### 1.2 Dependency Injection

#### 1.2.1 Property Injection

```csharp
// IInjectable - Default injection
// IScopeInjectable - Scoped injection
// INamedScopeInjectable - Named scope injection
internal class SampleButton1 : Button, IInjectable 
{
    [Inject]
    private readonly IHelloService? _helloService;

    public SampleButton1()
    {
        this.InjectServices();
        // Alternative without [Inject]:
        // this.InjectPropertiesAndFields(); 
    }

    protected override void OnClick()
    {
        _helloService?.SayHello();
    }
}
```

##### 1.2.1.1 `InjectAttribute`

```csharp
/// <summary>
/// Auto-injection attribute
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public sealed class InjectAttribute : Attribute
{
    /// <summary>
    /// Service registration key
    /// </summary>
    public object? Key { get; set; }

    /// <summary>
    /// Service type
    /// </summary>
    public Type? ServiceType { get; set; }

    /// <summary>
    /// Injection service attribute
    /// </summary>
    /// <param name="key"> Service registration key </param>
    /// <param name="serviceType"> Service type </param>
    public InjectAttribute(object? key = null, Type? serviceType = null)
    {
        Key = key;
        ServiceType = serviceType;
    }
}
```

#### 1.2.2 Constructor Injection

##### 1.2.2.1 Default Injection

Service registration:

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

Constructor injection:

```csharp
public class HelloTest
{
    private readonly IHelloService _service;
    public HelloTest(IHelloService service)
    {
        _service = service;
    }
}
```

##### 1.2.2.2 Key-based Injection

Service registration:

```csharp
[Service(typeof(IHelloService), "A", ReuseEnum.Transient)]
public class HelloHearthAService : IHelloService
{
    public void SayHello() => Console.WriteLine("Hello, Hearth A!");
}

[Service(typeof(IHelloService), "B", ReuseEnum.Transient)]
public class HelloHearthBService : IHelloService
{
    public void SayHello() => Console.WriteLine("Hello, Hearth B!");
}
```

Constructor injection:

```csharp
public class HelloTest
{
    private readonly IHelloService _aService;
    private readonly IHelloService _bService;
    
    public HelloTest(
        [InjectParam("A")] IHelloService aService, 
        [InjectParam("B")] IHelloService bService)
    {
        _aService = aService;
        _bService = bService;
    }
}
```

#### 1.2.3 ViewModel Locator

Use `ViewModelLocator` in XAML to bind view models. ViewModels can be registered using `[Service]` or auto-registered.

ViewModel:

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

XAML usage:

```xml
<Window ...
    xmlns:ha="clr-namespace:Hearth.ArcGIS;assembly=Hearth.ArcGIS"
    ha:ViewModelLocator.AutoWireViewModel="True">
    <Grid>
        <TextBlock Text="{Binding SampleText}" />
    </Grid>
</Window>
```

### 1.3 Custom Container Initialization

Customize container initialization by implementing `ContainerBuilderBase` and `HearthAppBase`:

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

Initialize before DI usage:

```csharp
CustomHearthApp.Instance.Container.RegisterAssemblyAndRefrencedAssembliesTypes(this.GetType().Assembly);
```

## 2 Using Options Configuration

Reference: [Options Documentation](https://learn.microsoft.com/zh-cn/dotnet/core/extensions/options#options-interfaces)

### 2.1 Create Configuration Class

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

### 2.2 Register Configuration During Module Initialization

```csharp
internal class Module1 : Module
{
    public Module1()
    {
        // samplesettings.json content:
        // "SampleSettings": {
        //     "Value1": "asd",
        //     "Value2": 123,
        //     "Value3": 123.456,
        //     "Value4": [ "asd", "zxc", "qwe" ]
        // }
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("samplesettings.json", true, true)
            .Build();
        
        HearthApp.App.Configure<SampleSettings>(
            configuration.GetSection(typeof(SampleSettings).Name));

        HearthApp.App.RegisterAssemblyAndRefrencedAssembliesTypes(this.GetType().Assembly);
    }
}
```

### 2.3 Implement `IConfigurationProvider<T>` for Auto-Configuration

```csharp
// Automatically registered on initialization
public class SampleSettingsConfigurationProvider : IConfigurationProvider<SampleSettings>
{
    public IConfiguration GetConfiguration()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("samplesettings.json", true, true)
            .Build();
        
        return configuration.GetSection(typeof(SampleSettings).Name);
    }
}
```

## 3 Using NLog Logging

Reference: [NLog Tutorial](https://github.com/NLog/NLog/wiki/Tutorial), [Advanced NLog Configuration](https://github.com/NLog/NLog/wiki/Configuration-file), [NLog Configuration Options](https://nlog-project.org/config/)

### 3.1 Log Configuration

Configuration file location: `...\Pro\bin\nlog.config`  

Example configuration:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      xsi:schemaLocation="http://www.nlog-project.org/schemas/NLog.xsd NLog.xsd"
      autoReload="true"
      throwExceptions="false"
      internalLogLevel="OFF" internalLogFile="c:\\temp\\nlog-internal.log">
	<variable name="logDirectory" value="${basedir}/GeoApp/logs"/>
	<targets>
		<target xsi:type="File" name="f_all" 
                fileName="${logDirectory}/${shortdate}.log" 
                archiveNumbering="Sequence" 
                archiveEvery="Day" 
                maxArchiveDays="30" 
                archiveAboveSize="104857600"
			    layout="[${longdate}] ${threadid} ${level} ${callsite} ${callsite-linenumber} ${message} ${exception}" />
	</targets>
	<rules>
		<logger name="Hearth.*" minlevel="Debug" writeTo="f_all" />
		<logger name="*" minlevel="Warn" writeTo="f_all" />
	</rules>
</nlog>
```

### 3.2 Logger Usage

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

Log output:

```bash
[2025-02-19 15:34:02.0141] 1 Trace Hearth.ArcGIS.Samples.Services.TestLogService.WriteLog 16 Configured Type Logger Class LogTrace 
[2025-02-19 15:34:02.0141] 1 Debug Hearth.ArcGIS.Samples.Services.TestLogService.WriteLog 17 Configured Type Logger Class LogDebug 
[2025-02-19 15:34:02.0141] 1 Info Hearth.ArcGIS.Samples.Services.TestLogService.WriteLog 18 Configured Type Logger Class LogInformation 
[2025-02-19 15:34:02.0141] 1 Warn Hearth.ArcGIS.Samples.Services.TestLogService.WriteLog 19 Configured Type Logger Class LogWarning 
[2025-02-19 15:34:02.0141] 1 Error Hearth.ArcGIS.Samples.Services.TestLogService.WriteLog 20 Configured Type Logger Class LogError 
[2025-02-19 15:34:02.0141] 1 Fatal Hearth.ArcGIS.Samples.Services.TestLogService.WriteLog 21 Configured Type Logger Class LogCritical 
```

## 4 Using AutoMapper

Reference: [AutoMapper Documentation](https://docs.automapper.org/en/stable/)

### 4.1 Model

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
    public Guid Id { get => _id; set => SetProperty(ref _id, value); }
    
    private string _name;
    public string Name { get => _name; set => SetProperty(ref _name, value); }
    
    private int _age;
    public int Age { get => _age; set => SetProperty(ref _age, value); }
    
    private DateTime _birthday;
    public DateTime Birthday { get => _birthday; set => SetProperty(ref _birthday, value); }
}
```

### 4.2 Configuration

**Method 1: Using Profile Configuration**

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

**Method 2: Using [AutoMap] Attribute**

```csharp
[AutoMap(typeof(PersonVO))]
public class Person { /* ... */ }

[AutoMap(typeof(Person))]
public class PersonVO : ViewModelBase { /* ... */ }
```

### 4.3 Usage

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
        Person person = _mapper.Map<Person>(personVO);
        // ...
    }
}
```