using Microsoft.EntityFrameworkCore;

namespace Teta.Packages.UoW.EfCore.Interfaces.BusinessEntity
{
    /// <summary>
    /// Фабрика интерфейсов репозитория
    /// </summary>
    public interface IGenericRepositoryFactory<TContext>
        where TContext : DbContext
    {
        /// <summary>
        /// Выполняет получение нового репозитория объектов
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <typeparam name="TKey">Тип ключа объекта в СУБД</typeparam>
        /// <typeparam name="TContext">Тип контекста</typeparam>
        /// <returns>Репозиторий для работы с типом объекта</returns>
        IGenericRepository<T, TKey, TContext> Create<T, TKey>()
            where TKey : struct
            where T : class, IBusinessEntity<TKey>;
    }
}