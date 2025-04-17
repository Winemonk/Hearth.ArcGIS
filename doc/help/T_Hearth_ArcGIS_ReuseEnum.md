# ReuseEnum 枚举


重用模式枚举



## Definition
**命名空间：** <a href="N_Hearth_ArcGIS">Hearth.ArcGIS</a>  
**程序集：** Hearth.ArcGIS (在 Hearth.ArcGIS.dll 中) 版本：1.1.0+1226e772e9a2b3048c2f3c2f269f27f8f1fb249e

**C#**
``` C#
public enum ReuseEnum
```



## 成员
<table>
<tr>
<td>Default</td>
<td>0</td>
<td>默认。</td></tr>
<tr>
<td>InThread</td>
<td>1</td>
<td>与 Scoped 作用域相同，但需要 ThreadScopeContext 。</td></tr>
<tr>
<td>Scoped</td>
<td>2</td>
<td>作用域为任何作用域，可以有名称也可以没有名称。</td></tr>
<tr>
<td>ScopedOrSingleton</td>
<td>3</td>
<td>与 Scoped 相同，但在没有可用作用域的情况下，将回退到 Singleton 重用。</td></tr>
<tr>
<td>Singleton</td>
<td>4</td>
<td>容器中单例。</td></tr>
<tr>
<td>Transient</td>
<td>5</td>
<td>瞬态，即不会重复使用。</td></tr>
</table>

## 参见


#### 引用
<a href="N_Hearth_ArcGIS">Hearth.ArcGIS 命名空间</a>  
