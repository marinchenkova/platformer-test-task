using System.Collections.Generic;

namespace Platformer.Components {

    public interface ICollisionDetector {

        void FetchCollisions(ICollection<CollisionInfo> dest);
    }

}
