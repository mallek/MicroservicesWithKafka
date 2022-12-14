using CQRS.Core.Events;

namespace Post.Common.Events
{
    public class PostCreatedEvent : BaseEvent
    {
        public PostCreatedEvent() : base(nameof(PostCreatedEvent))
        {

        }

        public string Author { get; set; } = "Unknown";
        public string Message { get; set; } = "No message";
        public DateTime DatePosted { get; set; }
    }
}