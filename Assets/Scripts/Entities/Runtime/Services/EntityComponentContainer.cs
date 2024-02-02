using System;
using Entities.Core;

namespace Entities.Services {

    internal readonly struct EntityComponentContainer {

        public readonly IEntityComponent component;
        public readonly Type next;

        public EntityComponentContainer(IEntityComponent component, Type next) {
            this.component = component;
            this.next = next;
        }
    }

}
