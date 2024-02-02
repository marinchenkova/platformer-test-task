using System;
using Entities.Core;
using UnityEngine;

namespace Platformer.Components {

    [Serializable]
    public sealed class CharacterControllerBindingComponent : IEntityComponent {

        public CharacterController characterController;
    }

}
