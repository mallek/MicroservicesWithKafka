using CQRS.Core.Events;

namespace CQRS.Core.Domain
{
    public abstract class AggregateRoot
    {
        protected Guid _id;
        private readonly List<BaseEvent> _changes = new();

        public Guid Id
        {
            get { return _id; }
        }

        public int Version { get; set; } = -1;

        public List<BaseEvent> GetUncommittedChanges()
        {
            return _changes;
        }

        public void MarkChangesAsCommitted()
        {
            _changes.Clear();
        }

        protected void RaiseEvent(BaseEvent @event)
        {
            ApplyChange(@event, true);
        }

        public void LoadsFromHistory(IEnumerable<BaseEvent> history)
        {
            foreach (var e in history.OrderBy(x => x.Version))
            {
                ApplyChange(e, false);
            }
        }

        private void ApplyChange(BaseEvent @event, bool isNew)
        {
            var method = this.GetType().GetMethod("Apply", new Type[] { @event.GetType() });

            if (method == null)
            {
                throw new Exception($"Aggregate {this.GetType().Name} does not have an Apply method for event {@event.GetType().Name}");
            }

            method.Invoke(this, new object[] { @event });

            if (isNew)
            {
                _changes.Add(@event);
            }
        }
    }
}