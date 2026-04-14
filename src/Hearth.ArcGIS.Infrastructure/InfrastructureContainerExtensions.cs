using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Hearth.ArcGIS.Infrastructure
{
    /// <summary>
    /// 基础设施容器扩展
    /// </summary>
    public static class InfrastructureContainerExtensions
    {
        /// <summary>
        /// 注册数据库上下文，<see cref="EntityFrameworkServiceCollectionExtensions.AddDbContext{TContext}(IServiceCollection, Action{IServiceProvider, DbContextOptionsBuilder}?, ServiceLifetime, ServiceLifetime)"/>
        /// </summary>
        /// <typeparam name="TContext">数据库上下文类型</typeparam>
        /// <param name="container">容器实例</param>
        /// <param name="optionsAction">配置项方法</param>
        /// <param name="contextLifetime">上下文生命周期</param>
        /// <param name="optionsLifetime">配置项生命周期</param>
        /// <returns>容器实例</returns>
        public static Container AddDbContext<TContext>(this Container container,
                                                       Action<DbContextOptionsBuilder>? optionsAction = null,
                                                       ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
                                                       ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
            where TContext : DbContext
            => AddDbContext<TContext, TContext>(container, optionsAction, contextLifetime, optionsLifetime);

        /// <summary>
        /// 注册数据库上下文，<see cref="EntityFrameworkServiceCollectionExtensions.AddDbContext{TContextService, TContextImplementation}(IServiceCollection, Action{DbContextOptionsBuilder}?, ServiceLifetime, ServiceLifetime)"/>
        /// </summary>
        /// <typeparam name="TContextService">数据库上下文类型</typeparam>
        /// <typeparam name="TContextImplementation">数据库上下文实现类型</typeparam>
        /// <param name="container">容器实例</param>
        /// <param name="optionsAction">配置项方法</param>
        /// <param name="contextLifetime">上下文生命周期</param>
        /// <param name="optionsLifetime">配置项生命周期</param>
        /// <returns>容器实例</returns>
        public static Container AddDbContext<TContextService, TContextImplementation>(this Container container,
                                                                                      Action<DbContextOptionsBuilder>? optionsAction = null,
                                                                                      ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
                                                                                      ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
            where TContextImplementation : DbContext, TContextService
        {
            ServiceCollection services = new ServiceCollection();
            services.AddDbContext<TContextService, TContextImplementation>(op =>
                                                                          {
                                                                              optionsAction?.Invoke(op);
#if DEBUG
                                                                              op.EnableSensitiveDataLogging()
                                                                                .LogTo(msg =>
                                                                                {
                                                                                    string line = new('=', 100);
                                                                                    string sp = $"\n{line} EFCore {line}\n";
                                                                                    Debug.WriteLine($"{sp}{msg}{sp}");
                                                                                }, LogLevel.Information);
#endif
                                                                          },
                                                                          contextLifetime,
                                                                          optionsLifetime);
            container.Populate(services, (r, sd) => r.IsRegistered(sd.ServiceType));
            return container;
        }
    }
}
