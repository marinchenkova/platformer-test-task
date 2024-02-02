using System;
using Entities.Core;
using UnityEngine;

namespace Platformer.Components {

    [Serializable]
    public sealed class TransformComponent : IEntityComponent {

        public Vector3 position;
        [SerializeField] private Vector3 _rotation;
        public Vector3 scale = Vector3.one;

        public Quaternion rotation { get => Quaternion.Euler(_rotation); set => _rotation = value.eulerAngles; }
    }

}
