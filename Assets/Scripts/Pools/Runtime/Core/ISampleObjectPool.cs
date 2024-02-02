namespace Pools.Core {

    public interface ISampleObjectPool<T> {

        T TakeElement(T sample, bool active = false);

        void RecycleElement(T element);
    }

}
