using System;
using Entities.Core;
using UnityEngine;

namespace Platformer.Components {

    [Serializable]
    public sealed class VelocityComponent : IEntityComponent {

        public Vector3 velocity;
    }

}
