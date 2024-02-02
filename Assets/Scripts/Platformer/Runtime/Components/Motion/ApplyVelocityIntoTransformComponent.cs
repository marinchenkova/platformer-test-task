using System;
using Entities.Core;
using UnityEngine;

namespace Platformer.Components {

    [Serializable]
    public sealed class ApplyVelocityIntoTransformComponent : IEntityComponent, IUpdatable {

        public TickGroup updateAt;

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

            var transformComponent = _entity.GetOrAddComponent<TransformComponent>();
            var velocity = _entity.GetComponent<VelocityComponent>()?.velocity ?? Vector3.zero;

            transformComponent.position += velocity * dt;
        }
    }

}
