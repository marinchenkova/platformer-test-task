using UnityEngine;

namespace Pools.Prefabs {

    public interface IPrefabPool {

        GameObject TakeElement(GameObject prefab, bool active = false);

        void RecycleElement(GameObject element);
    }

}
