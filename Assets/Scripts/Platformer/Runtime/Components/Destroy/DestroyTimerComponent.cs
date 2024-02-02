using System;
using Entities.Core;

namespace Platformer.Components {

    [Serializable]
    public sealed class DestroyTimerComponent : IEntityComponent, IUpdatable {

        public TickGroup updateAt;
        public float delay;

        private float _timer;
        private Entity _entity;

        void IEntityComponent.OnEnable(Entity entity) {
            _entity = entity;
            _entity.world.Subscribe(this, updateAt);
        }

        void IEntityComponent.OnDisable(Entity entity) {
            _entity.world.Unsubscribe(this, updateAt);
            _entity = default;
        }

        void IUpdatable.OnUpdate(float dt) {
            if (!_entity.IsAlive()) return;

            _timer += dt;
            if (_timer >= delay) _entity.Destroy();
        }
    }

}
