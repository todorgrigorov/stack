using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Stack.Persistence;
using Stack.Queries;

namespace Stack.Data.Persistence.EntityFramework
{
    public abstract class EntityFrameworkPersister : IDbPersister
    {
        public EntityFrameworkPersister(
            IEntityFrameworkContextBuilder contextBuilder,
            IEntityFrameworkPersistenceBuilder persistenceBuilder,
            IDbConnectionStringProvider connectionProvider,
            IDbMapper dbMapper)
        {
            this.contextBuilder = contextBuilder;
            this.persistenceBuilder = persistenceBuilder;
            this.connectionProvider = connectionProvider;
            this.dbMapper = dbMapper;
        }

        public TransactionStatus TransactionStatus
        {
            get
            {
                return status;
            }
        }
        public IEnumerable<ITableInfo> Schema
        {
            get
            {
                bool openConnection = false; // an existing connection might have been established already
                if (!IsConnected)
                {
                    // connection has not been opened yet
                    Connect();
                    openConnection = true;
                }

                IEnumerable<TableInfo> result = GetSchema(
                    context.Database.GetDbConnection(),
                    context.Database.CurrentTransaction?.GetDbTransaction());

                if (IsConnected && openConnection)
                {
                    Disconnect();
                }

                return result;
            }
        }
        public abstract IDialect Dialect { get; }

        public void EnableCache()
        {
            Validate();
            trackEntities = true;
        }
        public void RemoveFromCache<T>(T entity)
            where T : Entity
        {
            Validate();
            context.Entry(entity).State = EntityState.Detached;
        }
        public void DisableCache()
        {
            Validate();
            trackEntities = false;
        }

        public bool IsConnected
        {
            get
            {
                return context != null;
            }
        }
        public void Connect()
        {
            if (IsConnected)
            {
                throw new ConnectionException("A database connection has already been opened.");
            }

            context = new EntityFrameworkContext(contextBuilder, persistenceBuilder, connectionProvider);
            context.Database.OpenConnection();
            trackEntities = true; // enable cache by default
        }
        public void Disconnect()
        {
            if (!IsConnected)
            {
                throw new ConnectionException("No database connections are currently opened.");
            }

            context.Database.CloseConnection();
            context.Dispose();
            context = null;
        }

        public void BeginTransaction()
        {
            Validate();

            if (status != TransactionStatus.NotStarted)
            {
                throw new InvalidTransactionException("A database transaction has already been started.");
            }

            transaction = context.Database.BeginTransaction(IsolationLevel.ReadUncommitted);
            status = TransactionStatus.Begun;
        }
        public void CommitTransaction()
        {
            if (IsConnected)
            {
                if (status != TransactionStatus.Begun)
                {
                    throw new InvalidTransactionException("No database transactions have been started.");
                }

                transaction.Commit();
                status = TransactionStatus.Committed;
            }
        }
        public void RollbackTransaction()
        {
            if (IsConnected)
            {
                if (status != TransactionStatus.Begun)
                {
                    throw new InvalidTransactionException("No database transactions have been started.");
                }

                transaction.Rollback();
                status = TransactionStatus.Rollbacked;
            }
        }
        public void EndTransaction()
        {
            if (IsConnected)
            {
                if (status == TransactionStatus.NotStarted)
                {
                    throw new InvalidTransactionException("No database transactions have been started.");
                }

                transaction.Dispose();
                transaction = null;
                status = TransactionStatus.NotStarted;
            }
        }

        public void Flush()
        {
            if (status == TransactionStatus.Begun)
            {
                RollbackTransaction();
            }

            if (status != TransactionStatus.NotStarted)
            {
                EndTransaction();
            }

            Disconnect();
        }
        public void Save()
        {
            Validate();
            context.SaveChanges();
        }

        public T GetById<T>(int id)
            where T : Entity
        {
            Validate();
            var queryable = context.ForSet<T>();
            return queryable
                       .ApplyTracking(trackEntities)
                       .FirstOrDefault(e => e.Id == id);

        }
        public TEntity GetFirstByFilter<TEntity, TFilter>(FilterOptions<TEntity, TFilter> option)
            where TEntity : Entity
            where TFilter : Filter
        {
            Validate();
            var queryable = (IQueryable<TEntity>)context.ForSet<TEntity>();
            return queryable
                       .ApplyTracking(trackEntities)
                       .ApplyJoins(option)
                       .ApplyFilter(option)
                       .ApplySort(option)
                       .FirstOrDefault();
        }
        public IList<TEntity> GetByFilter<TEntity, TFilter>(FilterOptions<TEntity, TFilter> option)
            where TEntity : Entity
            where TFilter : Filter
        {
            Validate();
            var queryable = (IQueryable<TEntity>)context.ForSet<TEntity>();
            return queryable
                       .ApplyTracking(trackEntities)
                       .ApplyJoins(option)
                       .ApplyFilter(option)
                       .ApplyPage(option)
                       .ApplySort(option)
                       .ToList();
        }
        public int GetCount<TEntity, TFilter>(FilterOptions<TEntity, TFilter> option)
            where TEntity : Entity
            where TFilter : Filter
        {
            Validate();
            var queryable = (IQueryable<TEntity>)context.ForSet<TEntity>();
            return queryable
                       .ApplyTracking(trackEntities)
                       .ApplyFilter(option)
                       .Count();
        }

        public T Insert<T>(T entity)
            where T : Entity
        {
            Validate();
            entity = context
                        .ForSet<T>()
                        .Add(entity)
                        .Entity;
            Save();
            return entity;
        }
        public T Update<T>(T entity)
            where T : Entity
        {
            Validate();
            entity = context
                        .ForSet<T>()
                        .Update(entity)
                        .Entity;
            Save();
            return entity;
        }
        public T DeleteById<T>(int id)
            where T : Entity, new()
        {
            Validate();
            T entity = context
                        .ForSet<T>()
                        .Remove(GetById<T>(id))
                        .Entity;
            Save();
            return entity;
        }

        public IEnumerable<T> Execute<T>(IQuery query, Action<T> resultMap = null)
        {
            Validate();
            return dbMapper.LoadAndMap<T>(
                        context.Database.GetDbConnection(),
                        context.Database.CurrentTransaction?.GetDbTransaction(),
                        query,
                        resultMap);
        }
        public IEnumerable<TResult> Execute<TInner, TOuter, TResult>(
            IQuery query,
            Func<TInner, TOuter, TResult> resultMap)
                where TInner : Entity
                where TOuter : Entity
                where TResult : class, new()
        {
            Validate();
            return dbMapper.LoadAndMap(
                        context.Database.GetDbConnection(),
                        context.Database.CurrentTransaction?.GetDbTransaction(),
                        query,
                        resultMap);
        }
        public void Execute(IQuery query)
        {
            Validate();
            context.Database.ExecuteSqlCommand(query.Sql, query.Parameters);
        }

        #region Protected members
        protected abstract IEnumerable<TableInfo> GetSchema(IDbConnection connection, IDbTransaction transaction);
        #endregion

        #region Private members
        private void Validate()
        {
            if (!IsConnected)
            {
                throw new ConnectionException($"Context has not been configured. Maybe no call has been made to {nameof(IDbPersister.Connect)}?");
            }
        }

        private TransactionStatus status;
        private IDbContextTransaction transaction;
        private bool trackEntities;

        private EntityFrameworkContext context;
        private IEntityFrameworkContextBuilder contextBuilder;
        private IEntityFrameworkPersistenceBuilder persistenceBuilder;
        private IDbConnectionStringProvider connectionProvider;
        private IDbMapper dbMapper;
        #endregion
    }
}