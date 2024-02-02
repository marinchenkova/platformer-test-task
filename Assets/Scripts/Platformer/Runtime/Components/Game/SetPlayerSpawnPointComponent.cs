using System;
using Entities.Core;
using Entities.Unity;
using UnityEngine;

namespace Platformer.Components {

    [Serializable]
    public sealed class SetPlayerSpawnPointComponent : IEntityComponent {

        public EntitySource gameStateSource;
        public Transform spawnPoint;

        void IEntityComponent.OnEnable(Entity entity) {
            if (gameStateSource.Entity.GetComponent<GameStateComponent>() is { } gameStateComponent) {
                gameStateComponent.playerSpawnPoint = spawnPoint;
            }
        }
    }

}
