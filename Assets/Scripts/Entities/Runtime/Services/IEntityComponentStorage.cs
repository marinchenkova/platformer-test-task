using System;
using Entities.Core;

namespace Entities.Services {

    public interface IEntityComponentStorage {

        ComponentCollection GetComponents(Entity entity);

        T GetComponent<T>(Entity entity) where T : class, IEntityComponent;

        void SetComponent(Entity entity, IEntityComponent component);

        void RemoveComponent(Entity entity, Type componentType);

        void RemoveComponents(Entity entity, bool notifyDestroy);

        void Clear();
    }

}
