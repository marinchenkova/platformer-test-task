namespace Entities.Core {

    /// <summary>
    /// Entity component is a container for entity data and logic.
    /// World sends entity events to the related components.
    /// </summary>
    public interface IEntityComponent {

        /// <summary>
        /// Called after component was attached to an entity.
        /// </summary>
        void OnEnable(Entity entity) {}

        /// <summary>
        /// Called after component was detached from an entity or before entity destroy.
        /// </summary>
        void OnDisable(Entity entity) {}

        /// <summary>
        /// Called before entity is destroyed and after <see cref="OnDisable"/> call.
        /// </summary>
        void OnDestroy(Entity entity) {}
    }

}
