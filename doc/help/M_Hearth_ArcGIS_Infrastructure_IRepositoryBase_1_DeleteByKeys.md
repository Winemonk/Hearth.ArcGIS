# DeleteByKeys 方法


删除指定主键的实体。



## Definition
**命名空间：** <a href="N_Hearth_ArcGIS_Infrastructure">Hearth.ArcGIS.Infrastructure</a>  
**程序集：** Hearth.ArcGIS.Infrastructure (在 Hearth.ArcGIS.Infrastructure.dll 中) 版本：1.2.0+b41cd873c7cefaa2ff35e931610670fab36c4094

**C#**
``` C#
Task<bool> DeleteByKeys(
	params Object[] keys
)
```



#### 参数
<dl><dt>  Object[]</dt><dd>主键值集合</dd></dl>

#### 返回值
Task(Boolean)  
是否删除成功

## 参见


#### 引用
<a href="T_Hearth_ArcGIS_Infrastructure_IRepositoryBase_1">IRepositoryBase(TEntity) 接口</a>  
<a href="N_Hearth_ArcGIS_Infrastructure">Hearth.ArcGIS.Infrastructure 命名空间</a>  
