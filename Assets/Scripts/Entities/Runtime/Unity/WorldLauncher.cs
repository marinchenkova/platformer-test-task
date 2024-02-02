using Entities.Core;
using Entities.Services;
using UnityEngine;

namespace Entities.Unity {

    public sealed class WorldLauncher : MonoBehaviour {

        [SerializeField] private WorldReference _worldReference;
        [SerializeField] private EntityViewProvider _entityViewProvider;

        private WorldTickProvider _worldTickProvider;

        private void Awake() {
            _worldTickProvider = new WorldTickProvider();
            _worldReference.World = CreateWorld();
        }

        private void OnDestroy() {
            _worldReference.World?.DestroyAll();
            _worldReference.World = null;

            _worldTickProvider.Clear();
            _worldTickProvider = null;
        }

        private void Update() {
           _worldTickProvider.Update(Time.deltaTime);
        }

        private void LateUpdate() {
            _worldTickProvider.LateUpdate(Time.deltaTime);
        }

        private void FixedUpdate() {
            _worldTickProvider.FixedUpdate(Time.fixedDeltaTime);
        }

        private World CreateWorld() {
            return new World(
                new IncrementalEntityIdProvider(),
                new EntityStorage(),
                new EntityComponentStorage(),
                new EntityComponentFactory(),
                _entityViewProvider,
                _worldTickProvider
            );
        }
    }

}
