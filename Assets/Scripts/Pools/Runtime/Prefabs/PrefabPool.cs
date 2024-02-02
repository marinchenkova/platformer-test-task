using System;
using System.Collections.Generic;
using Pools.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Pools.Prefabs {

    public sealed class PrefabPool : MonoBehaviour, ISampleObjectPool<GameObject> {

        [Header("Default pool settings")]
        [SerializeField] [Min(0)] private int _initialCapacity = 0;
        [SerializeField] [Range(0f, 1f)] private float _ensureCapacityAt = 0.3f;
        [SerializeField] [Range(1f, 3f)] private float _ensureCapacityCoeff = 1.7f;

        [Header("Pools")]
        [SerializeField] private PoolConfig[] _preInitializedPools;

        [Serializable]
        private struct PoolConfig {
            public GameObject prefab;
            [Min(0)] public int initialCapacity;
            [Range(0f, 1f)] public float ensureCapacityAt;
            [Range(1f, 3f)] public float ensureCapacityCoeff;
        }

        private Dictionary<int, ObjectPool<GameObject>> _pools;
        private IPoolFactory<GameObject> _factory;

        private void Awake() {
            _factory = new ParentedGameObjectFactory(transform);

            int poolsCount = _preInitializedPools.Length;
            _pools = new Dictionary<int, ObjectPool<GameObject>>(poolsCount);

            for (int i = 0; i < poolsCount; i++) {
                var config = _preInitializedPools[i];

                var pool = new ObjectPool<GameObject>(_factory, config.prefab);
                pool.Initialize(config.ensureCapacityAt, config.ensureCapacityCoeff, config.initialCapacity);

                int prefabId = GetPrefabId(config.prefab);
                _pools[prefabId] = pool;
            }
        }

        private void OnDestroy() {
            foreach (var pool in _pools.Values) {
                pool.Clear();
            }

            _pools.Clear();
        }

        public GameObject TakeElement(GameObject prefab, bool active = false) {
            int prefabId = GetPrefabId(prefab);

            if (!_pools.TryGetValue(prefabId, out var pool)) {
                pool = new ObjectPool<GameObject>(_factory, prefab);
                pool.Initialize(_ensureCapacityAt, _ensureCapacityCoeff, _initialCapacity);

                _pools[prefabId] = pool;
            }

            return pool.TakeElement(active);
        }

        public void RecycleElement(GameObject element) {
            int prefabId = GetPrefabId(element);

            if (_pools.TryGetValue(prefabId, out var pool)) {
                pool.RecycleElement(element);
                return;
            }

            _factory.DestroyElement(element);
        }

        private static int GetPrefabId(Object prefab) {
            return prefab.name.GetHashCode();
        }
    }

}
