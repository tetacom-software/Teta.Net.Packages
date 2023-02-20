using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace Teta.Packages.UoW.EfCore.Interfaces.BusinessEntity
{
    /// <summary>
    /// Интерфейс UnitOfWork
    /// </summary>
    public interface IUnitOfWork<TContext> : IDisposable
            where TContext : DbContext
    {
        /// <summary>
        /// Ссылка на DbContext
        /// </summary>
        /// <returns>Экземпляр контекста<typeparamref name="TContext"/>.</returns>
        TContext CommonContext { get; }

        /// <summary>
        /// Получить репозиторий объектов <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">Бизнесс-Сущность.</typeparam>
        /// <typeparam name="TKey">Тип ключа сущности СУБД</typeparam>
        /// <typeparam name="TContext">Тип контекста</typeparam>
        /// <returns>Зкземпляр типа <see cref="IGenericRepository{TEntity, TKey, TContext}"/> </returns>
        IGenericRepository<TEntity, TKey, TContext> GetRepository<TEntity, TKey>()
            where TKey : struct
            where TEntity : class, IBusinessEntity<TKey>, new();

        /// <summary>
        /// Текущая транзакция
        /// </summary>
        public IDbContextTransaction CurrentTransaction { get; }

        /// <summary>
        /// Выполняет сохранение всех изменений из контекста в СУБД
        /// </summary>
        /// <returns>Число сущностей сохраненный в СУБД</returns>
        int SaveChanges();

        /// <summary>
        /// Асинхронная операция сохранения всех измененных сущностей из контекста
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous save operation. The task result contains the number of state entities written to database.</returns>
        Task SaveChangesAsync();

        /// <summary>
        /// Выполняет команду на прямую в СУБД
        /// </summary>
        /// <param name="sql">SQL запрос</param>
        /// <param name="parameters">Параметры запроса</param>
        /// <returns>Число сущенстей из СУБД</returns>
        int ExecuteSqlCommand(string sql, params object[] parameters);

        /// <summary>
        /// Выполнить SQL запрос
        /// </summary>
        /// <param name="sql">Запрос</param>
        /// <param name="parameters">Параметры запроса</param>
        /// <returns></returns>
        Task<int> ExecuteRawSqlAsync(string sql, params object[] parameters);

        /// <summary>
        /// Получить объекты используя прямой запрос в СУБД
        /// </summary>
        /// <typeparam name="TEntity">Тип сущности</typeparam>
        /// <param name="sql">SQL запрос в СУБД</param>
        /// <param name="parameters">Список параметров запроса</param>
        /// <returns>An <see cref="IQueryable{T}"/> Лейзи-коллекция объектов выбранных запросом</returns>
        IQueryable<TEntity> FromSql<TEntity>(string sql, params object[] parameters)
            where TEntity : class;

        /// <summary>
        /// Uses TrakGrap Api to attach disconnected entities
        /// </summary>
        /// <param name="rootEntity"> Root entity</param>
        /// <param name="callback">Delegate to convert Object's State properities to Entities entry state.</param>
        void TrackGraph(object rootEntity, Action<EntityEntryGraphNode> callback);

        /// <summary>
        /// Callback вызываемый после выполнения сохранения данных в случае если оно произошло без ошибок
        /// </summary>
        /// <param name="callback">Callback</param>
        IUnitOfWork<TContext> SaveCallback(Func<Task> callback);

        /// <summary>
        /// Callback вызываемый после выполнения сохранения данных в случае если оно произошло без ошибок
        /// </summary>
        /// <param name="callback">Callback</param>
        IUnitOfWork<TContext> OnErrorCallback(Func<Exception, Task> callback);

        /// <summary>
        /// Create save point
        /// </summary>
        /// <param name="savePointName">Savepoint name</param>
        /// <param name="transaction">transaction</param>
        /// <returns>Async operation</returns>
        Task CreateSavePointAsync(string savePointName, IDbContextTransaction transaction = null);

        /// <summary>
        /// Выполнить действие внутри транзакции
        /// </summary>
        /// <typeparam name="T">Возвращаемый тип</typeparam>
        /// <param name="callback">Каллбек</param>
        /// <returns>Результат асинхронно</returns>
        Task<T> ExecuteInsideTransaction<T>(Func<Task<T>> callback);

        /// <summary>
        /// Выполнить void метод внутри транзакции
        /// </summary>
        /// <param name="callback">Делегат</param>
        /// <returns>Асинхронная операция</returns>
        Task ExecuteInsideTransaction(Func<Task> callback);

        /// <summary>
        /// Открыть транзакцию
        /// </summary>
        IDbContextTransaction BeginDbTransaction();

        /// <summary>
        /// Использовать существующую транзакцию
        /// </summary>
        /// <param name="dbTransaction">transaction</param>
        void UseTransaction(IDbContextTransaction dbTransaction);
    }
}