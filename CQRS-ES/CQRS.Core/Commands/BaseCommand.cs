using CQRS.Core.Messages;

namespace CQRS.Core.Commands
{
    public abstract class BaseCommand : Message
    {
        public DateTime Timestamp { get; protected set; }

        protected BaseCommand()
        {
            Timestamp = DateTime.Now;
        }
    }
}