using Stack.Data.Registry.DomainEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stack.Registry.DomainEvents
{
    public class DefaultDomainEventContainer : IDomainEventContainer
    {
        public void Invoke<T>(T data, DomainEventType eventType)
            where T : class
        {
            Assure.NotNull(data, nameof(data));

            var scope = ContainerConfiguration.Current.Container.BuildScope();
            scope.Begin();

            try
            {
                switch (eventType)
                {
                    case DomainEventType.Creating:
                        EntityUtil.AssureEntity(data);
                        scope.TryInvoke<IEntityCreating<T>>(s => s.Creating(data));
                        break;
                    case DomainEventType.Created:
                        EntityUtil.AssureEntity(data);
                        scope.TryInvoke<IEntityCreated<T>>(s => s.Created(data));
                        break;
                    case DomainEventType.Updating:
                        EntityUtil.AssureEntity(data);
                        scope.TryInvoke<IEntityUpdating<T>>(s => s.Updating(data));
                        break;
                    case DomainEventType.Updated:
                        EntityUtil.AssureEntity(data);
                        scope.TryInvoke<IEntityUpdated<T>>(s => s.Updated(data));
                        break;
                    case DomainEventType.Deleting:
                        EntityUtil.AssureEntity(data);
                        scope.TryInvoke<IEntityDeleting<T>>(s => s.Deleting(data));
                        break;
                    case DomainEventType.Deleted:
                        EntityUtil.AssureEntity(data);
                        scope.TryInvoke<IEntityDeleted<T>>(s => s.Deleted(data));
                        break;
                    case DomainEventType.Uniqueness:
                        EntityUtil.AssureEntity(data);
                        scope.TryInvoke<IEntityUniqueness<T>>(s => s.Unique(data));
                        break;
                    case DomainEventType.Querying:
                        scope.TryInvoke<IEntityQuerying<T>>(s => s.Querying(data));
                        break;
                    default:
                        EventTypeMissing(eventType);
                        break;
                }
            }
            finally
            {
                scope.End();
            }
        }

        public void RegisterAll()
        {
            // automatically register ALL domain handlers defined by first getting them
            // and then checking for the implementation type (the interface)

            IEnumerable<Type> handlers = TypeLoader.LoadTypes(
                                            null,
                                            typeof(IEntityCreating<>),
                                            typeof(IEntityCreated<>),
                                            typeof(IEntityUpdating<>),
                                            typeof(IEntityUpdated<>),
                                            typeof(IEntityDeleting<>),
                                            typeof(IEntityDeleted<>),
                                            typeof(IEntityUniqueness<>),
                                            typeof(IEntityQuerying<>));

            IContainer container = ContainerConfiguration.Current.Container;
            foreach (Type handler in handlers)
            {
                Type handlerDeclaration = null;
                if (handler.Implements(typeof(IEntityCreating<>)))
                {
                    handlerDeclaration = typeof(IEntityCreating<>);
                }
                else if (handler.Implements(typeof(IEntityCreated<>)))
                {
                    handlerDeclaration = typeof(IEntityCreated<>);
                }
                else if (handler.Implements(typeof(IEntityUpdating<>)))
                {
                    handlerDeclaration = typeof(IEntityUpdating<>);
                }
                else if (handler.Implements(typeof(IEntityUpdated<>)))
                {
                    handlerDeclaration = typeof(IEntityUpdated<>);
                }
                else if (handler.Implements(typeof(IEntityDeleting<>)))
                {
                    handlerDeclaration = typeof(IEntityDeleting<>);
                }
                else if (handler.Implements(typeof(IEntityDeleted<>)))
                {
                    handlerDeclaration = typeof(IEntityDeleted<>);
                }
                else if (handler.Implements(typeof(IEntityUniqueness<>)))
                {
                    handlerDeclaration = typeof(IEntityUniqueness<>);
                }
                else if (handler.Implements(typeof(IEntityQuerying<>)))
                {
                    handlerDeclaration = typeof(IEntityQuerying<>);
                }

                Type type = handler
                            .LoadInterface(handlerDeclaration.Name)
                            .GetGenericArguments()
                            .First();
                container.Register(handlerDeclaration.MakeGenericType(type), handler);
            }
        }

        #region Private members
        private void EventTypeMissing(DomainEventType type)
        {
            throw new NotImplementedException($"Domain event has not been configured for handling {type.ToString()}.");
        }
        #endregion
    }
}