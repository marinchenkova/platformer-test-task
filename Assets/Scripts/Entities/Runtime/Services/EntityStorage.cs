using System.Collections.Generic;
using Entities.Core;

namespace Entities.Services {

    internal sealed class EntityStorage : IEntityStorage {

        private readonly HashSet<Entity> _entities = new HashSet<Entity>();

        public IEnumerable<Entity> Entities => _entities;

        public void AddEntity(Entity entity) {
            _entities.Add(entity);
        }

        public void RemoveEntity(Entity entity) {
            _entities.Remove(entity);
        }

        public bool ContainsEntity(Entity entity) {
            return _entities.Contains(entity);
        }

        public void Clear() {
            _entities.Clear();
        }
    }

}
