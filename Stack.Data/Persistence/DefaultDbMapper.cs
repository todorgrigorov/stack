using System;
using System.Collections.Generic;
using System.Data;
using Stack.Queries;

namespace Stack.Data.Persistence
{
    public class DefaultDbMapper : IDbMapper
    {
        public IEnumerable<T> LoadAndMap<T>(IDbConnection connection, IDbTransaction transaction, IQuery query)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<T> LoadAndMap<T>(IDbConnection connection, IDbTransaction transaction, IQuery query, Action<T> resultMap = null)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<TResult> LoadAndMap<TInner, TOuter, TResult>(
            IDbConnection connection,
            IDbTransaction transaction,
            IQuery query,
            Func<TInner, TOuter, TResult> resultMap)
                where TInner : Entity
                where TOuter : Entity
                where TResult : class, new()
        {
            throw new NotImplementedException();
        }
    }
}
