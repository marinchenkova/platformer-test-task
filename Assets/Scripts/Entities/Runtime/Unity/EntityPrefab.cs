using Entities.Core;
using UnityEngine;

namespace Entities.Unity {

    /// <summary>
    /// Provides components for entities.
    /// Components in a prefab are not able to hold scene references,
    /// see <see cref="EntityBuilder"/> to use components that are serialized within the scene.
    /// </summary>
    [CreateAssetMenu(fileName = nameof(EntityPrefab), menuName = "Entities/" + nameof(EntityPrefab))]
    public sealed class EntityPrefab : ScriptableObject {

        [SerializeReference] private IEntityComponent[] _components;

        public void CopyComponentsInto(Entity entity) {
            for (int i = 0; i < _components.Length; i++) {
                entity.SetComponentCopy(_components[i]);
            }
        }
    }
}
