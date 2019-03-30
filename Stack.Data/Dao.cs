using System;
using System.Collections.Generic;
using System.Linq;
using Stack.Mapping;
using Stack.Persistence;
using Stack.Queries;
using Stack.Registry;
using Stack.Registry.DomainEvents;

namespace Stack.Data
{
    public static class Dao
    {
        public static IDao<T> For<T>()
            where T : Entity, new()
        {
            return new Dao<T>();
        }
        public static IDao<TModel> ForContract<TEntity, TModel>()
            where TEntity : Entity, new()
            where TModel : Model, new()
        {
            return new ContractDao<TEntity, TModel>();
        }
        public static IMappingDao<T> ForMap<T>()
            where T : Entity
        {
            return new MappingDao<T>();
        }
    }

    internal class Dao<T> : IDao<T>
        where T : Entity, new()
    {
        internal Dao()
        {
            Type = typeof(T);
            Database.ValidateTable(Type);
            Persister = Database.Persister;
            DomainEventContainer = ContainerConfiguration.Current.DomainEventContainer;
        }

        public T Load(T data)
        {
            Assure.NotNull(data, nameof(data));
            EntityUtil.AssureEntity(data);
            return Load(data.Id);
        }
        public T Load(int id)
        {
            Assure.NotEqual(id, 0, nameof(id));

            T entity = Persister.GetById<T>(id);
            if (entity == null)
            {
                throw new EntityNotFoundException(new NotFoundError($"Entity could not be found.", id));
            }
            return entity;
        }
        public T LoadFirst<TFilter>(TFilter filter = null, SortOptions[] sort = null, JoinOptions[] joins = null)
            where TFilter : Filter
        {
            return Persister.GetFirstByFilter(GetFilterOptions(filter: filter, page: null, sort: sort, joins: joins));
        }
        public IList<T> List(PageOptions page = null, SortOptions[] sort = null, JoinOptions[] joins = null)
        {
            return List<Filter>(filter: null, page: page, sort: sort, joins: joins);
        }
        public IList<T> List<TFilter>(
            TFilter filter = null,
            PageOptions page = null,
            SortOptions[] sort = null,
            JoinOptions[] joins = null)
                where TFilter : Filter
        {
            return Persister.GetByFilter(GetFilterOptions(filter: filter, page: page, sort: sort, joins: joins));
        }
        public int Count<TFilter>(TFilter filter = null) where TFilter : Filter
        {
            return Persister.GetCount(GetFilterOptions(filter: filter, page: null, sort: null));
        }
        public bool Exists<TFilter>(int id)
            where TFilter : Filter, new()
        {
            Assure.NotEqual(id, 0, nameof(id));
            return Count(new TFilter() { Ids = new int[] { id } }) == 1;
        }

        public T Create(T data)
        {
            Assure.NotNull(data, nameof(data));
            EntityUtil.AssureEntity(data);

            if (!data.IsNew)
            {
                throw new InvalidOperationException($"Attempt to create a new object of type {Type.FullName} which already exists.");
            }

            DomainEventContainer.Invoke(data, DomainEventType.Creating);
            DomainEventContainer.Invoke(data, DomainEventType.Uniqueness);

            data.Validate();
            data.SetTimestamps();

            data = Persister.Insert(data);
            DomainEventContainer.Invoke(data, DomainEventType.Created);

            return data;
        }
        public T Update(T data)
        {
            Assure.NotNull(data, nameof(data));
            EntityUtil.AssureEntity(data);

            T entity = null;
            if (data.IsNew)
            {
                throw new InvalidOperationException($"Attempt to update a new object of type {Type.FullName} which has not been created.");
            }

            DomainEventContainer.Invoke(entity, DomainEventType.Updating);
            DomainEventContainer.Invoke(entity, DomainEventType.Uniqueness);

            entity.Validate();
            entity.SetTimestamps();

            entity = Persister.Update(entity);
            DomainEventContainer.Invoke(entity, DomainEventType.Updated);

            return entity;
        }
        public T Delete(int id)
        {
            if (id == 0)
            {
                throw new InvalidOperationException($"Attempt to delete a object of type {Type.FullName} which has not been created.");
            }

            T data = Load(id);
            DomainEventContainer.Invoke(data, DomainEventType.Deleting);

            data = Persister.DeleteById<T>(id);
            DomainEventContainer.Invoke(data, DomainEventType.Deleted);

            return data;
        }
        public T Delete(T data)
        {
            Assure.NotNull(data, nameof(data));
            EntityUtil.AssureEntity(data);
            return Delete(data.Id);
        }

        #region Internal members
        internal Type Type { get; private set; }
        internal IDbPersister Persister { get; private set; }
        internal IDomainEventContainer DomainEventContainer { get; private set; }

        internal void WithoutCache(Action action)
        {
            Assure.NotNull(action, nameof(action));
            Persister.DisableCache();
            action.Invoke();
            Persister.EnableCache();
        }
        #endregion

        #region Private members
        private FilterOptions<T, TFilter> GetFilterOptions<TFilter>(
            TFilter filter = null,
            PageOptions page = null,
            SortOptions[] sort = null,
            JoinOptions[] joins = null)
                where TFilter : Filter
        {
            var option = new FilterOptions<T, TFilter>();

            if (filter != null)
            {
                option.Data = filter;
                ApplyDefaultFilterQueries(option);
                DomainEventContainer.Invoke(option, DomainEventType.Querying);
            }

            if (page != null)
            {
                page.Validate();
                option.Modifier.Page = page;
            }

            if (sort != null)
            {
                foreach (SortOptions sortOption in sort)
                {
                    Assure.NotNull(sortOption, nameof(sortOption));
                    sortOption.Validate();
                }
                option.Modifier.Sort = sort;
            }

            if (joins != null)
            {
                foreach (var joinOption in joins)
                {
                    Assure.NotNull(joinOption, nameof(joinOption));
                    joinOption.Validate();
                }
                option.Modifier.Joins = joins;
            }

            return option;
        }
        private void ApplyDefaultFilterQueries<TFilter>(FilterOptions<T, TFilter> option)
            where TFilter : Filter
        {
            if (option.Data.Ids != null && option.Data.Ids.Length > 0)
            {
                option.Criteria.Add(f => option.Data.Ids.Contains(f.Id));
            }
            if (option.Data.NotIds != null && option.Data.NotIds.Length > 0)
            {
                option.Criteria.Add(f => !option.Data.NotIds.Contains(f.Id));
            }

            if (option.Data.CreatedBefore.HasValue)
            {
                option.Criteria.Add(f => f.Created < option.Data.CreatedBefore.Value);
            }
            if (option.Data.CreatedAfter.HasValue)
            {
                option.Criteria.Add(f => f.Created > option.Data.CreatedAfter.Value);
            }

            if (option.Data.UpdatedBefore.HasValue)
            {
                option.Criteria.Add(f => f.Updated < option.Data.UpdatedBefore.Value);
            }
            if (option.Data.UpdatedAfter.HasValue)
            {
                option.Criteria.Add(f => f.Updated > option.Data.UpdatedAfter.Value);
            }
        }
        #endregion
    }

    internal class ContractDao<TEntity, TModel> : IDao<TModel>
        where TEntity : Entity, new()
        where TModel : Model, new()
    {
        internal ContractDao()
        {
            dao = new Dao<TEntity>();
        }

        public TModel Load(TModel data)
        {
            return Load(data.Id);
        }
        public TModel Load(int id)
        {
            TEntity entity = null;
            dao.WithoutCache(() =>
            {
                entity = dao.Load(id);
            });
            return MapFromEntity(entity);
        }
        public TModel LoadFirst<TFilter>(TFilter filter = null, SortOptions[] sort = null, JoinOptions[] joins = null)
            where TFilter : Filter
        {
            TEntity entity = null;
            dao.WithoutCache(() =>
            {
                entity = dao.LoadFirst(filter, sort, joins);
            });
            return MapFromEntity(entity);
        }
        public IList<TModel> List(PageOptions page = null, SortOptions[] sort = null, JoinOptions[] joins = null)
        {
            return List<Filter>(filter: null, page: page, sort: sort, joins: joins);
        }
        public IList<TModel> List<TFilter>(
            TFilter filter = null,
            PageOptions page = null,
            SortOptions[] sort = null,
            JoinOptions[] joins = null)
                where TFilter : Filter
        {
            IList<TEntity> entities = null;
            dao.WithoutCache(() =>
            {
                entities = dao.List(filter: filter, page: page, sort: sort, joins: joins);
            });

            List<TModel> result = new List<TModel>();
            foreach (TEntity entity in entities)
            {
                result.Add(MapFromEntity(entity));
            }
            return result;
        }
        public int Count<TFilter>(TFilter filter = null) where TFilter : Filter
        {
            int count = 0;
            dao.WithoutCache(() =>
            {
                count = dao.Count(filter);
            });
            return count;
        }
        public bool Exists<TFilter>(int id)
            where TFilter : Filter, new()
        {
            bool exists = false;
            dao.WithoutCache(() =>
            {
                exists = dao.Exists<TFilter>(id);
            });
            return exists;
        }

        public TModel Create(TModel data)
        {
            TEntity mapped = MapToEntity(data);
            mapped = dao.Create(mapped);
            return MapFromEntity(mapped);
        }
        public TModel Update(TModel data)
        {
            TEntity mapped = MapToEntity(data);
            mapped = dao.Update(mapped);
            return MapFromEntity(mapped);
        }
        public TModel Delete(int id)
        {
            TEntity entity = dao.Delete(id);
            return MapFromEntity(entity);
        }
        public TModel Delete(TModel data)
        {
            return Delete(data.Id);
        }

        #region Private members
        private TModel MapFromEntity(TEntity entity)
        {
            return ContainerConfiguration.Current.Container.BuildScope().Run(s =>
            {
                return s.Get<IMapper>().Map<TEntity, TModel>(entity);
            });
        }
        private TEntity MapToEntity(TModel model)
        {
            return ContainerConfiguration.Current.Container.BuildScope().Run(s =>
            {
                return s.Get<IMapper>().Map<TModel, TEntity>(model);
            });
        }

        private Dao<TEntity> dao;
        #endregion
    }

    internal class MappingDao<T> : IMappingDao<T>
        where T : Entity
    {
        public MappingDao()
        {
            Database.ValidateTable(typeof(T));
            Persister = Database.Persister;
        }

        public IEnumerable<T> LoadAndMap(IQuery query, Action<T> resultMap = null)
        {
            return Persister.Execute(query, resultMap);
        }
        public IEnumerable<TResult> LoadAndMap<TResult>(IQuery query, Action<TResult> resultMap = null)
        {
            return Persister.Execute(query, resultMap);
        }
        public IEnumerable<TResult> LoadAndMap<TJoin, TResult>(IQuery query, Func<T, TJoin, TResult> resultMap)
            where TJoin : Entity
            where TResult : class, new()
        {
            return Persister.Execute(query, resultMap);
        }

        #region Internal members
        internal IDbPersister Persister { get; private set; }
        #endregion
    }
}
