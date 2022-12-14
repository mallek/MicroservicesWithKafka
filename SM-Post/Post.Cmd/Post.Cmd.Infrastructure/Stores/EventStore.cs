using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastructure.Stores
{
    public class EventStore : IEventStore
    {
        private readonly IEventStoreRepository _eventStoreRepository;
        private readonly IEventProducer _eventProducer;

        public EventStore(IEventStoreRepository eventStoreRepository, IEventProducer eventProducer)
        {
            _eventStoreRepository = eventStoreRepository;
            _eventProducer = eventProducer;
        }

        public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
        {
            var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId).ConfigureAwait(false);

            if (eventStream == null || !eventStream.Any())
            {
                throw new AggregateNotFoundException($"Event stream for aggregate {aggregateId} not found");
            }

            return eventStream.OrderBy(x => x.Version).Select(x => x.EventData).ToList();
        }

        public async Task SaveEventsAsync(Guid aggregateId, List<BaseEvent> events, int expectedVersion)
        {
            var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId).ConfigureAwait(false);

            if (expectedVersion != -1 && eventStream.Any() && eventStream.Last().Version != expectedVersion)
            {
                throw new ConcurrencyException($"Aggregate {aggregateId} has been modified by another process");
            }

            var version = expectedVersion;
            var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");


            foreach (var @event in events)
            {
                version++;
                @event.Version = version;
                var eventType = @event.GetType().Name;
                var eventModel = new EventModel
                {
                    TimeStamp = DateTime.Now,
                    AggregateIdentifier = aggregateId,
                    AggregateType = nameof(PostAggregate),
                    EventData = @event,
                    EventType = eventType,
                    Version = version
                };

                //should wrap in transaction with Kafka producer
                await _eventStoreRepository.SaveAsync(eventModel).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(topic))
                {
                    await _eventProducer.ProduceAsync(topic, @event).ConfigureAwait(false);
                }
            }
        }
    }
}