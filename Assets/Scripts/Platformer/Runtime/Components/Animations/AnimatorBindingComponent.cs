using System;
using Entities.Core;
using UnityEngine;

namespace Platformer.Components {

    [Serializable]
    public sealed class AnimatorBindingComponent : IEntityComponent {
        public Animator animator;
    }

}
