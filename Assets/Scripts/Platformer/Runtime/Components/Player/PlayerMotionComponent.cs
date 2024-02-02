using System;
using Entities.Core;
using Platformer.Utils;
using UnityEngine;

namespace Platformer.Components {

    [Serializable]
    public sealed class PlayerMotionComponent : IEntityComponent, IUpdatable {

        public TickGroup updateAt;

        [Header("Smoothing")]
        [Min(0f)] public float motionInputSmoothing = 20f;
        [Min(0f)] public float rotationInputSmoothing = 20f;
        [Min(0f)] public float motionSqrThreshold = 0.05f;

        [Header("Speed")]
        [Min(0f)] public float walkSpeed = 3f;
        [Min(0f)] public float runSpeed = 5f;
        [Min(0f)] public float crouchSpeed = 2f;
        [Min(0f)] public float inAirSpeed = 1f;

        [Header("Jump")]
        [Min(0f)] public float jumpForceWalk = 5f;
        [Min(0f)] public float jumpForceRun = 7f;
        [Min(0f)] public float jumpForceCrouch = 2f;
        [Min(0f)] public float jumpForceAir = 0f;

        public event Action<Vector3> OnMotionVectorChanged = delegate {  };
        public event Action OnFall = delegate {  };
        public event Action OnLand = delegate {  };
        public event Action OnStartRun = delegate {  };
        public event Action OnStopRun = delegate {  };
        public event Action OnCrouch = delegate {  };
        public event Action OnStand = delegate {  };
        public event Action<Vector3> OnJump = delegate {  };

        public Vector3 MotionVector { get; private set; }
        public Vector3 Velocity => _massComponent?.Velocity ?? Vector3.zero;
        public bool IsRunning { get; private set; }
        public bool IsCrouching { get; private set; }
        public bool IsGrounded => _massComponent?.IsGrounded ?? false;

        private Entity _entity;
        private PlayerInputComponent _playerInputComponent;
        private MassComponent _massComponent;
        private TransformComponent _transformComponent;
        private Quaternion _targetOrientation;
        private float _targetSpeed;
        private bool _isRunPressed;
        private bool _isCrouchPressed;

        void IEntityComponent.OnEnable(Entity entity) {
            _playerInputComponent = entity.GetComponent<PlayerInputComponent>();
            _transformComponent = entity.GetOrAddComponent<TransformComponent>();
            _massComponent = entity.GetComponent<MassComponent>();

            if (_playerInputComponent != null) {
                _playerInputComponent.OnMotionInputChanged -= HandleMotionInputChanged;
                _playerInputComponent.OnRunPressed -= HandleRunPressed;
                _playerInputComponent.OnRunReleased -= HandleRunReleased;
                _playerInputComponent.OnCrouchPressed -= HandleCrouchPressed;
                _playerInputComponent.OnCrouchReleased -= HandleCrouchReleased;
                _playerInputComponent.OnJumpPressed -= HandleJumpPressed;

                _playerInputComponent.OnMotionInputChanged += HandleMotionInputChanged;
                _playerInputComponent.OnRunPressed += HandleRunPressed;
                _playerInputComponent.OnRunReleased += HandleRunReleased;
                _playerInputComponent.OnCrouchPressed += HandleCrouchPressed;
                _playerInputComponent.OnCrouchReleased += HandleCrouchReleased;
                _playerInputComponent.OnJumpPressed += HandleJumpPressed;
            }

            if (_massComponent != null) {
                _massComponent.OnLand -= HandleLand;
                _massComponent.OnFall -= HandleFall;

                _massComponent.OnLand += HandleLand;
                _massComponent.OnFall += HandleFall;
            }

            _entity = entity;
            _entity.world.Subscribe(this, updateAt);
        }

        void IEntityComponent.OnDisable(Entity entity) {
            _entity.world.Unsubscribe(this, updateAt);
            _entity = default;

            if (_playerInputComponent != null) {
                _playerInputComponent.OnMotionInputChanged -= HandleMotionInputChanged;
                _playerInputComponent.OnRunPressed -= HandleRunPressed;
                _playerInputComponent.OnRunReleased -= HandleRunReleased;
                _playerInputComponent.OnCrouchPressed -= HandleCrouchPressed;
                _playerInputComponent.OnCrouchReleased -= HandleCrouchReleased;
                _playerInputComponent.OnJumpPressed -= HandleJumpPressed;
            }

            if (_massComponent != null) {
                _massComponent.OnLand -= HandleLand;
                _massComponent.OnFall -= HandleFall;
            }

            _playerInputComponent = null;
            _massComponent = null;
            _transformComponent = null;
        }

        void IUpdatable.OnUpdate(float dt) {
            if (!_entity.IsAlive()) return;

            var input = _playerInputComponent?.MotionInput ?? Vector2.zero;
            var targetInput = new Vector3(input.x, 0f, input.y) * _targetSpeed;
            var lastMotionVector = MotionVector;

            MotionVector = Vector3.Lerp(MotionVector, targetInput, dt * motionInputSmoothing);

            if (MotionVector.sqrMagnitude <= motionSqrThreshold) {
                MotionVector = Vector3.zero;
            }

            if (!input.IsNearlyZero()) {
                _targetOrientation = Quaternion.FromToRotation(Vector3.right, targetInput);
            }

            if (_massComponent != null) _massComponent.Input = MotionVector;
            _transformComponent.rotation = Quaternion.RotateTowards(_transformComponent.rotation, _targetOrientation, dt * rotationInputSmoothing);

            if (!lastMotionVector.IsNearlyEqual(MotionVector)) OnMotionVectorChanged.Invoke(MotionVector);
        }

        private void ProcessState() {
            CheckStateChanges();
            UpdateTargetSpeed();
        }

        private void CheckStateChanges() {
            if (!IsGrounded) {
                if (IsRunning) {
                    IsRunning = false;
                    OnStopRun.Invoke();
                }

                if (IsCrouching && !_isCrouchPressed) {
                    IsCrouching = false;
                    OnStand.Invoke();
                }
                else if (!IsCrouching && _isCrouchPressed) {
                    IsCrouching = true;
                    OnCrouch.Invoke();
                }

                return;
            }

            if (IsCrouching && !_isCrouchPressed) {
                IsCrouching = false;
                OnStand.Invoke();
            }
            else if (!IsCrouching && _isCrouchPressed) {
                IsCrouching = true;
                OnCrouch.Invoke();
            }

            var input = _playerInputComponent?.MotionInput ?? Vector2.zero;

            if (IsRunning && (IsCrouching || !_isRunPressed || input.IsNearlyZero())) {
                IsRunning = false;
                OnStopRun.Invoke();
            }
            else if (!IsRunning && !IsCrouching && _isRunPressed && !input.IsNearlyZero()) {
                IsRunning = true;
                OnStartRun.Invoke();
            }
        }

        private void UpdateTargetSpeed() {
            if (!IsGrounded) {
                _targetSpeed = inAirSpeed;
                return;
            }

            if (IsCrouching) {
                _targetSpeed = crouchSpeed;
                return;
            }

            if (IsRunning) {
                _targetSpeed = runSpeed;
                return;
            }

            _targetSpeed = walkSpeed;
        }

        private void HandleMotionInputChanged(Vector2 input) {
            ProcessState();
        }

        private void HandleRunPressed() {
            _isRunPressed = true;
            ProcessState();
        }

        private void HandleRunReleased() {
            _isRunPressed = false;
            ProcessState();
        }

        private void HandleCrouchPressed() {
            _isCrouchPressed = true;
            ProcessState();
        }

        private void HandleCrouchReleased() {
            _isCrouchPressed = false;
            ProcessState();
        }

        private void HandleJumpPressed() {
            float jumpForce = IsGrounded
                ? IsCrouching
                    ? jumpForceCrouch
                    : IsRunning ? jumpForceRun : jumpForceWalk
                : jumpForceAir;

            if (jumpForce.IsNearlyZero()) return;

            var impulse = Vector3.up * jumpForce;

            _massComponent?.ApplyImpulse(impulse);
            OnJump.Invoke(impulse);
        }

        private void HandleLand() {
            ProcessState();
            OnLand.Invoke();
        }

        private void HandleFall() {
            ProcessState();
            OnFall.Invoke();
        }
    }

}
