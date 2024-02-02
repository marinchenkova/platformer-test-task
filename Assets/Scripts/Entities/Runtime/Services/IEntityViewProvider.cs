using UnityEngine;

namespace Entities.Services {

    public interface IEntityViewProvider {

        GameObject CreateView(GameObject prefab, bool active = false);

        void DisposeView(GameObject view);
    }

}
