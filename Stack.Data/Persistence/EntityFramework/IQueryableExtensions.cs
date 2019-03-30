using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Stack.Data.Persistence.EntityFramework
{
    internal static class IQueryableExtensions
    {
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
        {
            return source.OrderBy(ToLambda<T>(propertyName));
        }
        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
        {
            return source.OrderByDescending(ToLambda<T>(propertyName));
        }

        public static IQueryable<TEntity> ApplyFilter<TEntity, TFilter>(
            this IQueryable<TEntity> queryable,
            FilterOptions<TEntity, TFilter> option)
                where TEntity : Entity
                where TFilter : Filter
        {
            foreach (Func<TEntity, bool> criterion in option.Criteria)
            {
                queryable = queryable
                                .Where(criterion)
                                .AsQueryable();
            }
            return queryable;
        }
        public static IQueryable<TEntity> ApplyPage<TEntity, TFilter>(
            this IQueryable<TEntity> queryable,
            FilterOptions<TEntity, TFilter> option)
                where TEntity : Entity
                where TFilter : Filter
        {
            if (option.Modifier.Page != null)
            {
                queryable = queryable
                                .Skip(option.Modifier.Page.Index * option.Modifier.Page.Size)
                                .Take(option.Modifier.Page.Size);
            }
            return queryable;
        }
        public static IQueryable<TEntity> ApplySort<TEntity, TFilter>(
            this IQueryable<TEntity> queryable,
            FilterOptions<TEntity, TFilter> option)
                where TEntity : Entity
                where TFilter : Filter
        {
            if (option.Modifier.Sort != null)
            {
                foreach (SortOptions sortOption in option.Modifier.Sort)
                {
                    if (sortOption.Order == SortOrder.Ascending)
                    {
                        queryable = queryable
                                    .OrderBy(sortOption.Field)
                                    .AsQueryable();
                    }
                    else
                    {
                        queryable = queryable
                                    .OrderByDescending(sortOption.Field)
                                    .AsQueryable();
                    }
                }
            }
            return queryable;
        }
        public static IQueryable<T> ApplyTracking<T>(this IQueryable<T> queryable, bool track)
            where T : Entity
        {
            if (!track)
            {
                return queryable.AsNoTracking();
            }
            else
            {
                return queryable.AsTracking();
            }
        }
        public static IQueryable<TEntity> ApplyJoins<TEntity, TFilter>(
            this IQueryable<TEntity> queryable,
            FilterOptions<TEntity, TFilter> option)
                where TEntity : Entity
                where TFilter : Filter
        {
            if (option.Modifier.Joins != null)
            {
                foreach (var joinOption in option.Modifier.Joins)
                {
                    queryable = queryable.Include(joinOption.Path);
                }
            }
            return queryable;
        }

        #region Private members
        private static Expression<Func<T, object>> ToLambda<T>(string propertyName)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T));
            MemberExpression property = Expression.Property(parameter, propertyName);
            UnaryExpression asObject = Expression.Convert(property, typeof(object));

            return Expression.Lambda<Func<T, object>>(asObject, parameter);
        }
        #endregion
    }
}
