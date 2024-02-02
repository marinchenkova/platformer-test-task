using Entities.Core;

namespace Entities.Services {

    public interface IEntityComponentFactory {

        IEntityComponent Copy(IEntityComponent sample);
    }

}
