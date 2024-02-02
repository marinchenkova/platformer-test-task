using System;
using UnityEngine;

namespace Platformer.Components.Triggers {

    public sealed class TriggerZone : MonoBehaviour {

        public event Action<GameObject> OnEnter = delegate {  };
        public event Action<GameObject> OnExit = delegate {  };

        private void OnTriggerEnter(Collider other) {
            OnEnter.Invoke(other.gameObject);
        }

        private void OnTriggerExit(Collider other) {
            OnExit.Invoke(other.gameObject);
        }
    }

}
