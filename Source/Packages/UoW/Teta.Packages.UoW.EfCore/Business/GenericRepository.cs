﻿using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Teta.Packages.UoW.EfCore.Interfaces.BusinessEntity;

namespace Teta.Packages.UoW.EfCore.Business
{
    /// <summary>
    /// Репозиторий для работы с типом объекта из EFCore
    /// </summary>
    public class GenericRepository<T, TKey, TContext> : IGenericRepository<T, TKey, TContext>
        where TKey : struct
        where T : class, IBusinessEntity<TKey>
        where TContext : DbContext
    {
        private readonly DbSet<T> _dbSet;
        private readonly TContext _context;

        /// <summary>
        /// Конструктор, DI инжекция текущего контекста СУБД
        /// </summary>
        public GenericRepository(TContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<T>();
        }

        /// <inheritdoc/> 
        public virtual T First(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.First(predicate);
        }

        /// <inheritdoc/> 
        public virtual T FirstOrDefault(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate == null)
            {
                return _dbSet.FirstOrDefault();
            }

            return _dbSet.FirstOrDefault(predicate);
        }

        /// <inheritdoc/> 
        public virtual IQueryable<T> GetAll(bool asTrack = false)
        {
            return asTrack ? _dbSet : _dbSet.AsNoTracking();
        }

        /// <inheritdoc/> 
        public IAsyncEnumerable<T> AsAsyncEnumerable()
        {
            return _dbSet.AsAsyncEnumerable();
        }

        /// <inheritdoc/> 
        public virtual IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        /// <inheritdoc/> 
        public virtual async Task<T> FindAsync(params TKey[] keys)
        {
            // TODO: проверить на работоспособность
            return await _dbSet.FindAsync(keys);
        }

        /// <inheritdoc/> 
        public virtual async ValueTask<EntityEntry<T>> AddAsync(T entity)
        {
            return await _dbSet.AddAsync(entity);
        }

        /// <inheritdoc/> 
        public virtual void AddRange(IEnumerable<T> entities)
        {
            _dbSet.AddRange(entities);
        }

        /// <inheritdoc/> 
        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        /// <inheritdoc/> 
        public virtual async Task DeleteByIdAsync(TKey id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        /// <inheritdoc/> 
        public async Task DeleteRangeAsync(IEnumerable<TKey> ids)
        {
            var entities = await _dbSet.Where(dS => ids.Contains(dS.Id)).ToListAsync();
            _dbSet.RemoveRange(entities);
        }

        /// <inheritdoc/>
        public void DeleteRangeAsync(IEnumerable<T> entity)
        {
            _dbSet.RemoveRange(entity);
        }

        /// <inheritdoc/> 
        public virtual void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        /// <inheritdoc/> 
        public virtual Task BulkInsertOrUpdateAsync(IList<T> entities)
        {
            return _context.BulkInsertOrUpdateAsync(entities, config => { config.SetOutputIdentity = true; } );
        }

        /// <inheritdoc/> 
        public virtual Task BulkInsertAsync(IList<T> entities)
        {
            return _context.BulkInsertAsync(entities, config => { config.SetOutputIdentity = true; });
        }

        /// <inheritdoc/> 
        public virtual Task BulkUpdateAsync(IList<T> entities)
        {
            return _context.BulkUpdateAsync(entities, config => { config.SetOutputIdentity = true; });
        }

        /// <inheritdoc/> 
        public virtual Task BulkDeleteAsync(IList<T> entities)
        {
            return _context.BulkDeleteAsync(entities);
        }

        /// <inheritdoc/> 
        public virtual void SaveChanges()
        {
            _context.SaveChanges();
        }

        /// <inheritdoc/> 
        public virtual Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        /// <inheritdoc/> 
        public async Task<T> FirstOrDefaultAsync([AllowNull]Expression<Func<T, bool>> predicate = null)
        {
            if (predicate == null)
            {
                return await _dbSet.FirstOrDefaultAsync();
            }

            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        /// <inheritdoc/> 
        public IOrderedQueryable<T> OrderBy<TK>(Expression<Func<T, TK>> predicate)
        {
            return _dbSet.OrderBy(predicate);
        }

        /// <inheritdoc/> 
        public IQueryable<IGrouping<TK, T>> GroupBy<TK>(Expression<Func<T, TK>> predicate)
        {
            return _dbSet.GroupBy(predicate);
        }

        /// <inheritdoc/> 
        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        /// <inheritdoc/> 
        public virtual void UpdateRange(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
        }
        
        /// <inheritdoc/> 
        public virtual async Task<T> GetByIdAsync(TKey key)
        {
            return await _dbSet.FindAsync(key);
        }

        /// <inheritdoc/> 
        public async Task<T> Upsert(T entity)
        {
            var tCacheEntity = await _dbSet.FindAsync(entity.Id);
            if (tCacheEntity == null)
            {
                var entry = await _dbSet.AddAsync(entity);
                tCacheEntity = entry.Entity;
            }

            return tCacheEntity;
        }

        /// <inheritdoc/> 
        public IQueryable<T> Execute(string query, DbParameter sqlParam)
        {
            return _dbSet.FromSqlRaw(query, sqlParam);
        }

        /// <inheritdoc/> 
        public IQueryable<T> ExecuteString(string query)
        {
            return _dbSet.FromSqlRaw(query);
        }
    }
}