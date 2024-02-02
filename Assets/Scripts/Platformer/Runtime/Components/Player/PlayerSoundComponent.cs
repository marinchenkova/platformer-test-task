using System;
using System.Collections.Generic;
using Entities.Core;
using Platformer.Utils;
using UnityEngine;

namespace Platformer.Components {

    [Serializable]
    public sealed class PlayerSoundComponent : IEntityComponent {

        public List<AudioClip> steps;
        public List<AudioClip> landings;
        public List<AudioClip> jumps;
        public AudioClip death;

        public float stepVolume;
        public float landingVolume;
        public float jumpVolume;
        public float deathVolume;

        [Min(0f)] public float landingPauseMin;

        private Entity _entity;
        private float _lastLandingSoundRequestTime;

        void IEntityComponent.OnEnable(Entity entity) {
            _entity = entity;

            var playerMotionComponent = entity.GetComponent<PlayerMotionComponent>();
            if (playerMotionComponent != null) {
                playerMotionComponent.OnJump -= OnJump;
                playerMotionComponent.OnJump += OnJump;

                playerMotionComponent.OnLand -= OnLand;
                playerMotionComponent.OnLand += OnLand;
            }

            var healthComponent = entity.GetComponent<HealthComponent>();
            if (healthComponent != null) {
                healthComponent.OnDeath -= OnDeath;
                healthComponent.OnDeath += OnDeath;
            }

            var playerStepsComponent = entity.GetComponent<PlayerStepsComponent>();
            if (playerStepsComponent != null) {
                playerStepsComponent.OnStep -= OnStep;
                playerStepsComponent.OnStep += OnStep;
            }
        }

        void IEntityComponent.OnDisable(Entity entity) {
            var playerMotionComponent = entity.GetComponent<PlayerMotionComponent>();
            if (playerMotionComponent != null) {
                playerMotionComponent.OnJump -= OnJump;
                playerMotionComponent.OnLand -= OnLand;
            }

            var healthComponent = entity.GetComponent<HealthComponent>();
            if (healthComponent != null) {
                healthComponent.OnDeath -= OnDeath;
            }

            var playerStepsComponent = entity.GetComponent<PlayerStepsComponent>();
            if (playerStepsComponent != null) {
                playerStepsComponent.OnStep -= OnStep;
            }

            _entity = default;
        }

        private void OnStep() {
            PlaySound(_entity, steps.GetRandomElement(), stepVolume);
        }

        private void OnJump(Vector3 obj) {
            PlaySound(_entity, jumps.GetRandomElement(), jumpVolume);
        }

        private void OnLand() {
            float time = Time.timeSinceLevelLoad;
            float lastTime = _lastLandingSoundRequestTime;

            if (time - lastTime < landingPauseMin) return;

            _lastLandingSoundRequestTime = time;

            PlaySound(_entity, landings.GetRandomElement(), landingVolume);
        }

        private void OnDeath() {
            PlaySound(_entity, death, deathVolume);
        }

        private static void PlaySound(Entity entity, AudioClip audioClip, float volume) {
            if (entity.GetComponent<AudioSourceBindingComponent>() is not {} audioSourceBindingComponent) return;

            audioSourceBindingComponent.audioSource.PlayOneShot(audioClip, volume);
        }
    }

}
