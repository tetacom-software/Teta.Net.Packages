using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Teta.Packages.UoW.EfCore.Interfaces.BusinessEntity
{
    /// <summary>
    /// Интерфейс репозитория с базовыми методами
    /// </summary>
    public interface IGenericRepository<T, TKey>
        where TKey : struct
        where T : class, IBusinessEntity<TKey>, new()
    {
        /// <summary>
        /// Выполняет сохранение всех изменений в СУБД
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// Асинхронное сохранение всех изменений в СУБД
        /// </summary>
        Task SaveChangesAsync();

        /// <summary>
        /// Получить первое значение соответствующие предикату
        /// </summary>
        /// <param name="predicate">Предикат</param>
        /// <returns>Экземпляр типа</returns>
        T First(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Получить первое значение по предикату или значение по умолчанию
        /// </summary>
        /// <param name="predicate">Предикат</param>
        /// <returns>Экземпляр типа</returns>
        T? FirstOrDefault([AllowNull]Expression<Func<T, bool>>? predicate = null);

        /// <summary>
        /// Получить первое значние асинхронно
        /// </summary>
        /// <param name="predicate">Предикат</param>
        /// <returns>Экземпляр типа</returns>
        Task<T?> FirstOrDefaultAsync([AllowNull]Expression<Func<T, bool>>? predicate = null);

        /// <summary>
        /// Получить все доступные значения
        /// </summary>
        /// <returns>IQueryable queries</returns>
        IQueryable<T> GetAll();

        /// <summary>
        /// Получить все доступные значения в виде асинхронного перечисления
        /// </summary>
        /// <returns>Асинхронное перечисление</returns>
        IAsyncEnumerable<T> AsAsyncEnumerable();

        /// <summary>
        /// Найти данные по условию в предикате (where)
        /// </summary>
        /// <param name="predicate">Предикат для поиска</param>
        /// <returns>Лейзи коллекция для условия</returns>
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Проверить наличие объекта по предикату поиска
        /// </summary>
        /// <param name="predicate">Предикат поиска</param>
        /// <returns>true если объект существует</returns>
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Найти сущности по ключам
        /// </summary>
        /// <param name="keys">Список ключей поиска</param>
        /// <returns>Экземпляр сущности</returns>
        Task<T> FindAsync(params TKey[] keys);

        /// <summary>
        /// Получить сущность по айди
        /// </summary>
        /// <param name="key">Айди</param>
        /// <returns></returns>
        Task<T> GetByIdAsync(TKey key);

        /// <summary>
        /// Добавить новую сущность
        /// </summary>
        /// <param name="entity">Сущность</param>
        ValueTask<EntityEntry<T>> AddAsync(T entity);

        /// <summary>
        /// Добавить набор сущностей
        /// </summary>
        /// <param name="entities">Коллекция сущностей</param>
        void AddRange(IEnumerable<T> entities);

        /// <summary>
        /// Remove entity from database
        /// </summary>
        /// <param name="entity">Entity object</param>
        void Delete(T entity);

        /// <summary>
        /// Удаляет сущность по айди
        /// </summary>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Асинхронная операция</returns>
        Task DeleteByIdAsync(TKey id);

        /// <summary>
        /// Remove entities from database
        /// </summary>
        /// <param name="entity">Entity object</param>
        Task DeleteRangeAsync(IEnumerable<TKey> entity);

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity object</param>
        void Update(T entity);

        /// <summary>
        /// Update or insert new entity
        /// </summary>
        /// <param name="entity">Entity object</param>
        Task<T> Upsert(T entity);

        /// <summary>
        /// Order by
        /// </summary>
        IOrderedQueryable<T> OrderBy<K>(Expression<Func<T, K>> predicate);

        /// <summary>
        /// Order by
        /// </summary>
        IQueryable<IGrouping<K, T>> GroupBy<K>(Expression<Func<T, K>> predicate);

        /// <summary>
        /// Remove range of given entities
        /// </summary>
        void RemoveRange(IEnumerable<T> entities);

        /// <summary>
        /// Update range of given entities
        /// </summary>
        /// <param name="entities"></param>
        void UpdateRange(IEnumerable<T> entities);

        /// <summary>
        /// Execute Sql query
        /// </summary>
        /// <param name="query">Query as string</param>
        /// <param name="sqlParam">Object include parameter</param>
        /// <returns></returns>
        IQueryable<T> Execute(string query, DbParameter sqlParam);

        /// <summary>
        /// Execute Sql query
        /// </summary>
        /// <param name="query">Query as string</param>
        /// <returns></returns>
        IQueryable<T> ExecuteString(string query);

        /// <summary>
        /// Удалить набор
        /// </summary>
        /// <param name="entities">Набор обсадных</param>
        void DeleteRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Обновить набор данных
        /// </summary>
        /// <param name="entities"></param>
        Task BulkInsertOrUpdateAsync(IList<T> entities);

        /// <summary>
        /// Bulk insert
        /// </summary>
        /// <param name="entities">Entites</param>
        /// <returns>Async operation</returns>
        Task BulkInsertAsync(IList<T> entities);

        /// <summary>
        /// Удаление сущностей пачкой
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <returns>Асинхронная операция</returns>
        Task BulkDeleteAsync(IList<T> entities);

        /// <summary>
        /// Bulk update entites by PK
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <returns>Async operation</returns>
        Task BulkUpdateAsync(IList<T> entities);
    }
}
