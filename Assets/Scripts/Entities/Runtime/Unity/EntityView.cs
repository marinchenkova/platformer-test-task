using Entities.Core;
using UnityEngine;

namespace Entities.Unity {

    public sealed class EntityView : MonoBehaviour, IEntityView {

        [SerializeReference] private IEntityComponent[] _viewComponents;

        public Entity Entity { get; private set; }

        public void Bind(Entity entity) {
            Entity = entity;

            if (_viewComponents != null) {
                for (int i = 0; i < _viewComponents.Length; i++) {
                    entity.SetComponentCopy(_viewComponents[i]);
                }
            }
        }

        public void Unbind() {
            Entity = default;
        }
    }

}
