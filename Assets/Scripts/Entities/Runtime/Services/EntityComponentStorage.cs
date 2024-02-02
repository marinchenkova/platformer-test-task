using System;
using System.Collections.Generic;
using Entities.Core;

namespace Entities.Services {

    internal sealed class EntityComponentStorage : IEntityComponentStorage {

        public IReadOnlyDictionary<EntityComponentId, EntityComponentContainer> ComponentData => _componentData;

        private readonly Dictionary<EntityComponentId, EntityComponentContainer> _componentData
            = new Dictionary<EntityComponentId, EntityComponentContainer>();

        public ComponentCollection GetComponents(Entity entity) {
            return new ComponentCollection(this, entity);
        }

        public T GetComponent<T>(Entity entity) where T : class, IEntityComponent {
            var id = new EntityComponentId(entity, typeof(T));
            return _componentData.TryGetValue(id, out var data)
                ? data.component as T
                : default;
        }

        public void SetComponent(Entity entity, IEntityComponent component) {
            if (component == null) return;

            var componentType = component.GetType();
            var id = new EntityComponentId(entity, componentType);

            if (_componentData.TryGetValue(id, out var data)) {
                data.component?.OnDisable(entity);
                _componentData[id] = new EntityComponentContainer(component, data.next);
                component.OnEnable(entity);

                return;
            }

            var firstEntryId = new EntityComponentId(entity);
            var nextComponentType = _componentData.TryGetValue(firstEntryId, out data) ? data.next : null;

            _componentData[firstEntryId] = new EntityComponentContainer(default, componentType);
            _componentData[id] = new EntityComponentContainer(component, nextComponentType);

            component.OnEnable(entity);
        }

        public void RemoveComponent(Entity entity, Type componentType) {
            if (componentType == null) return;

            var id = new EntityComponentId(entity, componentType);

            if (_componentData.TryGetValue(id, out var data) && data.component != null) {
                _componentData[id] = new EntityComponentContainer(null, data.next);

                data.component.OnDisable(entity);
            }
        }

        public void RemoveComponents(Entity entity, bool notifyDestroy) {
            var id = new EntityComponentId(entity);

            while (_componentData.TryGetValue(id, out var data)) {
                data.component?.OnDisable(entity);

                if (notifyDestroy) {
                    data.component?.OnDestroy(entity);
                }

                if (data.next == null) break;

                id = new EntityComponentId(entity, data.next);
            }

            id = new EntityComponentId(entity);

            while (_componentData.TryGetValue(id, out var data)) {
                _componentData.Remove(id);
                id = new EntityComponentId(entity, data.next);
            }
        }

        public void Clear() {
            _componentData.Clear();
        }
    }

}
