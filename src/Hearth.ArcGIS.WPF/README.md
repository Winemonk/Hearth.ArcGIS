# Hearth 框架扩展（DryIoC容器、Options配置、通用日志Nlog...）

## 1 服务注册
### 1.1 标记服务注册特性 — RegisterServiceAttribute

**使用：**

```csharp
public interface ISampleService
{
    void Test();
}

[RegisterService(typeof(ISampleService))]
public class SampleService : ISampleService
{
    public void Test()
    {
        MessageBox.Show("Hello World!", "Hello");
    }
}
```

**说明：**

```csharp
/// <summary>
/// 服务注册特性
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class RegisterServiceAttribute : Attribute
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
    /// 服务注册特性
    /// </summary>
    /// <param name="serviceType"> 服务注册类型 </param>
    /// <param name="serviceKey"> 服务注册键 </param>
    /// <param name="reuse"> 服务重用模式 </param>
    public RegisterServiceAttribute(Type? serviceType, string? serviceKey = null, ReuseEnum reuse = ReuseEnum.Default)
    {
        ServiceType = serviceType;
        ServiceKey = serviceKey;
        Reuse = reuse;
    }
}
```

### 1.2 在模块初始化时调用服务自动注册
```csharp
internal class Module1 : Module, IRegistrable // 需要添加 IRegistrable 接口，无需实现，只做标记
{
    private static Module1 _this = null;

    public static Module1 Current => _this ??= (Module1)FrameworkApplication.FindModule("Winemonk_ArcGIS_Framework_Samples_Module");

    public Module1()
    {
        // 调用服务自动注册
        this.RegisterServices();
    }
}
```

## 2 依赖注入

**使用（IInjectable.InjectServices()）：**
```csharp
internal class SamplePaneViewModel : ViewStatePane, IInjectable // 需要添加 IInjectable 接口，无需实现，只做标记
{
    private const string _viewPaneID = "Winemonk_ArcGIS_Framework_Samples_PlugIns_Panes_SamplePane";

    public SamplePaneViewModel(CIMView view)
      : base(view)
    {
        // 调用注入
        this.InjectServices();
    }

    // 标记自动注入特性
    [InjectService]
    private readonly ISampleService _SampleService;

    public ICommand TestInjectCommand => new RelayCommand(() =>
        {
            _sampleService?.HelloWorld();
        });


    #region Pane Overrides
    public override CIMView ViewState
    {
        get
        {
            _cimView.InstanceID = (int)InstanceID;
            return _cimView;
        }
    }
    #endregion Pane Overrides
}
```

**在自定义类型注册服务中，直接使用构造函数注入即可：**

```csharp
[RegisterService(typeof(TestLogService))]
public class TestLogService
{
    private readonly ILogger<TestLogService> _logger;
    public TestLogService(ILogger<TestLogService> logger)
    {
        _logger = logger;
    }

    public void WriteLog()
    {
        _logger?.LogTrace("Configured Logger Class LogTrace");
        _logger?.LogDebug("Configured Logger Class LogDebug");
        _logger?.LogInformation("Configured Logger Class LogInformation");
        _logger?.LogWarning("Configured Logger Class LogWarning");
        _logger?.LogError("Configured Logger Class LogError");
        _logger?.LogCritical("Configured Logger Class LogCritical");
    }
}
```

**说明：**

```csharp
/// <summary>
/// 自动c特性
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public sealed class InjectServiceAttribute : Attribute
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
    /// 自动注入特性
    /// </summary>
    /// <param name="key"> 服务注册键 </param>
    /// <param name="serviceType"> 服务类型 </param>
    public InjectServiceAttribute(object? key = null, Type? serviceType = null)
    {
        Key = key;
        ServiceType = serviceType;
    }
}
```

## 3 Options配置

### 3.1 创建配置类

```csharp
public class SampleSettings
{
    public string Value1 { get; set; }
    public int Value2 { get; set; }
    public double Value3 { get; set; }
    public string[] Value4 { get; set; }
}
```

### 3.2 在模块初始化时注册配置
```csharp
internal class Module1 : Module
{
    private static Module1 _this = null;

    public static Module1 Current => _this ??= (Module1)FrameworkApplication.FindModule("Winemonk_ArcGIS_Framework_Samples_Module");

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
        IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("SampleSettings.json", false, true).Build();
        GisApp.App().Configure<SampleSettings>(configuration.GetSection(typeof(SampleSettings).Name));
    }
}
```

### 3.3 使用配置

```csharp
internal class SamplePaneViewModel : ViewStatePane, IInjectable // 需要添加 IInjectable 接口，无需实现，只做标记
{
    private const string _viewPaneID = "Winemonk_ArcGIS_Framework_Samples_PlugIns_Panes_SamplePane";

    public SamplePaneViewModel(CIMView view)
      : base(view)
    {
        // 调用注入
        this.InjectServices();
        _optionsMonitor.OnChange(settings =>
        {
            string json = JsonSerializer.Serialize(_optionsMonitor.CurrentValue);
            FrameworkApplication.AddNotification(new Notification
            {
                Title = "SampleSettings Changed!",
                Message = json
            });
        });
    }
    
    // 标记自动注入特性
    [InjectService]
    private readonly IOptionsMonitor<SampleSettings> _optionsMonitor;

    public ICommand TestOptionsCommand  => new RelayCommand(() =>
        {
            string json = JsonSerializer.Serialize(_optionsMonitor.CurrentValue);
            MessageBox.Show(json, "SampleSettings");
        });

    #region Pane Overrides
    public override CIMView ViewState
    {
        get
        {
            _cimView.InstanceID = (int)InstanceID;
            return _cimView;
        }
    }
    #endregion Pane Overrides
}
```

## 4 通用日志（已集成Nlog）

**使用：**

```csharp
using Microsoft.Extensions.Logging;

namespace Hearth.ArcGIS.Samples.Services
{
    [RegisterService(typeof(TestLogService))]
    public class TestLogService
    {
        private readonly ILogger<TestLogService> _logger;
        public TestLogService(ILogger<TestLogService> logger)
        {
            _logger = logger;
        }

        public void WriteLog()
        {
            _logger?.LogTrace("Configured Logger Class LogTrace");
            _logger?.LogDebug("Configured Logger Class LogDebug");
            _logger?.LogInformation("Configured Logger Class LogInformation");
            _logger?.LogWarning("Configured Logger Class LogWarning");
            _logger?.LogError("Configured Logger Class LogError");
            _logger?.LogCritical("Configured Logger Class LogCritical");
        }
    }
}
```

```csharp
internal class SamplePaneViewModel : ViewStatePane, IInjectable // 需要添加 IInjectable 接口，无需实现，只做标记
{
    private const string _viewPaneID = "Winemonk_ArcGIS_Framework_Samples_PlugIns_Panes_SamplePane";

    public SamplePaneViewModel(CIMView view)
      : base(view)
    {
        // 调用注入
        this.InjectServices();
    }
    
    // 标记自动注入特性
    [InjectService]
    private readonly ILogger _logger1;
    [InjectService]
    private readonly ILogger<SamplePaneViewModel> _logger2;
    [InjectService]
    private readonly TestLogService _testLogService;
    
    public ICommand TestLogCommand => new RelayCommand(() =>
        {
            _logger1?.LogTrace("Default Logger LogTrace");
            _logger1?.LogDebug("Default Logger LogDebug");
            _logger1?.LogInformation("Default Logger LogInformation");
            _logger1?.LogWarning("Default Logger LogWarning");
            _logger1?.LogError("Default Logger LogError");
            _logger1?.LogCritical("Default Logger LogCritical");

            _logger2?.LogTrace("Not configured Logger Class LogTrace");
            _logger2?.LogDebug("Not configured Logger Class LogDebug");
            _logger2?.LogInformation("Not configured Logger Class LogInformation");
            _logger2?.LogWarning("Not configured Logger Class LogWarning");
            _logger2?.LogError("Not configured Logger Class LogError");
            _logger2?.LogCritical("Not configured Logger Class LogCritical");

            _testLogService?.WriteLog();
        });

    #region Pane Overrides
    public override CIMView ViewState
    {
        get
        {
            _cimView.InstanceID = (int)InstanceID;
            return _cimView;
        }
    }
    #endregion Pane Overrides
}
```

**日志配置（.\Pro\bin\nlog.config）：**

```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      xsi:schemaLocation="http://www.nlog-project.org/schemas/NLog.xsd NLog.xsd"
      autoReload="true"
      throwExceptions="false"
      internalLogLevel="OFF" internalLogFile="c:\temp\nlog-internal.log">
	<targets>
		<target xsi:type="File" name="f_all" fileName="${basedir}/GISAPP/logs/all-${shortdate}.log"
				archiveNumbering="Sequence" archiveEvery="Day" maxArchiveDays="30" archiveAboveSize="104857600"
				layout="[${longdate}] ${threadid} ${callsite} ${callsite-linenumber} ${message} ${exception}" />
		<target xsi:type="File" name="f_default" fileName="${basedir}/GISAPP/logs/default-${shortdate}.log"
				archiveNumbering="Sequence" archiveEvery="Day" maxArchiveDays="30" archiveAboveSize="104857600"
				layout="[${longdate}] ${threadid} ${callsite} ${callsite-linenumber} ${message} ${exception}" />
		<target xsi:type="File" name="f_test" fileName="${basedir}/GISAPP/logs/test-${shortdate}.log"
				archiveNumbering="Sequence" archiveEvery="Day" maxArchiveDays="30" archiveAboveSize="104857600"
				layout="[${longdate}] ${threadid} ${callsite} ${callsite-linenumber} ${message} ${exception}" />
	</targets>
	<rules>
		<logger name="*" minlevel="Trace" writeTo="f_all" />
		<logger name="." minlevel="Trace" writeTo="f_default" />
		<logger name="Hearth.Samples.Services.TestLogService" minlevel="Trace" writeTo="f_test" />
	</rules>
</nlog>
```

**日志：**

all-2025-02-11.log
```log
[2025-02-11 16:28:33.2379] 1 Hearth.Samples.PlugIns.Panes.SamplePaneViewModel.get_TestLogCommand 47 Default Logger LogTrace 
[2025-02-11 16:28:33.3172] 1 Hearth.Samples.PlugIns.Panes.SamplePaneViewModel.get_TestLogCommand 48 Default Logger LogDebug 
[2025-02-11 16:28:33.3231] 1 Hearth.Samples.PlugIns.Panes.SamplePaneViewModel.get_TestLogCommand 49 Default Logger LogInformation 
[2025-02-11 16:28:33.3231] 1 Hearth.Samples.PlugIns.Panes.SamplePaneViewModel.get_TestLogCommand 50 Default Logger LogWarning 
[2025-02-11 16:28:33.3231] 1 Hearth.Samples.PlugIns.Panes.SamplePaneViewModel.get_TestLogCommand 51 Default Logger LogError 
[2025-02-11 16:28:33.3231] 1 Hearth.Samples.PlugIns.Panes.SamplePaneViewModel.get_TestLogCommand 52 Default Logger LogCritical 
[2025-02-11 16:28:33.3231] 1 Hearth.Samples.PlugIns.Panes.SamplePaneViewModel.get_TestLogCommand 54 Not configured Logger Class LogTrace 
[2025-02-11 16:28:33.3231] 1 Hearth.Samples.PlugIns.Panes.SamplePaneViewModel.get_TestLogCommand 55 Not configured Logger Class LogDebug 
[2025-02-11 16:28:33.3231] 1 Hearth.Samples.PlugIns.Panes.SamplePaneViewModel.get_TestLogCommand 56 Not configured Logger Class LogInformation 
[2025-02-11 16:28:33.3231] 1 Hearth.Samples.PlugIns.Panes.SamplePaneViewModel.get_TestLogCommand 57 Not configured Logger Class LogWarning 
[2025-02-11 16:28:33.3231] 1 Hearth.Samples.PlugIns.Panes.SamplePaneViewModel.get_TestLogCommand 58 Not configured Logger Class LogError 
[2025-02-11 16:28:33.3231] 1 Hearth.Samples.PlugIns.Panes.SamplePaneViewModel.get_TestLogCommand 59 Not configured Logger Class LogCritical 
[2025-02-11 16:28:33.3231] 1 Hearth.Samples.Services.TestLogService.WriteLog 21 Configured Logger Class LogTrace 
[2025-02-11 16:28:33.3231] 1 Hearth.Samples.Services.TestLogService.WriteLog 22 Configured Logger Class LogDebug 
[2025-02-11 16:28:33.3231] 1 Hearth.Samples.Services.TestLogService.WriteLog 23 Configured Logger Class LogInformation 
[2025-02-11 16:28:33.3231] 1 Hearth.Samples.Services.TestLogService.WriteLog 24 Configured Logger Class LogWarning 
[2025-02-11 16:28:33.3231] 1 Hearth.Samples.Services.TestLogService.WriteLog 25 Configured Logger Class LogError 
[2025-02-11 16:28:33.3231] 1 Hearth.Samples.Services.TestLogService.WriteLog 26 Configured Logger Class LogCritical 
```

default-2025-02-11.log
```log
[2025-02-11 16:28:33.2379] 1 Hearth.Samples.PlugIns.Panes.SamplePaneViewModel.get_TestLogCommand 47 Default Logger LogTrace 
[2025-02-11 16:28:33.3172] 1 Hearth.Samples.PlugIns.Panes.SamplePaneViewModel.get_TestLogCommand 48 Default Logger LogDebug 
[2025-02-11 16:28:33.3231] 1 Hearth.Samples.PlugIns.Panes.SamplePaneViewModel.get_TestLogCommand 49 Default Logger LogInformation 
[2025-02-11 16:28:33.3231] 1 Hearth.Samples.PlugIns.Panes.SamplePaneViewModel.get_TestLogCommand 50 Default Logger LogWarning 
[2025-02-11 16:28:33.3231] 1 Hearth.Samples.PlugIns.Panes.SamplePaneViewModel.get_TestLogCommand 51 Default Logger LogError 
[2025-02-11 16:28:33.3231] 1 Hearth.Samples.PlugIns.Panes.SamplePaneViewModel.get_TestLogCommand 52 Default Logger LogCritical 
```

test-2025-02-11.log
```log
[2025-02-11 16:28:33.3231] 1 Hearth.Samples.Services.TestLogService.WriteLog 21 Configured Logger Class LogTrace 
[2025-02-11 16:28:33.3231] 1 Hearth.Samples.Services.TestLogService.WriteLog 22 Configured Logger Class LogDebug 
[2025-02-11 16:28:33.3231] 1 Hearth.Samples.Services.TestLogService.WriteLog 23 Configured Logger Class LogInformation 
[2025-02-11 16:28:33.3231] 1 Hearth.Samples.Services.TestLogService.WriteLog 24 Configured Logger Class LogWarning 
[2025-02-11 16:28:33.3231] 1 Hearth.Samples.Services.TestLogService.WriteLog 25 Configured Logger Class LogError 
[2025-02-11 16:28:33.3231] 1 Hearth.Samples.Services.TestLogService.WriteLog 26 Configured Logger Class LogCritical 
```