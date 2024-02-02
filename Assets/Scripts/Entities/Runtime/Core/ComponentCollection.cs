using System;
using Entities.Services;

namespace Entities.Core {

    public ref struct ComponentCollection {

        public IEntityComponent Current => _current;

        private readonly EntityComponentStorage _storage;
        private readonly Entity _entity;

        private IEntityComponent _current;
        private Type _nextComponentType;

        internal ComponentCollection(EntityComponentStorage storage, Entity entity) {
            _storage = storage;
            _entity = entity;
            _current = null;
            _nextComponentType = storage != null && _storage.ComponentData.TryGetValue(new EntityComponentId(_entity), out var container)
                ? container.next
                : null;
        }

        public bool MoveNext() {
            if (_nextComponentType == null) {
                return false;
            }

            var id = new EntityComponentId(_entity, _nextComponentType);
            var componentData = _storage.ComponentData;

            while (componentData.TryGetValue(id, out var data)) {
                if (data.component == null) {
                    id = new EntityComponentId(_entity, data.next);
                    continue;
                }

                _nextComponentType = data.next;
                _current = data.component;
                return true;
            }

            return false;
        }
    }

}
