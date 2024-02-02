using System;
using System.Collections.Generic;
using Entities.Core;
using Platformer.Utils;
using UnityEngine;

namespace Platformer.Components {

    [Serializable]
    public sealed class CharacterControllerCollisionDetectorComponent : IEntityComponent, ICollisionDetector {

        [Header("Raycast Settings")]
        [SerializeField] [Min(1)] private int _maxHits = 6;
        [SerializeField] [Min(0f)] private float _radiusAdd;
        [SerializeField] [Min(0f)] private float _distanceAdd;
        [SerializeField] private Vector3 _direction = Vector3.down;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private QueryTriggerInteraction _queryTriggerInteraction;

        private CharacterController _characterController;
        private Transform _transform;
        private RaycastHit[] _hits;

        void IEntityComponent.OnEnable(Entity entity) {
            _hits = new RaycastHit[_maxHits];

            _characterController = entity.GetComponent<CharacterControllerBindingComponent>()?.characterController;
            if (_characterController != null) _transform = _characterController.transform;

            entity.GetOrAddComponent<CollisionsBufferComponent>().Register(this);
        }

        void IEntityComponent.OnDisable(Entity entity) {
            _hits = null;
            _transform = null;
            _characterController = null;

            entity.GetComponent<CollisionsBufferComponent>()?.Unregister(this);
        }

        public void FetchCollisions(ICollection<CollisionInfo> dest) {
            var origin = _characterController.center + _transform.position;
            float radiusAdd = _characterController.skinWidth + _radiusAdd;
            float radius = _characterController.radius + radiusAdd;
            float halfDistance = _characterController.height * 0.5f - _characterController.radius + radiusAdd + _distanceAdd;
            var direction = _transform.rotation * _direction;

            int hits = Physics.SphereCastNonAlloc(origin, radius, direction, _hits, halfDistance, _layerMask, _queryTriggerInteraction);
            int selfHash = _transform.GetHashCode();

            for (int i = 0; i < hits; i++) {
                var hit = _hits[i];

                if (hit.transform.GetHashCode() == selfHash ||
                    hit.distance <= 0f && hit.point.IsNearlyZero() // such hits are mostly invalid
                ) {
                    continue;
                }

                dest.Add(new CollisionInfo(hit.transform, hit.point, hit.normal, hit.distance, _transform.SearchEntityOnEntityView()));
            }
        }
    }

}
