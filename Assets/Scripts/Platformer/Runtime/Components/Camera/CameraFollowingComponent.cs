using System;
using Entities.Core;
using UnityEngine;

namespace Platformer.Components {

    [Serializable]
    public sealed class CameraFollowingComponent : IEntityComponent, IUpdatable {

        public TickGroup updateAt;
        public float verticalOffset;
        public float horizontalOffset;
        [Min(0f)] public float maxDistanceToUseReturnSpeed;
        [Min(0f)] public float returnSpeed;
        [Min(0f)] public float positionSmoothing;

        private Entity _entity;
        private Transform _cameraTransform;

        void IEntityComponent.OnEnable(Entity entity) {
            _entity = entity;

            var cameraBindingComponent = _entity.GetComponent<CameraBindingComponent>();
            if (cameraBindingComponent == null) return;

            _cameraTransform = cameraBindingComponent.camera.transform;
            _entity.world.Subscribe(this, updateAt);
        }

        void IEntityComponent.OnDisable(Entity entity) {
            _entity.world.Unsubscribe(this, updateAt);
            _entity = default;
            _cameraTransform = null;
        }

        void IUpdatable.OnUpdate(float dt) {
            if (!_entity.IsAlive()) return;

            var transformComponent = _entity.GetOrAddComponent<TransformComponent>();

            var playerPosition = transformComponent.position;
            var currentPosition = _cameraTransform.position;
            var targetPosition = playerPosition + new Vector3(0f, verticalOffset, horizontalOffset);

            float positionFactor = positionSmoothing;
            float maxSqrDistance = maxDistanceToUseReturnSpeed * maxDistanceToUseReturnSpeed;

            if (Vector3.SqrMagnitude(currentPosition - targetPosition) >= maxSqrDistance) {
                positionFactor = returnSpeed;
            }

            currentPosition = Vector3.Lerp(currentPosition, targetPosition, dt * positionFactor);
            _cameraTransform.position = currentPosition;
        }
    }

}
