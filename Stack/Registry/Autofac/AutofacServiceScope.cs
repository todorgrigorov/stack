using System;
using Autofac;
using AFContainer = Autofac.IContainer;

namespace Stack.Registry.Autofac
{
    public class AutofacServiceScope : IServiceScope, IDisposable
    {
        public AutofacServiceScope(AFContainer container)
        {
            this.container = container;
        }

        public void Begin(IServiceScope source = null)
        {
            if (source == null)
            {
                scope = container.BeginLifetimeScope();
            }
            else
            {
                scope = ((AutofacServiceScope)source).Scope.BeginLifetimeScope();
            }
        }
        public void Run(Action<IServiceScope> action, IServiceScope source = null)
        {
            Execute(a =>
            {
                action.Invoke(this);
                return new object();
            }, source);
        }
        public T Run<T>(Func<IServiceScope, T> action, IServiceScope source = null)
        {
            return Execute(action, source);
        }
        public void End()
        {
            Dispose();
        }

        public void TryInvoke<T>(Action<T> action)
        {
            Assure.NotNull(action, nameof(action));
            T service = (T)TryGet(typeof(T));
            if (service != null)
            {
                action.Invoke(service);
            }
        }

        public T Get<T>()
        {
            return (T)Get(typeof(T));
        }
        public object Get(Type type)
        {
            ValidateScope();

            try
            {
                return scope.Resolve(type);
            }
            catch (Exception e)
            {
                throw new RegistryException($"Unable to resolve implementation for {type.FullName}. Maybe it has not been registered?", e);
            }
        }
        public object TryGet(Type type)
        {
            Assure.NotNull(type, nameof(type));
            ValidateScope();
            bool resolved = scope.TryResolve(type, out object instance);
            return resolved ? instance : null;
        }
        public T TryGet<T>()
        {
            return (T)TryGet(typeof(T));
        }

        public void Dispose()
        {
            scope.Dispose();
            scope = null;
        }

        #region Internal members
        internal ILifetimeScope Scope
        {
            get
            {
                return scope;
            }
        }
        #endregion

        #region Private members
        private void ValidateScope()
        {
            if (scope == null)
            {
                throw new RegistryException($"Service lifecycle has not been started. Maybe no call to {nameof(Begin)} has been made?");
            }
        }

        private T Execute<T>(Func<IServiceScope, T> action, IServiceScope source = null)
        {
            Begin(source);
            try
            {
                return action.Invoke(this);
            }
            finally
            {
                End();
            }
        }

        private AFContainer container;
        private ILifetimeScope scope;
        #endregion
    }
}
