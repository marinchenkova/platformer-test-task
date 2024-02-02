using System;
using Entities.Core;
using Entities.Unity;
using UnityEngine;

namespace Platformer.Components {

    [Serializable]
    public sealed class SpawnEntityOnDestroyComponent : IEntityComponent {

        public EntityPrefab prefab;
        public Vector3 scale = Vector3.one;
        public int count;

        void IEntityComponent.OnDestroy(Entity entity) {
            var position = entity.GetComponent<TransformComponent>()?.position ?? Vector3.zero;

            for (int i = 0; i < count; i++) {
                var newEntity = entity.world.CreateEntity();

                var transformComponent = newEntity.GetOrAddComponent<TransformComponent>();
                transformComponent.position = position;
                transformComponent.scale = scale;

                prefab.CopyComponentsInto(newEntity);
            }
        }
    }

}
