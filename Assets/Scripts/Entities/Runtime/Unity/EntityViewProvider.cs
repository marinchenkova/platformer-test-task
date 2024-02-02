using Entities.Services;
using Pools.Prefabs;
using UnityEngine;

namespace Entities.Unity {

    public sealed class EntityViewProvider : MonoBehaviour, IEntityViewProvider {

        [SerializeField] private PrefabPool _prefabPool;

        public GameObject CreateView(GameObject prefab, bool active = false) {
            return _prefabPool.TakeElement(prefab, active);
        }

        public void DisposeView(GameObject view) {
            _prefabPool.RecycleElement(view);
        }
    }

}
