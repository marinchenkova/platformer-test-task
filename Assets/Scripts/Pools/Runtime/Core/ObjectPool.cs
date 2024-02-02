using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pools.Core {

    public sealed class ObjectPool<T> : IObjectPool<T> {

        private readonly IPoolFactory<T> _poolFactory;
        private readonly Queue<T> _elementQueue = new Queue<T>();
        private readonly T _sample;

        private float _ensureCapacityAt = 0f;
        private float _ensureCapacityCoeff = 1f;
        private int _targetCapacity = 1;

        public ObjectPool(IPoolFactory<T> poolFactory, T sample) {
            _sample = sample;
            _poolFactory = poolFactory;
        }

        public void Initialize(float ensureCapacityAt, float ensureCapacityCoeff, int initialCapacity = 0) {
            _ensureCapacityAt = ensureCapacityAt;
            _ensureCapacityCoeff = ensureCapacityCoeff;
            _targetCapacity = initialCapacity > 0 ? initialCapacity : 1;

            if (_ensureCapacityAt is < 0f or > 1f) {
                throw new ArgumentException($"Invalid parameter ensureCapacityAt = {_ensureCapacityAt}, " +
                                            $"value must be in range [0, 1]");
            }

            if (_ensureCapacityCoeff < 1f) {
                throw new ArgumentException($"Invalid parameter ensureCapacityCoeff = {_ensureCapacityCoeff}, " +
                                            $"value must be >= 1");
            }

            if (initialCapacity > 0) EnsureCapacity(initialCapacity);
        }

        public void Clear() {
            for (int i = 0; i < _elementQueue.Count; i++) {
                _poolFactory.DestroyElement(_elementQueue.Dequeue());
            }
        }

        public T TakeElement(bool active = false) {
            EnsureCapacity();

            var element = _elementQueue.Dequeue();
            if (active) _poolFactory.ActivateElement(element);

            return element;
        }

        public void RecycleElement(T element) {
            _poolFactory.DeactivateElement(element);
            _elementQueue.Enqueue(element);
        }

        private void EnsureCapacity(int maxCapacity = -1) {
            int inPoolCount = _elementQueue.Count;
            int ensureAt = Mathf.FloorToInt(_targetCapacity * _ensureCapacityAt);

            if (inPoolCount > ensureAt) return;

            int neededCapacity = Mathf.CeilToInt(_targetCapacity * _ensureCapacityCoeff);
            _targetCapacity = maxCapacity < 0 ? _targetCapacity : Math.Min(maxCapacity, neededCapacity);

            int instantiateCount = _targetCapacity - inPoolCount;
            for (int i = 0; i < instantiateCount; i++) {
                RecycleElement(_poolFactory.CreateElement(_sample));
            }
        }

        public override string ToString() {
            return $"ObjectPool<{nameof(T)}>(sample {_sample}, count {_elementQueue.Count}/{_targetCapacity})";
        }
    }

}
