using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Stack.Queries;

namespace Stack.Data.Persistence
{
    public class DapperDbMapper : IDbMapper
    {
        public DapperDbMapper()
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        public IEnumerable<T> LoadAndMap<T>(
            IDbConnection connection,
            IDbTransaction transaction,
            IQuery query,
            Action<T> resultMap = null)
        {
            Assure.NotNull(connection, nameof(connection));
            Assure.NotNull(query, nameof(query));

            IEnumerable<T> data = connection.Query<T>(query.Sql, param: query.Parameters, transaction: transaction);
            foreach (var item in data)
            {
                resultMap?.Invoke(item);
            }
            return data;
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
            Assure.NotNull(connection, nameof(connection));
            Assure.NotNull(query, nameof(query));

            return connection.Query(query.Sql, resultMap, param: query.Parameters, transaction: transaction, splitOn: nameof(Entity.Id).ToCapitalCase());
        }
    }
}
