using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Teta.Packages.UoW.EfCore.Interfaces.BusinessEntity;

namespace Teta.Packages.UoW.EfCore.Business;

/// <summary>
/// Extensions for unit of work
/// </summary>
public static class TetaUoWExtensions
{
    /// <summary>
    /// Выполнить регистрацию UnitOfWork
    /// </summary>
    /// <param name="services">Сервисы DI</param>
    /// <param name="configureDbContextPool">Делегат конфигурации пула подключения</param>
    /// <returns>Сервисы DI</returns>
    public static IServiceCollection RegisterUnitOfWork<TContext, TContextImplementation>(
        this IServiceCollection services, 
        Action<DbContextOptionsBuilder> configureDbContextPool)
        where TContext : class
        where TContextImplementation : DbContext, TContext
    {
        services.TryAddScoped<IGenericRepositoryFactory<TContextImplementation>, GenericRepositoryFactory<TContextImplementation>>();
        services.TryAddScoped<IUnitOfWork<TContextImplementation>, UnitOfWork<TContextImplementation>>();

        services.AddDbContextPool<TContext, TContextImplementation>(configureDbContextPool);
        services.TryAddScoped<IUnitOfWork<TContextImplementation>, UnitOfWork<TContextImplementation>>();

        return services;
    }
}