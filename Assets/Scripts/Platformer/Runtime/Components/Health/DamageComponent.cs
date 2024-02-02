using System;
using Entities.Core;

namespace Platformer.Components {
    
    [Serializable]
    public sealed class DamageComponent : IEntityComponent {

        public float damage;
        public bool death;

        public void TryApplyDamage(Entity entity) {
            var healthComponent = entity.GetComponent<HealthComponent>();
            if (healthComponent == null) return;

            if (death) healthComponent.ApplyDeath();
            else healthComponent.ApplyDamage(damage);
        }
    }
    
}
