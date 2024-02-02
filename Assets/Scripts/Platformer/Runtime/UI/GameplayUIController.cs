using Entities.Core;
using Entities.Unity;
using Platformer.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Platformer.UI {

    public sealed class GameplayUIController : MonoBehaviour {

        [SerializeField] private EntitySource _gameStateEntitySource;
        [SerializeField] private TMP_Text _textPressAnyKey;
        [SerializeField] private Image _imageControls;

        private GameStateComponent _gameStateComponent;

        private void OnEnable() {
            if (_gameStateEntitySource.Entity.IsAlive()) {
                OnGameStateEntityCreated(_gameStateEntitySource.Entity);
                return;
            }

            _gameStateEntitySource.OnEntityCreated += OnGameStateEntityCreated;
        }

        private void OnDisable() {
            _gameStateEntitySource.OnEntityCreated -= OnGameStateEntityCreated;

            if (_gameStateComponent != null) {
                _gameStateComponent.OnPlayerDestroyed -= OnPlayerDestroyed;
                _gameStateComponent.OnPlayerRespawned -= OnPlayerRespawned;
            }
        }

        private void OnGameStateEntityCreated(Entity entity) {
            _gameStateComponent = entity.GetComponent<GameStateComponent>();
            if (_gameStateComponent == null) return;

            _gameStateComponent.OnPlayerDestroyed -= OnPlayerDestroyed;
            _gameStateComponent.OnPlayerRespawned -= OnPlayerRespawned;

            _gameStateComponent.OnPlayerDestroyed += OnPlayerDestroyed;
            _gameStateComponent.OnPlayerRespawned += OnPlayerRespawned;

            if (_gameStateComponent.Player.IsAlive()) OnPlayerRespawned(_gameStateComponent.Player);
            else OnPlayerDestroyed();
        }

        private void OnPlayerRespawned(Entity player) {
            _textPressAnyKey.enabled = false;
            _imageControls.enabled = false;
        }

        private void OnPlayerDestroyed() {
            _textPressAnyKey.enabled = true;
            _imageControls.enabled = true;
        }
    }

}
