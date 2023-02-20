using Microsoft.EntityFrameworkCore;
using Teta.Packages.UoW.EfCore.Interfaces.BusinessEntity;

namespace Teta.Packages.UoW.EfCore.Business
{
    /// <summary>
    /// Фабрика управляющая сохранием репозитория, инкапсулирует логику работы с DI контейнером
    /// </summary>
    public class GenericRepositoryFactory<TContext> : IGenericRepositoryFactory<TContext>
        where TContext : DbContext
    {
        private readonly TContext _dbContext;
        public GenericRepositoryFactory(TContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IGenericRepository<T, TKey, TContext> Create<T, TKey>()
            where TKey : struct
            where T : class, IBusinessEntity<TKey>, new()
        {
            return new GenericRepository<T, TKey, TContext>(_dbContext);
        }
    }
}