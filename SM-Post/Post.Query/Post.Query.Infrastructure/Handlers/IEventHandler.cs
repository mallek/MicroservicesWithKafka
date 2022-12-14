using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Post.Common.Events;

namespace Post.Query.Infrastructure.Handlers
{
    public interface IEventHandler
    {
        Task On(PostCreatedEvent @event);
        Task On(MessageUpdatedEvent @event);
        Task On(PostLikedEvent @event);
        Task On(CommentAddedEvent @event);
        Task On(CommentUpdatedEvent @event);
        Task On(CommentRemovedEvent @event);
        Task On(PostRemovedEvent @event);
    }
}