using System;
using Entities.Core;
using UnityEngine;

namespace Platformer.Components {
    
    [Serializable]
    public sealed class SyncCharacterControllerComponent : IEntityComponent, IUpdatable {

        public TickGroup updateAt;

        private Entity _entity;
        private CharacterController _characterController;
        private Transform _transform;

        void IEntityComponent.OnEnable(Entity entity) {
            _entity = entity;
            _characterController = entity.GetComponent<CharacterControllerBindingComponent>()?.characterController;

            if (_characterController == null) return;

            _transform = _characterController.transform;

            // Fetch current position
            _characterController.enabled = false;
            var transformComponent = _entity.GetOrAddComponent<TransformComponent>();
            _transform.position = transformComponent.position;
            _characterController.enabled = true;

            entity.world.Subscribe(this, updateAt);
        }

        void IEntityComponent.OnDisable(Entity entity) {
            _entity.world.Unsubscribe(this, updateAt);
            _characterController = null;
            _transform = null;
            _entity = default;
        }

        void IUpdatable.OnUpdate(float dt) {
            if (!_entity.IsAlive()) return;

            var transformComponent = _entity.GetOrAddComponent<TransformComponent>();
            var velocity = _entity.GetComponent<VelocityComponent>()?.velocity ?? Vector3.zero;

            _characterController.Move(velocity * dt);
            transformComponent.position = _transform.position;

            _transform.rotation = transformComponent.rotation;
            _transform.localScale = transformComponent.scale;
        }
    }
    
}
