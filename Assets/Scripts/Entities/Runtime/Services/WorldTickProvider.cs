using System;
using System.Collections.Generic;
using Entities.Core;
using Entities.Runtime.Services;
using UnityEngine;

namespace Entities.Services {

    public sealed class WorldTickProvider : IWorldTickProvider {

        private readonly UpdateGroup _update = new UpdateGroup(0);
        private readonly UpdateGroup _lateUpdate = new UpdateGroup(0);
        private readonly UpdateGroup _fixedUpdate = new UpdateGroup(0);
        private readonly UpdateGroup _lateFixedUpdate = new UpdateGroup(0);

        public void Subscribe(IUpdatable updatable, TickGroup tickGroup) {
            switch (tickGroup) {
                case TickGroup.Update:
                    _update.Add(updatable);
                    break;

                case TickGroup.LateUpdate:
                    _lateUpdate.Add(updatable);
                    break;

                case TickGroup.FixedUpdate:
                    _fixedUpdate.Add(updatable);
                    break;

                case TickGroup.LateFixedUpdate:
                    _lateFixedUpdate.Add(updatable);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(tickGroup), tickGroup, null);
            }
        }

        public void Unsubscribe(IUpdatable updatable, TickGroup tickGroup) {
            switch (tickGroup) {
                case TickGroup.Update:
                    _update.Remove(updatable);
                    break;

                case TickGroup.LateUpdate:
                    _lateUpdate.Remove(updatable);
                    break;

                case TickGroup.FixedUpdate:
                    _fixedUpdate.Remove(updatable);
                    break;

                case TickGroup.LateFixedUpdate:
                    _lateFixedUpdate.Remove(updatable);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(tickGroup), tickGroup, null);
            }
        }

        public void Update(float dt) {
            _update.Update(dt);
        }

        public void LateUpdate(float dt) {
            _lateUpdate.Update(dt);
        }

        public void FixedUpdate(float dt) {
            _fixedUpdate.Update(dt);
            _lateFixedUpdate.Update(dt);
        }

        public void Clear() {
            _update.Clear();
            _lateUpdate.Clear();
            _fixedUpdate.Clear();
            _lateFixedUpdate.Clear();
        }

        private readonly struct UpdateGroup {

            private readonly HashSet<IUpdatable> _listen;
            private readonly HashSet<IUpdatable> _add;
            private readonly HashSet<IUpdatable> _remove;

            public UpdateGroup(int capacity) {
                _listen = new HashSet<IUpdatable>(capacity);
                _remove = new HashSet<IUpdatable>();
                _add = new HashSet<IUpdatable>();
            }

            public void Add(IUpdatable updatable) {
                _add.Add(updatable);
                _remove.Remove(updatable);
            }

            public void Remove(IUpdatable updatable) {
                _add.Remove(updatable);
                _remove.Add(updatable);
            }

            public void Update(float dt) {
                foreach (var updatable in _listen) {
                    updatable.OnUpdate(dt);
                }

                UpdateListeners();
            }

            public void Clear() {
                _listen.Clear();
                _add.Clear();
                _remove.Clear();
            }

            private void UpdateListeners() {
                foreach (var updatable in _add) {
                    _listen.Add(updatable);
                }

                foreach (var updatable in _remove) {
                    _listen.Remove(updatable);
                }

                _add.Clear();
                _remove.Clear();
            }
        }
    }

}
