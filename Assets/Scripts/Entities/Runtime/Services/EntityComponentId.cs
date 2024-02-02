using System;
using Entities.Core;

namespace Entities.Services {

    internal readonly struct EntityComponentId : IEquatable<EntityComponentId> {

        private readonly Entity _entity;
        private readonly Type _type;

        public EntityComponentId(Entity entity, Type type = null) {
            _entity = entity;
            _type = type;
        }

        public bool Equals(EntityComponentId other) {
            return _entity.Equals(other._entity) && _type == other._type;
        }

        public override bool Equals(object obj) {
            return obj is EntityComponentId other && Equals(other);
        }

        public override int GetHashCode() {
            return HashCode.Combine(_entity, _type);
        }

        public static bool operator ==(EntityComponentId left, EntityComponentId right) {
            return left.Equals(right);
        }

        public static bool operator !=(EntityComponentId left, EntityComponentId right) {
            return !left.Equals(right);
        }

        public override string ToString() {
            return $"{nameof(EntityComponentId)}({_entity}, {_type})";
        }
    }

}
