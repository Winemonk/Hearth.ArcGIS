# Query 方法


查询实体。



## Definition
**命名空间：** <a href="N_Hearth_ArcGIS_Infrastructure">Hearth.ArcGIS.Infrastructure</a>  
**程序集：** Hearth.ArcGIS.Infrastructure (在 Hearth.ArcGIS.Infrastructure.dll 中) 版本：1.2.0+b41cd873c7cefaa2ff35e931610670fab36c4094

**C#**
``` C#
Task<IReadOnlyList<TEntity>> Query(
	Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryAction = null
)
```



#### 参数
<dl><dt>  Func(IQueryable(<a href="T_Hearth_ArcGIS_Infrastructure_IRepositoryBase_1">TEntity</a>), IQueryable(<a href="T_Hearth_ArcGIS_Infrastructure_IRepositoryBase_1">TEntity</a>))  (Optional)</dt><dd>查询条件</dd></dl>

#### 返回值
Task(IReadOnlyList(<a href="T_Hearth_ArcGIS_Infrastructure_IRepositoryBase_1">TEntity</a>))  
查询结果

## 参见


#### 引用
<a href="T_Hearth_ArcGIS_Infrastructure_IRepositoryBase_1">IRepositoryBase(TEntity) 接口</a>  
<a href="N_Hearth_ArcGIS_Infrastructure">Hearth.ArcGIS.Infrastructure 命名空间</a>  
