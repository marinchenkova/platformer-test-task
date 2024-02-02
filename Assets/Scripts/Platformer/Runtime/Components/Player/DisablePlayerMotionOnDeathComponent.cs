using System;
using Entities.Core;
using UnityEngine;

namespace Platformer.Components {

    [Serializable]
    public sealed class DisablePlayerMotionOnDeathComponent : IEntityComponent {

        public float setOrientationAngle = 90f;

        private Entity _entity;

        void IEntityComponent.OnEnable(Entity entity) {
            _entity = entity;

            var healthComponent = entity.GetComponent<HealthComponent>();
            if (healthComponent == null) return;

            healthComponent.OnDeath -= OnDeath;
            healthComponent.OnDeath += OnDeath;
        }

        void IEntityComponent.OnDisable(Entity entity) {
            var healthComponent = entity.GetComponent<HealthComponent>();
            if (healthComponent != null) healthComponent.OnDeath -= OnDeath;

            _entity = default;
        }

        private void OnDeath() {
            _entity.RemoveComponent<PlayerInputComponent>();
            _entity.RemoveComponent<PlayerMotionComponent>();
            _entity.RemoveComponent<PlayerPoseComponent>();

            if (_entity.GetComponent<MassComponent>() is { } massComponent) {
                massComponent.Input = Vector3.zero;
            }

            _entity.GetComponent<TransformComponent>().rotation = Quaternion.AngleAxis(setOrientationAngle, Vector3.up);
        }
    }

}
