# InsertRange 方法


批量插入实体。



## Definition
**命名空间：** <a href="N_Hearth_ArcGIS_Infrastructure">Hearth.ArcGIS.Infrastructure</a>  
**程序集：** Hearth.ArcGIS.Infrastructure (在 Hearth.ArcGIS.Infrastructure.dll 中) 版本：1.2.0+b41cd873c7cefaa2ff35e931610670fab36c4094

**C#**
``` C#
public virtual Task<IReadOnlyList<TEntity>> InsertRange(
	IList<TEntity> entities
)
```



#### 参数
<dl><dt>  IList(<a href="T_Hearth_ArcGIS_Infrastructure_RepositoryBase_1">TEntity</a>)</dt><dd>实体集合</dd></dl>

#### 返回值
Task(IReadOnlyList(<a href="T_Hearth_ArcGIS_Infrastructure_RepositoryBase_1">TEntity</a>))  
插入成功的实体集合

#### 实现
<a href="M_Hearth_ArcGIS_Infrastructure_IRepositoryBase_1_InsertRange">IRepositoryBase(TEntity).InsertRange(IList(TEntity))</a>  


## 参见


#### 引用
<a href="T_Hearth_ArcGIS_Infrastructure_RepositoryBase_1">RepositoryBase(TEntity) 类</a>  
<a href="N_Hearth_ArcGIS_Infrastructure">Hearth.ArcGIS.Infrastructure 命名空间</a>  
