using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Events;

namespace CQRS.Core.Domain
{
    public interface IEventStoreRepository
    {
        Task SaveAsync(EventModel @event);
        Task<IEnumerable<EventModel>> FindByAggregateId(Guid aggregateId);
    }
}