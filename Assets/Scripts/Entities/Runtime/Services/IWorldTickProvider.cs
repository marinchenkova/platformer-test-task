using Entities.Core;

namespace Entities.Runtime.Services {

    public interface IWorldTickProvider {

        void Subscribe(IUpdatable updatable, TickGroup tickGroup);

        void Unsubscribe(IUpdatable updatable, TickGroup tickGroup);
    }

}
