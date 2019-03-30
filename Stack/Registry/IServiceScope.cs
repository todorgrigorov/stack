using System;

namespace Stack.Registry
{
    public interface IServiceScope
    {
        void Begin(IServiceScope source = null);
        void Run(Action<IServiceScope> action, IServiceScope source = null);
        T Run<T>(Func<IServiceScope, T> action, IServiceScope source = null);
        void End();

        object TryGet(Type type);
        T TryGet<T>();
        object Get(Type type);
        T Get<T>();

        void TryInvoke<T>(Action<T> action);
    }
}
