using System;
using Entities.Core;
using Platformer.Utils;
using UnityEngine;

namespace Platformer.Components {

    [Serializable]
    public sealed class TwoPointLinearMovementComponent : IEntityComponent, IUpdatable {

        public TickGroup updateAt;
        public Vector3 offset1;
        public Vector3 offset2;
        public float speed;
        public float initialProgress;
        public bool initialDirectionUp;

        private Vector3 _initialPoint;
        private float _progress;
        private float _direction;
        private Entity _entity;

        void IEntityComponent.OnEnable(Entity entity) {
            _entity = entity;
            _entity.world.Subscribe(this, updateAt);

            _direction = initialDirectionUp ? 1 : -1;
            _progress = initialProgress;
            _initialPoint = _entity.GetOrAddComponent<TransformComponent>().position;

            ApplyProgress(_progress);
        }

        void IEntityComponent.OnDisable(Entity entity) {
            var velocityComponent = _entity.GetOrAddComponent<VelocityComponent>();
            velocityComponent.velocity = Vector3.zero;

            _entity.world.Unsubscribe(this, updateAt);
            _entity = default;
        }

        void IUpdatable.OnUpdate(float dt) {
            if (!_entity.IsAlive()) return;

            float distance = Vector3.Magnitude(offset2 - offset1);
            float duration = speed.IsNearlyZero() ? 0f : Mathf.Abs(distance / speed);

            _progress = duration > 0f
                ? Mathf.Clamp01(_progress + _direction * dt / duration)
                : _direction > 0 ? 1f : 0f;

            if (_direction > 0 && _progress >= 1f) _direction = -1;
            else if (_direction < 0 && _progress <= 0f) _direction = 1;

            ApplyProgress(_progress);
        }

        private void ApplyProgress(float progress) {
            var transformComponent = _entity.GetOrAddComponent<TransformComponent>();
            transformComponent.position = Vector3.Lerp(_initialPoint + offset1, _initialPoint + offset2, progress);

            var velocityComponent = _entity.GetOrAddComponent<VelocityComponent>();
            velocityComponent.velocity = (offset2 - offset1).normalized * (speed * _direction);
        }
    }

}
