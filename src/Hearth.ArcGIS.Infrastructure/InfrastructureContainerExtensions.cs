using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Hearth.ArcGIS.Infrastructure
{
    /// <summary>
    /// <see cref="IContainer"/> 接口的扩展方法。
    /// </summary>
    public static class InfrastructureContainerExtensions
    {
        /// <summary>
        /// 注册 <see cref="DbContext"/> 到 <see cref="IContainer"/> 容器。
        /// </summary>
        /// <typeparam name="TContext"><see cref="DbContext"/> 类型</typeparam>
        /// <param name="container"><see cref="IContainer"/> 容器</param>
        /// <param name="optionsAction">
        ///     <para>
        ///         为上下文配置 <see cref="DbContextOptions" /> 的可选操作。
        ///         这提供了一种在派生上下文中重写 <see cref="DbContext.OnConfiguring" /> 方法来执行上下文配置的替代方法。
        ///     </para>
        ///     <para>
        ///         如果此处提供了一个操作，则如果在派生上下文上重写了 <see cref="DbContext.OnConfiguring" /> 方法，
        ///         则该方法仍将运行 <see cref="DbContext.OnConfiguring" /> 除了在此处执行的配置外，还将应用配置。
        ///     </para>
        ///     <para>
        ///         为了将选项传递到上下文中，您需要在上下文中公开一个构造函数，该构造函数接受 
        ///         <see cref="DbContextOptions{TContext}" /> 并将其传递给 <see cref="DbContext" /> 的基构造函数。
        ///     </para>
        /// </param>
        /// <param name="contextLifetime">在容器中注册 DbContext 服务的生存期。</param>
        /// <param name="optionsLifetime">在容器中注册 DbContextOptions 上下文选项服务的生存期。</param>
        /// <returns>相同的容器，以便可以链接多个调用。</returns>
        public static IContainer AddDbContext<TContext>(this IContainer container,
            Action<DbContextOptionsBuilder>? optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped) where TContext : DbContext
        {
            ServiceCollection sc = new ServiceCollection();
            sc.AddDbContext<TContext>(optionsAction, contextLifetime, optionsLifetime);
            container.Populate(sc, (r, sd) => r.IsRegistered(sd.ServiceType));
            return container;
        }

        /// <summary>
        /// 注册 <see cref="DbContext"/> 到 <see cref="IContainer"/> 容器。
        /// </summary>
        /// <param name="container"><see cref="IContainer"/> 容器</param>
        /// <param name="dbContextType"><see cref="DbContext"/> 类型</param>
        /// <param name="optionsAction">
        ///     <para>
        ///         为上下文配置 <see cref="DbContextOptions" /> 的可选操作。
        ///         这提供了一种在派生上下文中重写 <see cref="DbContext.OnConfiguring" /> 方法来执行上下文配置的替代方法。
        ///     </para>
        ///     <para>
        ///         如果此处提供了一个操作，则如果在派生上下文上重写了 <see cref="DbContext.OnConfiguring" /> 方法，
        ///         则该方法仍将运行 <see cref="DbContext.OnConfiguring" /> 除了在此处执行的配置外，还将应用配置。
        ///     </para>
        ///     <para>
        ///         为了将选项传递到上下文中，您需要在上下文中公开一个构造函数，该构造函数接受 
        ///         <see cref="DbContextOptions{TContext}" /> 并将其传递给 <see cref="DbContext" /> 的基构造函数。
        ///     </para>
        /// </param>
        /// <param name="contextLifetime">在容器中注册 DbContext 服务的生存期。</param>
        /// <param name="optionsLifetime">在容器中注册 DbContextOptions 上下文选项服务的生存期。</param>
        /// <returns>相同的容器，以便可以链接多个调用。</returns>
        /// <exception cref="ArgumentException"> <paramref name="dbContextType" /> 类型不是 <see cref="DbContext" /> 类型。</exception>
        public static IContainer AddDbContext(this IContainer container,
            Type dbContextType,
            Action<DbContextOptionsBuilder>? optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        {
            ArgumentNullException.ThrowIfNull(dbContextType);
            if (!typeof(DbContext).IsAssignableFrom(dbContextType))
                throw new ArgumentException("类型必须是：DbContext", nameof(dbContextType));

            MethodInfo[] addDbContextMethods = typeof(EntityFrameworkServiceCollectionExtensions)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name == nameof(EntityFrameworkServiceCollectionExtensions.AddDbContext) && m.IsGenericMethod)
                .ToArray();
            MethodInfo? addDbContextMethod = addDbContextMethods.SingleOrDefault(m =>
            {
                ParameterInfo[] parameters = m.GetParameters();
                if (parameters.Length != 4)
                    return false;
                if (!m.ContainsGenericParameters || m.GetGenericArguments().Length != 1)
                    return false;
                return parameters[0].ParameterType == typeof(IServiceCollection)
                    && parameters[1].ParameterType == typeof(Action<DbContextOptionsBuilder>)
                    && parameters[2].ParameterType == typeof(ServiceLifetime)
                    && parameters[3].ParameterType == typeof(ServiceLifetime);
            });
            if (addDbContextMethod != null)
            {
                ServiceCollection sc = new ServiceCollection();
                MethodInfo methodInfo = addDbContextMethod.MakeGenericMethod(dbContextType);
                methodInfo.Invoke(null, new object?[] { sc, null, null, null });
                container.Populate(sc, (r, sd) => r.IsRegistered(sd.ServiceType));
            }
            return container;
        }
    }
}
