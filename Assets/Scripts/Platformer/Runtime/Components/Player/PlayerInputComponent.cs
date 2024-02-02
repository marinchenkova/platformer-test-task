using System;
using Entities.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Platformer.Components {

    [Serializable]
    public sealed class PlayerInputComponent : IEntityComponent {

        [SerializeField] private InputActionsLauncher _inputActionsLauncher;

        public event Action<Vector2> OnMotionInputChanged = delegate {  };
        public event Action<float> OnZoomChanged = delegate {  };
        public Vector2 MotionInput { get; private set; }

        public event Action OnRunPressed = delegate {  };
        public event Action OnRunReleased = delegate {  };
        public bool IsRunPressed { get; private set; }

        public event Action OnCrouchPressed = delegate {  };
        public event Action OnCrouchReleased = delegate {  };
        public bool IsCrouchPressed { get; private set; }

        public event Action OnJumpPressed = delegate {  };

        void IEntityComponent.OnEnable(Entity entity) {
            var playerMap = _inputActionsLauncher.InputActions.Player;

            playerMap.Move.performed -= OnMove;
            playerMap.Move.performed += OnMove;

            playerMap.Move.canceled -= OnMoveCancelled;
            playerMap.Move.canceled += OnMoveCancelled;

            playerMap.Run.performed -= OnStartRun;
            playerMap.Run.performed += OnStartRun;

            playerMap.Run.canceled -= OnStopRun;
            playerMap.Run.canceled += OnStopRun;

            playerMap.Crouch.performed -= OnCrouch;
            playerMap.Crouch.performed += OnCrouch;

            playerMap.Crouch.canceled -= OnCrouchCancelled;
            playerMap.Crouch.canceled += OnCrouchCancelled;

            playerMap.Jump.performed -= OnJump;
            playerMap.Jump.performed += OnJump;

            playerMap.Zoom.performed -= OnZoom;
            playerMap.Zoom.performed += OnZoom;

            MotionInput = playerMap.Move.ReadValue<Vector2>();
            OnMotionInputChanged.Invoke(MotionInput);
        }

        void IEntityComponent.OnDisable(Entity entity) {
            if (_inputActionsLauncher == null || _inputActionsLauncher.InputActions == null) return;

            var playerMap = _inputActionsLauncher.InputActions.Player;

            playerMap.Move.performed -= OnMove;
            playerMap.Move.canceled -= OnMoveCancelled;

            playerMap.Run.performed -= OnStartRun;
            playerMap.Run.canceled -= OnStopRun;

            playerMap.Crouch.performed -= OnCrouch;
            playerMap.Crouch.canceled -= OnCrouchCancelled;

            playerMap.Jump.performed -= OnJump;

            playerMap.Zoom.performed -= OnZoom;
        }

        private void OnMove(InputAction.CallbackContext ctx) {
            MotionInput = ctx.ReadValue<Vector2>();
            OnMotionInputChanged.Invoke(MotionInput);
        }

        private void OnMoveCancelled(InputAction.CallbackContext ctx) {
            MotionInput = ctx.ReadValue<Vector2>();
            OnMotionInputChanged.Invoke(MotionInput);
        }

        private void OnStartRun(InputAction.CallbackContext ctx) {
            IsRunPressed = true;
            OnRunPressed.Invoke();
        }

        private void OnStopRun(InputAction.CallbackContext ctx) {
            IsRunPressed = false;
            OnRunReleased.Invoke();
        }

        private void OnCrouch(InputAction.CallbackContext ctx) {
            IsCrouchPressed = true;
            OnCrouchPressed.Invoke();
        }

        private void OnCrouchCancelled(InputAction.CallbackContext ctx) {
            IsCrouchPressed = false;
            OnCrouchReleased.Invoke();
        }

        private void OnJump(InputAction.CallbackContext ctx) {
            OnJumpPressed.Invoke();
        }

        private void OnZoom(InputAction.CallbackContext ctx) {
            OnZoomChanged.Invoke(ctx.ReadValue<Vector2>().y);
        }
    }

}
