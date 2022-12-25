using Teta.Packages.UoW.EfCore.Interfaces.BusinessEntity;

namespace Teta.Packages.UoW.EfCore.Business
{
    /// <summary>
    /// Фабрика управляющая сохранием репозитория, инкапсулирует логику работы с DI контейнером
    /// </summary>
    public class GenericRepositoryFactory : IGenericRepositoryFactory
    {
        private readonly ICommonDbContext _dbContext;

        public GenericRepositoryFactory(ICommonDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IGenericRepository<T, TKey> Create<T, TKey>()
            where TKey : struct
            where T : class, IBusinessEntity<TKey>, new()
        {
            return new GenericRepository<T, TKey>(_dbContext);
        }
    }
}
