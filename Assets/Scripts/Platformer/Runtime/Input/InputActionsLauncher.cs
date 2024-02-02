using System;
using UnityEngine;

namespace Platformer {

    public sealed class InputActionsLauncher : MonoBehaviour {

        public InputActions InputActions { get; private set; }

        private void Awake() {
            InputActions = new InputActions();
        }

        private void OnEnable() {
            InputActions.Enable();
        }

        private void OnDisable() {
            InputActions.Disable();
        }

        private void OnDestroy() {
            InputActions.Disable();
            InputActions.Dispose();
            InputActions = null;
        }
    }

}
