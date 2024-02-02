namespace Entities {

    internal sealed class IncrementalEntityIdProvider : IEntityIdProvider {

        private long _lastId;

        public long GetNextId() {
            return _lastId++;
        }
    }

}
