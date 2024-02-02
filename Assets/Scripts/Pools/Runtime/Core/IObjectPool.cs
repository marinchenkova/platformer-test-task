namespace Pools.Core {

    public interface IObjectPool<T> {

        T TakeElement(bool active = false);

        void RecycleElement(T element);
    }

}
