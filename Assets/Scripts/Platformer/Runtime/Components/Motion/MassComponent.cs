using System;
using System.Collections.Generic;
using Entities.Core;
using Platformer.Utils;
using UnityEngine;

namespace Platformer.Components {

    [Serializable]
    public sealed class MassComponent : IEntityComponent, IUpdatable {

        public TickGroup updateAt;

        [Header("Inertia")]
        public float airInertialFactor = 1f;
        public float groundInertialFactor = 1f;
        public float inputInfluenceFactor = 1f;

        [Header("Gravity")]
        public float gravityForce = 9.8f;
        public bool isGravityEnabled = true;

        public event Action OnFall = delegate {  };
        public event Action OnLand = delegate {  };

        public Vector3 Input { get; set; }
        public Vector3 Velocity { get; private set; }
        public bool IsGrounded { get; private set; }

        private readonly Dictionary<object, Vector3> _forceMap = new Dictionary<object, Vector3>();

        private Entity _entity;
        private Vector3 _inertialComponent;
        private Vector3 _gravitationalComponent;
        private Vector3 _forceComponent;
        private Vector3 _lastGroundNormal;

        void IEntityComponent.OnEnable(Entity entity) {
            _entity = entity;
            _entity.world.Subscribe(this, updateAt);
        }

        void IEntityComponent.OnDisable(Entity entity) {
            _entity.world.Unsubscribe(this, updateAt);
            _entity = default;
        }

        void IUpdatable.OnUpdate(float dt) {
            if (!_entity.IsAlive()) return;

            ProcessCollisions();

            UpdateInertialComponent(Input, dt);
            UpdateGravitationalComponent(dt);
            UpdateForceComponent();

            Velocity = _gravitationalComponent + _inertialComponent + _forceComponent;

            _entity.GetOrAddComponent<VelocityComponent>().velocity = Velocity;
        }

        public void ApplyImpulse(Vector3 impulse) {
            _inertialComponent += impulse.WithY(0f);
            _gravitationalComponent.y += impulse.y;
        }

        public void ApplyVelocityChange(Vector3 impulse) {
            _inertialComponent = impulse.WithY(0f);
            _gravitationalComponent.y = impulse.y;
        }

        public void ApplyForceSource(object source, Vector3 force) {
            _forceMap[source] = force;
        }

        public void RemoveForceSource(object source) {
            _forceMap.Remove(source);
        }

        private void ProcessCollisions() {
            bool wasGrounded = IsGrounded;
            bool isGrounded = false;

            var hits = _entity.GetComponent<CollisionsBufferComponent>()?.Hits ?? Array.Empty<CollisionInfo>();
            CollisionInfo nearestCollision = default;
            float minDistance = float.MaxValue;

            for (int i = 0; i < hits.Count; i++) {
                var collisionInfo = hits[i];
                if (collisionInfo.distance >= minDistance) continue;

                minDistance = collisionInfo.distance;
                nearestCollision = collisionInfo;
                isGrounded = true;
            }

            IsGrounded = isGrounded;

            if (isGrounded && !nearestCollision.normal.IsNearlyEqual(_lastGroundNormal)) {
                _lastGroundNormal = nearestCollision.hasContact ? nearestCollision.normal : Vector3.up;
                _inertialComponent = Vector3.ProjectOnPlane(_inertialComponent, nearestCollision.normal);
            }

            if (wasGrounded && !isGrounded) OnFall.Invoke();
            else if (!wasGrounded && isGrounded) OnLand.Invoke();
        }

        /// <summary>
        /// Interpolates value of the inertial component towards current force vector
        /// with ground or in-air inertial factor.
        /// </summary>
        private void UpdateInertialComponent(Vector3 input, float dt) {
            float factor = IsGrounded
                ? groundInertialFactor
                : airInertialFactor * GetInputInfluence(input, _inertialComponent, inputInfluenceFactor);

            _inertialComponent = Vector3.Lerp(_inertialComponent, input, factor * dt);
        }

        /// <summary>
        /// Interpolates value of the gravitational component:
        ///
        /// 1) If gravity is enabled and character is not grounded (i.e. is falling down) -
        ///    gravitational component is increased by gravity force per frame.
        ///
        /// 2) If gravity is not enabled or character is grounded -
        ///    gravitational component is interpolated towards Vector3.zero with ground or in-air inertial factor.
        ///
        /// </summary>
        private void UpdateGravitationalComponent(float dt) {
            if (isGravityEnabled && !IsGrounded) {
                _gravitationalComponent += gravityForce * dt * Vector3.down;
                return;
            }

            float factor = IsGrounded ? groundInertialFactor : airInertialFactor;
            _gravitationalComponent = Vector3.Lerp(_gravitationalComponent, Vector3.zero, factor * dt);
        }

        private void UpdateForceComponent() {
            var targetForce = Vector3.zero;
            foreach (var force in _forceMap.Values) {
                targetForce += force;
            }

            _forceComponent = targetForce;
        }

        /// <summary>
        /// Input influence allows to save the bigger amount of the inertial energy while not grounded,
        /// the greater the inertial component value compared to input value:
        ///
        /// 1) Value is a relation of input magnitude to inertial component magnitude,
        ///    if inertial component is bigger than input component.
        ///
        /// 2) Value is 1f (no influence),
        ///    if inertial component is equal or less than input component.
        ///
        /// </summary>
        private static float GetInputInfluence(Vector3 input, Vector3 inertialComponent, float multiplier) {
            float inertialSqrMagnitude = inertialComponent.sqrMagnitude;
            float forceSqrMagnitude = input.sqrMagnitude;

            return inertialSqrMagnitude > forceSqrMagnitude
                ? multiplier * forceSqrMagnitude / inertialSqrMagnitude
                : 1f;
        }
    }

}
