using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DbSet.Extensions
{
    public static class DbSetAsyncExtensions
    {
        private static async Task<TEntity> FirstOrCreateAsync<TEntity>(
            this DbSet<TEntity> dbSet,
            Expression<Func<TEntity, bool>> predicate,
            Func<TEntity> defaultValue,
            bool addIfNotExists)
            where TEntity : class
        {
            var result = predicate != null
                ? await dbSet.FirstOrDefaultAsync(predicate)
                : await dbSet.FirstOrDefaultAsync();

            if (result != null) return result;
            result = defaultValue?.Invoke();
            if (result != null && addIfNotExists)
                await dbSet.AddAsync(result);
            return result;
        }
        
        public static async Task<TEntity> FirstOrCreateAndAddAsync<TEntity>(
            this DbSet<TEntity> dbSet,
            Expression<Func<TEntity, bool>> predicate,
            Func<TEntity> defaultValue)
            where TEntity : class
        {
            return await FirstOrCreateAsync(dbSet, predicate, defaultValue, true);
        }
        
        public static async Task<TEntity> FirstOrCreateAsync<TEntity>(
            this DbSet<TEntity> dbSet,
            Expression<Func<TEntity, bool>> predicate,
            Func<TEntity> defaultValue)
            where TEntity : class
        {
            return await FirstOrCreateAsync(dbSet, predicate, defaultValue, false);
        }
        
        private static async Task<TEntity> UpdateOrCreateAsync<TEntity>(
            this DbSet<TEntity> dbSet,
            Expression<Func<TEntity, bool>> predicate,
            Func<TEntity, TEntity> updateParams,
            bool addIfNotExists)
            where TEntity : class, new()
        {
            var result = await FirstOrCreateAsync(dbSet, predicate, () => new TEntity(), addIfNotExists);
            result = updateParams.Invoke(result);
            dbSet.Update(result);
            return result;
        } 
        
        public static async Task<TEntity> UpdateOrCreateAsync<TEntity>(
            this DbSet<TEntity> dbSet,
            Expression<Func<TEntity, bool>> predicate,
            Func<TEntity, TEntity> updateParams)
            where TEntity : class, new()
        {
            return await  UpdateOrCreateAsync(dbSet, predicate, updateParams, false);
        }
        
        public static async Task<TEntity> UpdateOrCreateAndAddAsync<TEntity>(
            this DbSet<TEntity> dbSet,
            Expression<Func<TEntity, bool>> predicate,
            Func<TEntity> defaultValue,
            Func<TEntity, TEntity> updateParams)
            where TEntity : class, new()
        {
            return await UpdateOrCreateAsync(dbSet, predicate, updateParams, true);
        }
    }
}