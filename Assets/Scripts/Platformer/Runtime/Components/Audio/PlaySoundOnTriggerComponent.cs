using System;
using Entities.Core;
using Platformer.Components.Triggers;
using UnityEngine;

namespace Platformer.Components {

    [Serializable]
    public sealed class PlaySoundOnTriggerComponent : IEntityComponent {

        public AudioClip audioClip;
        public float volume;
        public TriggerEventType triggerEvent;
        public bool destroyComponentAfter;

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

            _entity = default;
            _triggerZoneComponent = null;
        }

        private void OnTriggerEvent(Entity entity) {
            if (entity.GetComponent<AudioSourceBindingComponent>() is not {} audioSourceBindingComponent) return;

            audioSourceBindingComponent.audioSource.PlayOneShot(audioClip, volume);
            if (destroyComponentAfter) _entity.RemoveComponent<PlaySoundOnTriggerComponent>();
        }
    }

}
