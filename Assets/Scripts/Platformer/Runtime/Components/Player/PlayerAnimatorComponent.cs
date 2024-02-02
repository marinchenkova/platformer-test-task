using System;
using Entities.Core;
using UnityEngine;

namespace Platformer.Components {

    [Serializable]
    public sealed class PlayerAnimatorComponent : IEntityComponent, IUpdatable {

        public TickGroup updateAt;

        [Header("Speed")]
        public string speedParameter;
        public AnimationCurve speedCurve;

        [Header("Pose")]
        public string poseParameter;
        public AnimationCurve poseCurve;

        [Header("Jump")]
        public string jumpParameter;
        public string isGroundedParameter;

        [Header("Death")]
        public string isDeadParameter;

        private Animator _animator;
        private PlayerMotionComponent _playerMotionComponent;
        private PlayerPoseComponent _playerPoseComponent;
        private Entity _entity;

        private int _speedParameterHash;
        private int _poseParameterHash;
        private int _jumpParameterHash;
        private int _isGroundedParameterHash;
        private int _isDeadParameterHash;

        private bool _isDead;

        void IEntityComponent.OnEnable(Entity entity) {
            _speedParameterHash = Animator.StringToHash(speedParameter);
            _poseParameterHash = Animator.StringToHash(poseParameter);
            _jumpParameterHash = Animator.StringToHash(jumpParameter);
            _isGroundedParameterHash = Animator.StringToHash(isGroundedParameter);
            _isDeadParameterHash = Animator.StringToHash(isDeadParameter);

            _entity = entity;
            _animator = entity.GetComponent<AnimatorBindingComponent>().animator;
            _playerMotionComponent = entity.GetComponent<PlayerMotionComponent>();
            _playerPoseComponent = entity.GetComponent<PlayerPoseComponent>();

            if (_playerMotionComponent != null) {
                _playerMotionComponent.OnJump -= OnJump;
                _playerMotionComponent.OnJump += OnJump;
            }

            var healthComponent = entity.GetComponent<HealthComponent>();
            if (healthComponent != null) {
                healthComponent.OnDeath -= OnDeath;
                healthComponent.OnDeath += OnDeath;
            }

            _entity.world.Subscribe(this, updateAt);
        }

        void IEntityComponent.OnDisable(Entity entity) {
            if (_playerMotionComponent != null) {
                _playerMotionComponent.OnJump -= OnJump;
            }

            var healthComponent = entity.GetComponent<HealthComponent>();
            if (healthComponent != null) {
                healthComponent.OnDeath -= OnDeath;
            }

            _entity.world.Unsubscribe(this, updateAt);

            _entity = default;
            _animator = null;
            _playerMotionComponent = null;
            _playerPoseComponent = null;
        }

        void IUpdatable.OnUpdate(float dt) {
            if (!_entity.IsAlive()) return;

            bool isGrounded = _isDead || (_playerMotionComponent?.IsGrounded ?? true);
            _animator.SetBool(_isGroundedParameterHash, isGrounded);

            float realSpeed = _isDead ? 0f : _playerMotionComponent?.MotionVector.magnitude ?? 0f;
            float speed = Mathf.Clamp01(speedCurve.Evaluate(realSpeed));
            _animator.SetFloat(_speedParameterHash, speed);

            float realPose = _isDead ? 1f : _playerPoseComponent?.Progress ?? 1f;
            float pose = Mathf.Clamp01(poseCurve.Evaluate(realPose));
            _animator.SetFloat(_poseParameterHash, pose);
        }

        private void OnJump(Vector3 impulse) {
            _animator.SetTrigger(_jumpParameterHash);
        }

        private void OnDeath() {
            _isDead = true;
            _animator.SetBool(_isDeadParameterHash, true);
        }
    }

}
