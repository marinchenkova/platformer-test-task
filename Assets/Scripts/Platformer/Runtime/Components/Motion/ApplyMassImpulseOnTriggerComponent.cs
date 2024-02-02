using System;
using Entities.Core;
using Platformer.Components.Triggers;
using UnityEngine;

namespace Platformer.Components {

    [Serializable]
    public sealed class ApplyMassImpulseOnTriggerComponent : IEntityComponent {

        public TriggerEventType triggerEvent;
        public Vector3 direction;
        [Min(0f)] public float amplitude;

        private TriggerZoneComponent _triggerZoneComponent;

        public void OnEnable(Entity entity) {
            _triggerZoneComponent = entity.GetComponent<TriggerZoneComponent>();
            if (_triggerZoneComponent == null) return;

            switch (triggerEvent) {
                case TriggerEventType.Enter:
                    _triggerZoneComponent.OnTriggerEnter -= OnTriggerEvent;
                    _triggerZoneComponent.OnTriggerEnter += OnTriggerEvent;
                    break;

                case TriggerEventType.Exit:
                    _triggerZoneComponent.OnTriggerExit -= OnTriggerEvent;
                    _triggerZoneComponent.OnTriggerExit += OnTriggerEvent;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OnDisable(Entity entity) {
            if (_triggerZoneComponent != null) {
                switch (triggerEvent) {
                    case TriggerEventType.Enter:
                        _triggerZoneComponent.OnTriggerEnter -= OnTriggerEvent;
                        break;

                    case TriggerEventType.Exit:
                        _triggerZoneComponent.OnTriggerExit -= OnTriggerEvent;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            _triggerZoneComponent = null;
        }

        private void OnTriggerEvent(Entity entity) {
            entity.GetComponent<MassComponent>()?.ApplyImpulse(direction * amplitude);
        }
    }


}
