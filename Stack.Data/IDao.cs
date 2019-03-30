using System.Collections.Generic;

namespace Stack.Data
{
    public interface IDao<T>
        where T : class
    {
        T Load(T data);
        T Load(int id);
        T LoadFirst<TFilter>(TFilter filter = null, SortOptions[] sort = null, JoinOptions[] joins = null) where TFilter : Filter;
        IList<T> List(PageOptions page = null, SortOptions[] sort = null, JoinOptions[] joins = null);
        IList<T> List<TFilter>(
            TFilter filter = null,
            PageOptions page = null,
            SortOptions[] sort = null,
            JoinOptions[] joins = null)
                where TFilter : Filter;
        int Count<TFilter>(TFilter filter = null) where TFilter : Filter;
        bool Exists<TFilter>(int id) where TFilter : Filter, new();
        T Create(T data);
        T Update(T data);
        T Delete(int id);
        T Delete(T data);
    }
}
