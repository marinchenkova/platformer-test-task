using System;
using Entities.Core;
using UnityEngine;

namespace Platformer.Components {

    [Serializable]
    public sealed class CameraBindingComponent : IEntityComponent {
        public Camera camera;
    }

}
