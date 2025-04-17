# QueryPage 方法


分页查询实体。



## Definition
**命名空间：** <a href="N_Hearth_ArcGIS_Infrastructure">Hearth.ArcGIS.Infrastructure</a>  
**程序集：** Hearth.ArcGIS.Infrastructure (在 Hearth.ArcGIS.Infrastructure.dll 中) 版本：1.2.0+b41cd873c7cefaa2ff35e931610670fab36c4094

**C#**
``` C#
public virtual Task<PagedResult<TEntity>> QueryPage(
	int pageNumber,
	int pageSize,
	Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryAction = null
)
```



#### 参数
<dl><dt>  Int32</dt><dd>页码</dd><dt>  Int32</dt><dd>页容量</dd><dt>  Func(IQueryable(<a href="T_Hearth_ArcGIS_Infrastructure_RepositoryBase_1">TEntity</a>), IQueryable(<a href="T_Hearth_ArcGIS_Infrastructure_RepositoryBase_1">TEntity</a>))  (Optional)</dt><dd>查询条件</dd></dl>

#### 返回值
Task(<a href="T_Hearth_ArcGIS_Infrastructure_PagedResult_1">PagedResult</a>(<a href="T_Hearth_ArcGIS_Infrastructure_RepositoryBase_1">TEntity</a>))  
查询结果

#### 实现
<a href="M_Hearth_ArcGIS_Infrastructure_IRepositoryBase_1_QueryPage">IRepositoryBase(TEntity).QueryPage(Int32, Int32, Func(IQueryable(TEntity), IQueryable(TEntity)))</a>  


## 参见


#### 引用
<a href="T_Hearth_ArcGIS_Infrastructure_RepositoryBase_1">RepositoryBase(TEntity) 类</a>  
<a href="N_Hearth_ArcGIS_Infrastructure">Hearth.ArcGIS.Infrastructure 命名空间</a>  
