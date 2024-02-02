using System;
using Entities.Core;

namespace Platformer.Components {

    [Serializable]
    public sealed class PlayerCameraSettingsComponent : IEntityComponent {

        public float initialCameraZoom;

        public float Zoom { get; set; }

        public void OnEnable(Entity entity) {
            Zoom = initialCameraZoom;
        }

    }

}
