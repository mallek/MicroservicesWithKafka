namespace CQRS.Core.Messages
{
    public abstract class Message
    {
        public string MessageType { get; protected set; }
        public Guid Id { get; set; }

        protected Message()
        {
            MessageType = GetType().Name;
        }
    }
}
