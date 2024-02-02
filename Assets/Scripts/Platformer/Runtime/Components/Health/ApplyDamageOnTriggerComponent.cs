using System;
using Entities.Core;
using Platformer.Components.Triggers;
using UnityEngine;

namespace Platformer.Components {

    [Serializable]
    public sealed class ApplyDamageOnTriggerComponent : IEntityComponent {

        public TriggerEventType triggerEvent;

        private TriggerZoneComponent _triggerZoneComponent;
        private Entity _entity;

        void IEntityComponent.OnEnable(Entity entity) {
            _entity = entity;
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

        void IEntityComponent.OnDisable(Entity entity) {
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
            _entity = default;
        }

        private void OnTriggerEvent(Entity entity) {
            _entity.GetComponent<DamageComponent>()?.TryApplyDamage(entity);
        }
    }


}
