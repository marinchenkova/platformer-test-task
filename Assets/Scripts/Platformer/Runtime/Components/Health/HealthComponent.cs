using System;
using Entities.Core;
using UnityEngine;

namespace Platformer.Components {

    [Serializable]
    public sealed class HealthComponent : IEntityComponent {

        public float health;
        public bool destroyOnZeroHealth;
        public float destroyDelay;

        public event Action<float> OnTakeDamage = delegate {  };
        public event Action OnDeath = delegate {  };

        private Entity _entity;

        void IEntityComponent.OnEnable(Entity entity) {
            _entity = entity;
        }

        void IEntityComponent.OnDisable(Entity entity) {
            _entity = default;
        }

        public void ApplyDamage(float damage) {
            if (damage <= 0f || health <= 0f || !_entity.IsAlive()) return;

            health = Mathf.Max(0f, health - damage);

            OnTakeDamage.Invoke(damage);
            CheckDeath();
        }

        public void ApplyDeath() {
            if (health <= 0f || !_entity.IsAlive()) return;

            float damage = health;
            health = 0f;

            OnTakeDamage.Invoke(damage);
            CheckDeath();
        }

        private void CheckDeath() {
            if (health > 0f) return;

            OnDeath.Invoke();

            if (!destroyOnZeroHealth) return;

            if (destroyDelay > 0f) _entity.SetComponent(new DestroyTimerComponent { delay = destroyDelay });
            else _entity.Destroy();
        }
    }

}
