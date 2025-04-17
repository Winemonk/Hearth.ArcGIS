# IScopeInjectable 接口


作用域注入接口



## Definition
**命名空间：** <a href="N_Hearth_ArcGIS">Hearth.ArcGIS</a>  
**程序集：** Hearth.ArcGIS (在 Hearth.ArcGIS.dll 中) 版本：1.1.0+1226e772e9a2b3048c2f3c2f269f27f8f1fb249e

**C#**
``` C#
public interface IScopeInjectable : IInjectable
```

<table><tr><td><strong>Implements</strong></td><td><a href="T_Hearth_ArcGIS_IInjectable">IInjectable</a></td></tr>
</table>



## 属性
<table>
<tr>
<td><a href="P_Hearth_ArcGIS_IScopeInjectable_Scope">Scope</a></td>
<td>作用域</td></tr>
</table>

## 扩展方法
<table>
<tr>
<td><a href="M_Hearth_ArcGIS_InjectableExtensions_InjectPropertiesAndFields">InjectPropertiesAndFields</a></td>
<td>解析并注入 <em>injectable</em> 实例内的所有字段和属性。<br />(由 <a href="T_Hearth_ArcGIS_InjectableExtensions">InjectableExtensions</a> 定义。)</td></tr>
<tr>
<td><a href="M_Hearth_ArcGIS_InjectableExtensions_InjectServices">InjectServices</a></td>
<td>解析并注入 <em>injectable</em> 实例内的 <a href="T_Hearth_ArcGIS_InjectAttribute">InjectAttribute</a> 标注的所有字段。<br />(由 <a href="T_Hearth_ArcGIS_InjectableExtensions">InjectableExtensions</a> 定义。)</td></tr>
</table>

## 参见


#### 引用
<a href="N_Hearth_ArcGIS">Hearth.ArcGIS 命名空间</a>  
