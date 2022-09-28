using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace DbSet.Extensions
{
    public static class DbSetExtensions
    {
        private static TEntity FirstOrCreate<TEntity>(
            this DbSet<TEntity> dbSet,
            Expression<Func<TEntity, bool>> predicate,
            Func<TEntity> defaultValue,
            bool addIfNotExists)
            where TEntity : class
        {
            var result = predicate != null
                ? dbSet.FirstOrDefault(predicate)
                : dbSet.FirstOrDefault();

            if (result != null) return result;
            result = defaultValue?.Invoke();
            if (result != null && addIfNotExists)
                dbSet.Add(result);
            return result;
        }
        
        public static TEntity FirstOrCreateAndAdd<TEntity>(
            this DbSet<TEntity> dbSet,
            Expression<Func<TEntity, bool>> predicate,
            Func<TEntity> defaultValue)
            where TEntity : class
        {
            return FirstOrCreate(dbSet, predicate, defaultValue, true);
        }
        
        public static TEntity FirstOrCreate<TEntity>(
            this DbSet<TEntity> dbSet,
            Expression<Func<TEntity, bool>> predicate,
            Func<TEntity> defaultValue)
            where TEntity : class
        {
            return FirstOrCreate(dbSet, predicate, defaultValue, false);
        }

        [Obsolete]
        private static TEntity UpdateOrCreate<TEntity>(
            this DbSet<TEntity> dbSet,
            Expression<Func<TEntity, bool>> predicate,
            Func<TEntity, TEntity> updateParams,
            bool addIfNotExists)
            where TEntity : class, new()
        {
            var result = FirstOrCreate(dbSet, predicate, () => new TEntity(), addIfNotExists);
            result = updateParams.Invoke(result);
            dbSet.Update(result);
            return result;
        } 
        
        [Obsolete]
        public static TEntity UpdateOrCreate<TEntity>(
            this DbSet<TEntity> dbSet,
            Expression<Func<TEntity, bool>> predicate,
            Func<TEntity, TEntity> updateParams)
            where TEntity : class, new()
        {
            return UpdateOrCreate(dbSet, predicate, updateParams, false);
        }
        
        [Obsolete]
        public static TEntity UpdateOrCreateAndAdd<TEntity>(
            this DbSet<TEntity> dbSet,
            Expression<Func<TEntity, bool>> predicate,
            Func<TEntity> defaultValue,
            Func<TEntity, TEntity> updateParams)
            where TEntity : class, new()
        {
            return UpdateOrCreate(dbSet, predicate, updateParams, true);
        }
    }
}