using System;
using System.Collections.Generic;
using Stack.Queries;

namespace Stack.Persistence
{
    public interface IDbPersister
    {
        bool IsConnected { get; }
        void Connect();
        void Disconnect();
        IEnumerable<ITableInfo> Schema { get; }
        IDialect Dialect { get; }

        void EnableCache();
        void RemoveFromCache<T>(T entity) where T : Entity;
        void DisableCache();

        TransactionStatus TransactionStatus { get; }
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        void EndTransaction();

        void Flush();
        void Save();

        T GetById<T>(int id) where T : Entity;
        TEntity GetFirstByFilter<TEntity, TFilter>(FilterOptions<TEntity, TFilter> options)
            where TEntity : Entity
            where TFilter : Filter;
        IList<TEntity> GetByFilter<TEntity, TFilter>(FilterOptions<TEntity, TFilter> options)
            where TEntity : Entity
            where TFilter : Filter;
        int GetCount<TEntity, TFilter>(FilterOptions<TEntity, TFilter> option)
            where TEntity : Entity
            where TFilter : Filter;
        T Insert<T>(T entity) where T : Entity;
        T Update<T>(T entity) where T : Entity;
        T DeleteById<T>(int id) where T : Entity, new();

        IEnumerable<T> Execute<T>(IQuery query, Action<T> resultMap = null);
        IEnumerable<TResult> Execute<TInner, TOuter, TResult>(IQuery query, Func<TInner, TOuter, TResult> resultMap) where TInner : Entity where TOuter : Entity where TResult : class, new();
        void Execute(IQuery query);
    }
}
