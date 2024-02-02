using System;
using Entities.Core;
using Entities.Unity;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Platformer.Components {

    [Serializable]
    public sealed class GameStateComponent : IEntityComponent, IUpdatable {

        [SerializeField] private InputActionsLauncher _inputActionsLauncher;
        [SerializeField] private EntityBuilder _playerBuilder;
        public Transform playerSpawnPoint;
        public float initialCameraZoom;

        public event Action<Entity> OnPlayerRespawned = delegate {  };
        public event Action OnPlayerDestroyed = delegate {  };

        public Entity Player { get; private set; }
        public float PlayerCameraZoom { get; set; }

        private Entity _entity;

        void IEntityComponent.OnEnable(Entity entity) {
            _entity = entity;

            _inputActionsLauncher.InputActions.Menu.AnyKey.performed -= OnAnyKeyPressed;
            _inputActionsLauncher.InputActions.Menu.AnyKey.performed += OnAnyKeyPressed;

            PlayerCameraZoom = initialCameraZoom;
        }

        void IEntityComponent.OnDisable(Entity entity) {
            DestroyPlayer();

            if (_inputActionsLauncher != null && _inputActionsLauncher.InputActions != null) {
                _inputActionsLauncher.InputActions.Menu.AnyKey.performed -= OnAnyKeyPressed;
            }

            _entity.world.Unsubscribe(this, TickGroup.Update);
            _entity = default;
        }

        void IUpdatable.OnUpdate(float dt) {
            if (!_entity.IsAlive()) return;

            if (Player.IsAlive()) return;

            _inputActionsLauncher.InputActions.Menu.AnyKey.performed -= OnAnyKeyPressed;
            _inputActionsLauncher.InputActions.Menu.AnyKey.performed += OnAnyKeyPressed;

            OnPlayerDestroyed.Invoke();

            _entity.world.Unsubscribe(this, TickGroup.Update);
        }

        private void OnAnyKeyPressed(InputAction.CallbackContext ctx) {
            RespawnPlayer();
            _inputActionsLauncher.InputActions.Menu.AnyKey.performed -= OnAnyKeyPressed;
        }

        private void RespawnPlayer() {
            DestroyPlayer();

            Player = _entity.world.CreateEntity();

            Player.SetComponent(new GameStateBindingComponent { gameState = _entity });

            var transformComponent = Player.GetOrAddComponent<TransformComponent>();
            transformComponent.position = playerSpawnPoint == null ? Vector3.zero : playerSpawnPoint.position;

            _playerBuilder.CopyComponentsInto(Player);

            OnPlayerRespawned.Invoke(Player);

            _entity.world.Subscribe(this, TickGroup.Update);
        }

        private void DestroyPlayer() {
            if (!Player.IsAlive()) return;

            Player.Destroy();
            Player = default;

            OnPlayerDestroyed.Invoke();
        }
    }

}
