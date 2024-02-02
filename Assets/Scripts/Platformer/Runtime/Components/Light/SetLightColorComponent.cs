using System;
using System.Collections.Generic;
using Entities.Core;
using UnityEngine;

namespace Platformer.Components {

    [Serializable]
    public sealed class SetLightColorComponent : IEntityComponent {

        public Color color = Color.white;
        public List<Light> lights;

        void IEntityComponent.OnEnable(Entity entity) {
            for (int i = 0; i < lights.Count; i++) {
                lights[i].color = color;
            }
        }
    }

}
