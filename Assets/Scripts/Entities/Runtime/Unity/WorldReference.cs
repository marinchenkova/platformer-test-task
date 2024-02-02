using Entities.Core;
using UnityEngine;

namespace Entities.Unity {

    [CreateAssetMenu(fileName = nameof(WorldReference), menuName = "Entities/" + nameof(WorldReference))]
    public sealed class WorldReference : ScriptableObject {

        public World World { get; internal set; }

    }

}
