using System;
using Entities.Core;
using Platformer.Components.Triggers;

namespace Platformer.Components {

    [Serializable]
    public sealed class TriggerZoneBindingComponent : IEntityComponent {
        public TriggerZone triggerZone;
    }

}
