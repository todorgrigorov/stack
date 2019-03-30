using System;
using System.Collections.Generic;
using Stack.Queries;

namespace Stack.Data
{
    public interface IMappingDao<T>
        where T : class
    {
        IEnumerable<T> LoadAndMap(IQuery query, Action<T> resultMap = null);
        IEnumerable<TResult> LoadAndMap<TResult>(IQuery query, Action<TResult> resultMap = null);
        IEnumerable<TResult> LoadAndMap<TJoin, TResult>(IQuery query, Func<T, TJoin, TResult> resultMap) where TJoin : Entity where TResult : class, new();
    }
}
