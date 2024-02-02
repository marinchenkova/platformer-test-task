using System;
using Entities.Core;
using UnityEngine;

namespace Platformer.Components {

    [Serializable]
    public sealed class PlayerCameraComponent : IEntityComponent, IUpdatable {

        public TickGroup updateAt;
        public float zoomSmoothing = 1f;
        public float zoomSensitivity = 1f;
        public Vector2 zoomVector = Vector2.one;
        public float minZoom = 1;
        public float maxZoom = 10;

        private Entity _entity;
        private float _targetZoom;
        private float _currentZoom;

        void IEntityComponent.OnEnable(Entity entity) {
            _entity = entity;

            var playerInputComponent = _entity.GetComponent<PlayerInputComponent>();

            if (playerInputComponent != null) {
                playerInputComponent.OnZoomChanged -= OnZoomChanged;
                playerInputComponent.OnZoomChanged += OnZoomChanged;
            }

            var gameState = _entity.GetComponent<GameStateBindingComponent>()?.gameState ?? default;
            if (gameState.GetComponent<PlayerCameraSettingsComponent>() is { } playerCameraSettingsComponent) {
                _targetZoom = Mathf.Clamp(playerCameraSettingsComponent.Zoom, minZoom, maxZoom);
                _currentZoom = _targetZoom;
            }

            _entity.world.Subscribe(this, updateAt);

            ApplyZoom(_entity, _currentZoom);
        }

        void IEntityComponent.OnDisable(Entity entity) {
            _entity.world.Unsubscribe(this, updateAt);

            var playerInputComponent = _entity.GetComponent<PlayerInputComponent>();

            if (playerInputComponent != null) {
                playerInputComponent.OnZoomChanged -= OnZoomChanged;
                playerInputComponent.OnZoomChanged += OnZoomChanged;
            }
        }

        void IEntityComponent.OnDestroy(Entity entity) {
            var gameState = _entity.GetComponent<GameStateBindingComponent>()?.gameState ?? default;

            if (gameState.GetComponent<PlayerCameraSettingsComponent>() is { } playerCameraSettingsComponent) {
                playerCameraSettingsComponent.Zoom = _targetZoom;
            }

            _entity = default;
        }

        public void OnUpdate(float dt) {
            if (!_entity.IsAlive()) return;

            _currentZoom = Mathf.Lerp(_currentZoom, _targetZoom, dt * zoomSmoothing);

            ApplyZoom(_entity, _currentZoom);
        }

        private void OnZoomChanged(float delta) {
            _targetZoom = Mathf.Clamp(_targetZoom + delta * zoomSensitivity, minZoom, maxZoom);
        }

        private void ApplyZoom(Entity entity, float zoom) {
            if (entity.GetComponent<CameraFollowingComponent>() is not {} cameraFollowingComponent) return;

            var offset = zoom * zoomVector;
            cameraFollowingComponent.horizontalOffset = offset.x;
            cameraFollowingComponent.verticalOffset = offset.y;
        }
    }

}
