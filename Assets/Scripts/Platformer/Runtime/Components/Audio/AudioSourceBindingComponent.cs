using System;
using Entities.Core;
using UnityEngine;

namespace Platformer.Components {

    [Serializable]
    public sealed class AudioSourceBindingComponent : IEntityComponent {
        public AudioSource audioSource;
    }

}
