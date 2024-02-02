using System;
using Entities.Core;
using UnityEngine;

namespace Platformer.Components {

    [Serializable]
    public sealed class ViewBindingComponent : IEntityComponent {

        [SerializeField] private Transform _transform;
        public bool useGameObjectDirectly;
        public bool applyViewPositionOnBind;

        public Transform Transform { get; set; }

        private IEntityView _entityView;

        void IEntityComponent.OnEnable(Entity entity) {
            Transform = useGameObjectDirectly
                ? _transform
                : entity.world.CreateView(_transform.gameObject, active: false).transform;

            _entityView = Transform.GetComponent<IEntityView>();
            _entityView?.Bind(entity);

            var transformComponent = entity.GetOrAddComponent<TransformComponent>();

            if (applyViewPositionOnBind) {
                transformComponent.position = Transform.position;
                transformComponent.rotation = Transform.rotation;
                transformComponent.scale = Transform.localScale;
            }
            else {
                Transform.SetPositionAndRotation(transformComponent.position, transformComponent.rotation);
                Transform.localScale = transformComponent.scale;
            }

            Transform.gameObject.SetActive(true);
        }

        void IEntityComponent.OnDisable(Entity entity) {
            _entityView?.Unbind();
            if (Transform != null) entity.world.DisposeView(Transform.gameObject);

            Transform = null;
            _entityView = null;
        }
    }

}
