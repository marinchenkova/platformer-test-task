using System;
using Entities.Core;
using UnityEngine;

namespace Platformer.Components {

    [Serializable]
    public sealed class TriggerZoneComponent : IEntityComponent {

        [SerializeField] private LayerMask _includeLayers;

        public event Action<Entity> OnTriggerEnter = delegate {  };
        public event Action<Entity> OnTriggerExit = delegate {  };

        public void OnEnable(Entity entity) {
            var triggerZone = entity.GetComponent<TriggerZoneBindingComponent>().triggerZone;

            triggerZone.OnEnter -= OnEnter;
            triggerZone.OnExit -= OnExit;

            triggerZone.OnEnter += OnEnter;
            triggerZone.OnExit += OnExit;
        }

        public void OnDisable(Entity entity) {
            var triggerZone = entity.GetComponent<TriggerZoneBindingComponent>().triggerZone;

            triggerZone.OnEnter -= OnEnter;
            triggerZone.OnExit -= OnExit;
        }

        private void OnEnter(GameObject gameObject) {
            if (!ContainsLayer(_includeLayers, gameObject.layer)) return;

            var entity = gameObject.GetComponent<IEntityView>()?.Entity ?? default;
            if (!entity.IsAlive()) return;

            OnTriggerEnter.Invoke(entity);
        }

        private void OnExit(GameObject gameObject) {
            if (!ContainsLayer(_includeLayers, gameObject.layer)) return;

            var entity = gameObject.GetComponent<IEntityView>()?.Entity ?? default;
            if (!entity.IsAlive()) return;

            OnTriggerExit.Invoke(entity);
        }

        public static bool ContainsLayer(LayerMask mask, int layer) {
            return mask == (mask | (1 << layer));
        }
    }

}
