namespace Pools.Core {

    public interface IPoolFactory<T> {

        T CreateElement(T sample);
        void DestroyElement(T element);

        void ActivateElement(T element);
        void DeactivateElement(T element);
    }

}
