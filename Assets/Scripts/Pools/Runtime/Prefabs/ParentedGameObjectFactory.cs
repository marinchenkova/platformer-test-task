using Pools.Core;
using UnityEngine;

namespace Pools.Prefabs {

    public sealed class ParentedGameObjectFactory : IPoolFactory<GameObject> {

        private readonly Transform _parent;

        public ParentedGameObjectFactory(Transform parent) {
            _parent = parent;
        }

        public GameObject CreateElement(GameObject sample) {
            var gameObject = Object.Instantiate(sample, Vector3.zero, Quaternion.identity, _parent);
            gameObject.name = sample.name;

            return gameObject;
        }

        public void DestroyElement(GameObject element) {
            Object.Destroy(element);
        }

        public void ActivateElement(GameObject element) {
            element.SetActive(true);
        }

        public void DeactivateElement(GameObject element) {
            element.SetActive(false);
        }
    }

}
