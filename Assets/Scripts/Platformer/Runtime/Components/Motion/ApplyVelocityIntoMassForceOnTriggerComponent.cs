using System;
using Entities.Core;
using UnityEngine;

namespace Platformer.Components {

    [Serializable]
    public sealed class ApplyVelocityIntoMassForceOnTriggerComponent : IEntityComponent, IUpdatable {

        public TickGroup updateAt;

        private TriggerZoneComponent _triggerZoneComponent;
        private Entity _entity;
        private Entity _target;

        void IEntityComponent.OnEnable(Entity entity) {
            _entity = entity;

            _triggerZoneComponent = _entity.GetComponent<TriggerZoneComponent>();
            if (_triggerZoneComponent == null) return;

            _triggerZoneComponent.OnTriggerEnter -= OnTriggerEnter;
            _triggerZoneComponent.OnTriggerEnter += OnTriggerEnter;
            
            _triggerZoneComponent.OnTriggerExit -= OnTriggerExit;
            _triggerZoneComponent.OnTriggerExit += OnTriggerExit;
        }

        void IEntityComponent.OnDisable(Entity entity) {
            if (_triggerZoneComponent != null) {
                _triggerZoneComponent.OnTriggerEnter -= OnTriggerEnter;
                _triggerZoneComponent.OnTriggerExit -= OnTriggerExit;
            }

            _entity.world.Unsubscribe(this, updateAt);

            _triggerZoneComponent = null;
            _entity = default;
            _target = default;
        }

        void IUpdatable.OnUpdate(float dt) {
            if (_target.IsAlive()) ApplyVelocity(_target);
        }

        private void OnTriggerEnter(Entity entity) {
            _target = entity;
            _entity.world.Subscribe(this, updateAt);
            ApplyVelocity(_target);
        }

        private void OnTriggerExit(Entity entity) {
            entity.GetComponent<MassComponent>()?.RemoveForceSource(this);
            _target = default;
            _entity.world.Unsubscribe(this, updateAt);
        }

        private void ApplyVelocity(Entity entity) {
            var velocity = _entity.GetComponent<VelocityComponent>()?.velocity ?? Vector3.zero;
            entity.GetComponent<MassComponent>()?.ApplyForceSource(this, velocity);
        }
    }

}
