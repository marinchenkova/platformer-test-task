using System;
using Entities.Core;

namespace Platformer.Components {

    [Serializable]
    public sealed class DestroyBelowHeightComponent : IEntityComponent, IUpdatable {

        public TickGroup updateAt;
        public float height;

        private Entity _entity;

        void IEntityComponent.OnEnable(Entity entity) {
            _entity = entity;
            _entity.world.Subscribe(this, updateAt);
        }

        void IEntityComponent.OnDisable(Entity entity) {
            _entity.world.Unsubscribe(this, updateAt);
            _entity = default;
        }

        void IUpdatable.OnUpdate(float dt) {
            if (!_entity.IsAlive()) return;

            float currentHeight = _entity.GetComponent<TransformComponent>()?.position.y ?? 0f;
            if (currentHeight >= height) return;

            _entity.Destroy();
        }
    }

}
