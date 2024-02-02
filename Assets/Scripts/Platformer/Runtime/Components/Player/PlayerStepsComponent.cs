using System;
using Entities.Core;
using Platformer.Utils;
using UnityEngine;

namespace Platformer.Components {

    [Serializable]
    public sealed class PlayerStepsComponent : IEntityComponent, IUpdatable {

        public TickGroup updateAt;
        [Min(0f)] public float baseStepDuration;
        public float velocityWeight;

        public event Action OnStep = delegate {  };

        private Entity _entity;
        private float _timer;

        void IEntityComponent.OnEnable(Entity entity) {
            _entity = entity;
            _entity.world.Subscribe(this, updateAt);
        }

        void IEntityComponent.OnDisable(Entity entity) {
            _entity.world.Unsubscribe(this, updateAt);
            _entity = default;
        }

        void IUpdatable.OnUpdate(float dt) {
            var playerInputComponent = _entity.GetComponent<PlayerInputComponent>();
            if (playerInputComponent == null) return;

            if (playerInputComponent.MotionInput.IsNearlyZero()) return;

            if (_entity.GetComponent<PlayerMotionComponent>() is not { IsGrounded: true }) return;

            var velocity = _entity.GetComponent<VelocityComponent>()?.velocity ?? Vector3.zero;
            float speed = velocity.magnitude;
            float stepLength = baseStepDuration + (speed > 0f ? velocityWeight * speed : 0f);

            _timer += dt;

            if (_timer < stepLength) return;

            OnStep.Invoke();
            _timer = 0f;
        }
    }

}
