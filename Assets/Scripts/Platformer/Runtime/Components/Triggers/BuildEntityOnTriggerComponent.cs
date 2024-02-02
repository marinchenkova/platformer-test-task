using System;
using Entities.Core;
using Entities.Unity;
using Platformer.Components.Triggers;

namespace Platformer.Components {

    [Serializable]
    public sealed class BuildEntityOnTriggerComponent : IEntityComponent {

        public EntityBuilder entityBuilder;
        public TriggerEventType triggerEvent;
        public bool destroySelfAfter;

        private TriggerZoneComponent _triggerZoneComponent;
        private Entity _entity;

        void IEntityComponent.OnEnable(Entity entity) {
            _entity = entity;
            _triggerZoneComponent = entity.GetComponent<TriggerZoneComponent>();

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

            _triggerZoneComponent = null;
            _entity = default;
        }

        private void OnTriggerEvent(Entity entity) {
            var newEntity = entity.world.CreateEntity();
            entityBuilder.CopyComponentsInto(newEntity);

            if (destroySelfAfter) _entity.Destroy();
        }
    }

}
