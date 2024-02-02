using System;
using System.Runtime.CompilerServices;
using Entities.Core;

namespace Platformer.Components {

    [Serializable]
    public sealed class SyncViewTransformComponent : IEntityComponent, IUpdatable {

        public TickGroup updateAt;

        private Entity _entity;

        void IEntityComponent.OnEnable(Entity entity) {
            _entity = entity;
            FetchTransform(_entity);

            _entity.world.Subscribe(this, updateAt);
        }

        void IEntityComponent.OnDisable(Entity entity) {
            _entity.world.Unsubscribe(this, updateAt);
            _entity = default;
        }

        void IUpdatable.OnUpdate(float dt) {
            if (!_entity.IsAlive()) return;

            FetchTransform(_entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FetchTransform(Entity entity) {
            var viewBindingComponent = entity.GetComponent<ViewBindingComponent>();
            if (viewBindingComponent == null) return;

            var transformComponent = entity.GetOrAddComponent<TransformComponent>();
            var transform = viewBindingComponent.Transform;

            transform.SetPositionAndRotation(transformComponent.position, transformComponent.rotation);
            transform.localScale = transformComponent.scale;
        }
    }

}
