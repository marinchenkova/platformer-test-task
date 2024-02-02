using Entities.Core;
using UnityEngine;

namespace Entities.Unity {

    /// <summary>
    /// Provides components for entities.
    /// Can hold components with scene references.
    /// Can be used with <see cref="EntitySource"/> to create persistent entities.
    /// </summary>
    public sealed class EntityBuilder : MonoBehaviour {

        [SerializeField] private EntityPrefab _entityPrefab;
        [SerializeField] private bool _applyPrefabFirst;
        [SerializeReference] private IEntityComponent[] _components;

        public void CopyComponentsInto(Entity entity) {
            if (_applyPrefabFirst && _entityPrefab != null) _entityPrefab.CopyComponentsInto(entity);

            for (int i = 0; i < _components.Length; i++) {
                entity.SetComponentCopy(_components[i]);
            }

            if (!_applyPrefabFirst && _entityPrefab != null) _entityPrefab.CopyComponentsInto(entity);
        }
    }

}
