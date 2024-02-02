using System;
using System.Collections.Generic;
using Entities.Core;
using UnityEngine;

namespace Platformer.Components {

    [Serializable]
    public sealed class CollisionsBufferComponent : IEntityComponent {

        public IReadOnlyList<CollisionInfo> Hits => GetHits();

        private readonly HashSet<ICollisionDetector> _detectors = new HashSet<ICollisionDetector>();
        private readonly List<CollisionInfo> _hits = new List<CollisionInfo>();
        private int _lastUpdateFrame;

        public void Register(ICollisionDetector detector) {
            _detectors.Add(detector);
        }

        public void Unregister(ICollisionDetector detector) {
            _detectors.Remove(detector);
        }

        private IReadOnlyList<CollisionInfo> GetHits() {
            int frame = Time.frameCount;
            if (frame == _lastUpdateFrame) return _hits;

            _hits.Clear();
            _lastUpdateFrame = frame;

            foreach (var detector in _detectors) {
                detector.FetchCollisions(_hits);
            }

            return _hits;
        }
    }

}
