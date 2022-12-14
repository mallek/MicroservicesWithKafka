using CQRS.Core.Events;

namespace Post.Common.Events
{
    public class CommentUpdatedEvent : BaseEvent
    {
        public CommentUpdatedEvent() : base(nameof(CommentUpdatedEvent))
        {

        }

        public Guid CommentId { get; set; }
        public string UserName { get; set; } = "Unknown";
        public string Comment { get; set; } = "No comment";
        public DateTime EditDate { get; set; }
    }
}