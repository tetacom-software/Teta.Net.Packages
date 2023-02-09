using System.Data.Common;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Teta.Packages.UoW.EfCore.Interfaces.BusinessEntity;

namespace Teta.Packages.UoW.EfCore.Business
{
    /// <summary>
    /// Фабрика репозиториев для инжектирования через контекст типа сущности
    /// </summary>
    /// <typeparam name="T">Тип сущности</typeparam>
    /// <typeparam name="TKey">Тип ПК сущности</typeparam>
    public class FactoryBasedGenericRepository<T, TKey> : IGenericRepository<T, TKey>
        where TKey : struct
        where T : class, IBusinessEntity<TKey>, new()
    {
        private readonly IGenericRepository<T, TKey> _wrappedEntity;
        public FactoryBasedGenericRepository(IGenericRepositoryFactory factory)
        {
            _wrappedEntity = factory.Create<T, TKey>();
        }

        /// <inheritdoc/>
        public ValueTask<EntityEntry<T>> AddAsync(T entity)
        {
            return _wrappedEntity.AddAsync(entity);
        }

        /// <inheritdoc/>
        public void AddRange(IEnumerable<T> entities)
        {
            _wrappedEntity.AddRange(entities);
        }

        /// <inheritdoc/>
        public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return _wrappedEntity.AnyAsync(predicate);
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<T> AsAsyncEnumerable()
        {
            return _wrappedEntity.AsAsyncEnumerable();
        }

        /// <inheritdoc/>
        public void Delete(T entity)
        {
            _wrappedEntity.Delete(entity);
        }

        /// <inheritdoc/>
        public Task DeleteByIdAsync(TKey id)
        {
            return _wrappedEntity.DeleteByIdAsync(id);
        }

        /// <inheritdoc/>
        public Task DeleteRangeAsync(IEnumerable<TKey> entity)
        {
            return _wrappedEntity.DeleteRangeAsync(entity);
        }

        /// <inheritdoc/>
        public void DeleteRangeAsync(IEnumerable<T> entities)
        {
            _wrappedEntity.DeleteRangeAsync(entities);
        }

        /// <inheritdoc/>
        public Task BulkInsertOrUpdateAsync(IList<T> entities)
        {
            return _wrappedEntity.BulkInsertOrUpdateAsync(entities);
        }

        /// <inheritdoc/>
        public Task BulkUpdateAsync(IList<T> entities)
        {
            return _wrappedEntity.BulkUpdateAsync(entities);
        }

        /// <inheritdoc/> 
        public Task BulkInsertAsync(IList<T> entities)
        {
            return _wrappedEntity.BulkInsertAsync(entities);
        }

        /// <inheritdoc/>
        public IQueryable<T> Execute(string query, DbParameter sqlParam)
        {
            return _wrappedEntity.Execute(query, sqlParam);
        }

        /// <inheritdoc/>
        public IQueryable<T> ExecuteString(string query)
        {
            return _wrappedEntity.ExecuteString(query);
        }

        /// <inheritdoc/>
        public Task<T> FindAsync(params TKey[] keys)
        {
            return _wrappedEntity.FindAsync(keys);
        }

        /// <inheritdoc/>
        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return _wrappedEntity.FindBy(predicate);
        }

        /// <inheritdoc/>
        public T First(Expression<Func<T, bool>> predicate)
        {
            return _wrappedEntity.First(predicate);
        }

        /// <inheritdoc/>
        public T FirstOrDefault(Expression<Func<T, bool>> predicate = null)
        {
            return _wrappedEntity.FirstOrDefault(predicate);
        }

        /// <inheritdoc/>
        public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate = null)
        {
            return _wrappedEntity.FirstOrDefaultAsync(predicate);
        }

        /// <inheritdoc/>
        public IQueryable<T> GetAll()
        {
            return _wrappedEntity.GetAll();
        }

        /// <inheritdoc/>
        public Task<T> GetByIdAsync(TKey key)
        {
            return _wrappedEntity.GetByIdAsync(key);
        }

        /// <inheritdoc/>
        public IQueryable<IGrouping<TK, T>> GroupBy<TK>(Expression<Func<T, TK>> predicate)
        {
            return _wrappedEntity.GroupBy(predicate);
        }

        /// <inheritdoc/>
        public IOrderedQueryable<T> OrderBy<TK>(Expression<Func<T, TK>> predicate)
        {
            return _wrappedEntity.OrderBy(predicate);
        }

        /// <inheritdoc/>
        public void RemoveRange(IEnumerable<T> entities)
        {
            _wrappedEntity.RemoveRange(entities);
        }

        /// <inheritdoc/>
        public void SaveChanges()
        {
            _wrappedEntity.SaveChanges();
        }

        /// <inheritdoc/>
        public Task SaveChangesAsync()
        {
            return _wrappedEntity.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public void Update(T entity)
        {
            _wrappedEntity.Update(entity);
        }

        /// <inheritdoc/>
        public void UpdateRange(IEnumerable<T> entities)
        {
            _wrappedEntity.UpdateRange(entities);
        }

        /// <inheritdoc/>
        public Task<T> Upsert(T entity)
        {
            return _wrappedEntity.Upsert(entity);
        }

        public Task BulkDeleteAsync(IList<T> entities)
        {
            return _wrappedEntity.BulkDeleteAsync(entities);
        }
    }
}
