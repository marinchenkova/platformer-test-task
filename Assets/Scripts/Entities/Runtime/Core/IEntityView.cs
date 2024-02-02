namespace Entities.Core {

    public interface IEntityView {

        Entity Entity { get; }

        void Bind(Entity entity);

        void Unbind();
    }

}
