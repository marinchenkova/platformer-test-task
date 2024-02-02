using Entities.Core;
using UnityEngine;

namespace Platformer.Components {

    public readonly struct CollisionInfo {

        public readonly bool hasContact;

        public readonly Entity entity;
        public readonly Transform transform;

        public readonly Vector3 point;
        public readonly Vector3 normal;
        public readonly float distance;

        public CollisionInfo(
            Transform transform,
            Vector3 point,
            Vector3 normal,
            float distance,
            Entity entity = default,
            bool hasContact = true
        ) {
            this.entity = entity;
            this.transform = transform;
            this.point = point;
            this.normal = normal;
            this.distance = distance;
            this.hasContact = hasContact;
        }
    }

}
