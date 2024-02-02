using System.Collections.Generic;
using Entities.Core;

namespace Entities.Services {

    public interface IEntityStorage {

        IEnumerable<Entity> Entities { get; }

        void AddEntity(Entity entity);

        void RemoveEntity(Entity entity);

        bool ContainsEntity(Entity entity);

        void Clear();
    }

}
