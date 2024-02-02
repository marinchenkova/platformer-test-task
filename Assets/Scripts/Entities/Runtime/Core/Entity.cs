using System;

namespace Entities.Core {

    /// <summary>
    /// Entity is an ID stored in the World, which points to a group of entity components.
    /// Entity can have only one component per type.
    /// </summary>
    public readonly struct Entity : IEquatable<Entity> {

        /// <summary>
        /// The world in which this entity was created.
        /// World can be used to create and destroy entities, setup components, subscribe to updates.
        /// </summary>
        public readonly World world;

        private readonly long _id;

        internal Entity(World world, long id) {
            _id = id;
            this.world = world;
        }

        public bool Equals(Entity other) {
            return _id == other._id && world == other.world;
        }

        public override bool Equals(object obj) {
            return obj is Entity other && Equals(other);
        }

        public override int GetHashCode() {
            return HashCode.Combine(_id, world);
        }

        public static bool operator ==(Entity left, Entity right) {
            return left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right) {
            return !left.Equals(right);
        }

        public override string ToString() {
            return $"{nameof(Entity)}({_id})";
        }
    }

}
