using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Teta.Packages.UoW.EfCore.Interfaces.BusinessEntity;

namespace Teta.Packages.UoW.EfCore.Business
{
    /// <summary>
    /// Реализация UnitOfWork <see cref="IUnitOfWorkBase"/> and <see cref="IUnitOfWork{TContext}"/> interface.
    /// </summary>
    /// <typeparam name="TContext">Тип контекста приложения</typeparam>
    public class UnitOfWork<TContext> : IUnitOfWork<TContext>, IUnitOfWorkBase
        where TContext : DbContext
    {
        private bool _disposed;
        private readonly IGenericRepositoryFactory _repositoryFactory;
        private readonly ILogger<IUnitOfWorkBase>? _logger;
        private readonly ConcurrentBag<Func<Task>> _aftersaveCallback;
        private readonly ConcurrentBag<Func<Exception, Task>> _errorsCallback;

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessBusinessUnitOfWork{TContext}"/> class.
        /// </summary>
        /// <param name="context">Контекст СУБД приложения</param>
        /// <param name="repositoryFactory">Фабрика репозиториев</param>
        /// <param name="logger">Logger</param>
        public UnitOfWork(
            [NotNull]TContext context, 
            [NotNull]IGenericRepositoryFactory repositoryFactory, 
            [NotNull]ILogger<IUnitOfWorkBase> logger)
        {
            CommonContext = context ?? throw new ArgumentNullException(nameof(context));
            _aftersaveCallback = new ConcurrentBag<Func<Task>>();
            _errorsCallback = new ConcurrentBag<Func<Exception, Task>>();
            _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public TContext CommonContext { get; }

        /// <summary>
        /// Текущая транзакция
        /// </summary>
        public IDbContextTransaction CurrentTransaction => CommonContext.Database.CurrentTransaction;


        /// <inheritdoc/>
        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>()
            where TKey : struct
            where TEntity : class, IBusinessEntity<TKey>, new()
        {
            return _repositoryFactory.Create<TEntity, TKey>();
        }

        /// <inheritdoc/>
        public int ExecuteSqlCommand(string sql, params object[] parameters) =>
            CommonContext.Database.ExecuteSqlRaw(sql, parameters);

        /// <inheritdoc/>
        public IQueryable<TEntity> FromSql<TEntity>(string sql, params object[] parameters)
            where TEntity : class =>
            CommonContext.Set<TEntity>().FromSqlRaw(sql, parameters);

        /// <inheritdoc/>
        public int SaveChanges()
        {
            int res = -1;
            try
            {
                res = CommonContext.SaveChanges();
                foreach (var item in _aftersaveCallback)
                {
                    item().Wait();
                }
            }
            catch (Exception ex)
            {
                foreach (var item in _errorsCallback)
                {
                    try
                    {
                        item(ex).Wait();
                    }
                    catch (Exception ehEx)
                    {
                        var errorHandleException = new Exception(ehEx.ToString(), ex);
                        _logger.LogError(errorHandleException, "Error while saving exception");
                    }
                }

                throw;
            }

            return res;
        }

        /// <inheritdoc/>
        public async Task CreateSavePointAsync(string savePointName, IDbContextTransaction transaction = null)
        {
            if (transaction != null)
            {
                await transaction.CreateSavepointAsync(savePointName);
            } 
            else if (CommonContext.Database.CurrentTransaction != null)
            {
                await CommonContext.Database.CurrentTransaction.CreateSavepointAsync(savePointName);
            }
        }

        /// <inheritdoc/>
        public async Task SaveChangesAsync()
        {
            try
            {
                await CommonContext.SaveChangesAsync();
                foreach (var item in _aftersaveCallback)
                {
                    await item.Invoke();
                }
            }
            catch (Exception ex)
            {
                foreach (var item in _errorsCallback)
                {
                    try
                    {
                        item.Invoke(ex).Wait();
                    }
                    catch (Exception ehEx)
                    {
                        var errorHandleException = new Exception(ehEx.ToString(), ex);
                        _logger.LogError(errorHandleException, "Error while saving exception");
                    }
                }

                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<int> ExecuteRawSqlAsync(string sql, params object[] parameters)
        {
            return await CommonContext.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        /// <inheritdoc/>
        public IDbContextTransaction BeginDbTransaction()
        {
            return CommonContext.Database.BeginTransaction();
        }

        /// <inheritdoc/>
        public void UseTransaction(IDbContextTransaction dbTransaction)
        {
            CommonContext.Database.UseTransaction(dbTransaction.GetDbTransaction());
        }

        /// <inheritdoc/>
        public async Task SaveChangesAsync(params IUnitOfWorkBase[] unitOfWorks)
        {
            using var ts = new TransactionScope();
            foreach (var unitOfWork in unitOfWorks)
            {
                await unitOfWork.SaveChangesAsync();
            }

            ts.Complete();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // dispose the db context.
                    CommonContext.Dispose();
                    _aftersaveCallback.Clear();
                }
            }

            _disposed = true;
        }

        /// <inheritdoc/>
        public void TrackGraph(object rootEntity, Action<EntityEntryGraphNode> callback)
        {
            CommonContext.ChangeTracker.TrackGraph(rootEntity, callback);
        }

        /// <inheritdoc/>
        public IUnitOfWorkBase SaveCallback(Func<Task> callback)
        {
            _aftersaveCallback.Add(callback);
            return this;
        }

        /// <inheritdoc/>
        public IUnitOfWorkBase OnErrorCallback(Func<Exception, Task> callback)
        {
            _errorsCallback.Add(callback);
            return this;
        }

        /// <inheritdoc/>
        public async Task<T> ExecuteInsideTransaction<T>(Func<Task<T>> callback)
        {
            if (CommonContext.Database.CurrentTransaction == null)
            {
                await using var transaction = await CommonContext.Database.BeginTransactionAsync();
                var resp = await callback.Invoke();

                await transaction.CommitAsync();

                return resp;
            } 
            else
            {
                return await callback.Invoke();
            }
        }

        /// <inheritdoc/>
        public async Task ExecuteInsideTransaction(Func<Task> callback)
        {
            await using var transaction = await CommonContext.Database.BeginTransactionAsync();
            await callback.Invoke();

            await transaction.CommitAsync();
        }
    }
}
