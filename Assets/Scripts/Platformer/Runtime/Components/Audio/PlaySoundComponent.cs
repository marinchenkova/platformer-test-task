using System;
using Entities.Core;
using UnityEngine;

namespace Platformer.Components {

    [Serializable]
    public sealed class PlaySoundComponent : IEntityComponent {

        public ComponentEventType onEvent;
        public AudioClip audioClip;
        public float volume;
        public bool loop;

        public enum ComponentEventType {
            OnEnable,
            OnDisable,
            OnDestroy,
        }

        void IEntityComponent.OnEnable(Entity entity) {
            if (onEvent == ComponentEventType.OnEnable) PlaySound(entity);
        }

        void IEntityComponent.OnDisable(Entity entity) {
            if (onEvent == ComponentEventType.OnDisable) PlaySound(entity);
        }

        void IEntityComponent.OnDestroy(Entity entity) {
            if (onEvent == ComponentEventType.OnDestroy) PlaySound(entity);
        }

        private void PlaySound(Entity entity) {
            if (entity.GetComponent<AudioSourceBindingComponent>() is not {} audioSourceBindingComponent) return;

            var audioSource = audioSourceBindingComponent.audioSource;

            if (loop) {
                audioSource.volume = volume;
                audioSource.clip = audioClip;
                audioSource.loop = true;
                audioSource.Play();
            }
            else {
                audioSource.PlayOneShot(audioClip, volume);
            }
        }
    }

}
