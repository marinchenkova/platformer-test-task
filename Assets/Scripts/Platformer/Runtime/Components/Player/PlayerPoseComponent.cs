using System;
using Entities.Core;
using UnityEngine;

namespace Platformer.Components {

    [Serializable]
    public sealed class PlayerPoseComponent : IEntityComponent, IUpdatable {

        public TickGroup updateAt;

        [Header("Stand")]
        [Min(0f)] public float standHeight;
        [Min(0f)] public float standRadius;

        [Header("Crouch")]
        [Min(0f)] public float crouchHeight;
        [Min(0f)] public float crouchRadius;

        [Header("Transitions")]
        [Min(0f)] public float poseChangeDuration;

        public float Progress { get; private set; } = 1f;

        private Entity _entity;
        private int _direction = 1;

        private PlayerMotionComponent _playerMotionComponent;
        private CharacterController _characterController;

        void IEntityComponent.OnEnable(Entity entity) {
            _entity = entity;

             var characterControllerBindingComponent = _entity.GetComponent<CharacterControllerBindingComponent>();
             if (characterControllerBindingComponent == null) return;

            _characterController = characterControllerBindingComponent.characterController;
            _playerMotionComponent = _entity.GetComponent<PlayerMotionComponent>();

            if (_playerMotionComponent != null) {
                _playerMotionComponent.OnStand -= OnStand;
                _playerMotionComponent.OnStand += OnStand;

                _playerMotionComponent.OnCrouch -= OnCrouch;
                _playerMotionComponent.OnCrouch += OnCrouch;
            }

            _entity.world.Subscribe(this, updateAt);

            Progress = 1f;
            _direction = 1;

            ApplyHeightByProgress(Progress);
        }

        void IEntityComponent.OnDisable(Entity entity) {
            _entity.world.Unsubscribe(this, updateAt);

            if (_playerMotionComponent != null) {
                _playerMotionComponent.OnStand -= OnStand;
                _playerMotionComponent.OnCrouch -= OnCrouch;
            }

            _entity = default;
            _characterController = null;
            _playerMotionComponent = null;
        }

        void IUpdatable.OnUpdate(float dt) {
            if (!_entity.IsAlive()) return;

            if (_direction > 0 && Progress >= 1f || _direction < 0 && Progress <= 0f) {
                return;
            }

            Progress = poseChangeDuration > 0f
                ? Mathf.Clamp01(Progress + _direction * dt / poseChangeDuration)
                : _direction > 0 ? 1f : 0f;

            ApplyHeightByProgress(Progress);
        }

        private void OnStand() {
            _direction = 1;
        }

        private void OnCrouch() {
            _direction = -1;
        }

        private void ApplyHeightByProgress(float progress) {
            float height = Mathf.Lerp(crouchHeight, standHeight, progress);
            float radius = Mathf.Lerp(crouchRadius, standRadius, progress);
            var center = (height - standHeight) * new Vector3(0f, 0.5f, 0f);

            _characterController.height = height;
            _characterController.radius = radius;
            _characterController.center = center;
        }
    }

}
