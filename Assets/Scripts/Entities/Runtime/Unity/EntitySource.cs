using System;
using Entities.Core;
using UnityEngine;

namespace Entities.Unity {

    /// <summary>
    /// Creates and holds an entity reference.
    /// Entity is created in the passed <see cref="World"/> at Start call,
    /// then <see cref="EntityBuilder"/> fills the entity with prefab and runtime components.
    /// Entity is destroyed at OnDestroy call.
    /// </summary>
    public sealed class EntitySource : MonoBehaviour {

        [SerializeField] private WorldReference _worldReference;
        [SerializeField] private EntityBuilder _entityBuilder;

        public event Action<Entity> OnEntityCreated = delegate {  };

        public Entity Entity { get; private set; }

        private void Start() {
            if (_worldReference.World == null) return;

            Entity = _worldReference.World.CreateEntity();
            _entityBuilder.CopyComponentsInto(Entity);

            OnEntityCreated.Invoke(Entity);
        }

        private void OnDestroy() {
            if (Entity.IsAlive()) Entity.Destroy();
        }
    }

}
