using CQRS.Core.Commands;
using CQRS.Core.Infrastructure;

namespace Post.Cmd.Infrastructure.Dispatchers
{
    public class CommandDispatcher : ICommandDispatcher
    {

        private readonly Dictionary<Type, Func<BaseCommand, Task>> _handlers = new();

        public void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand
        {
            if (_handlers.ContainsKey(typeof(T)))
            {
                throw new IndexOutOfRangeException($"Handler for {typeof(T).Name} already registered");
            }

            _handlers.Add(typeof(T), command => handler((T)command));
        }

        public async Task SendAsync(BaseCommand command)
        {
            if (!_handlers.ContainsKey(command.GetType()))
            {
                throw new IndexOutOfRangeException($"Handler for {command.GetType().Name} not registered");
            }

            await _handlers[command.GetType()](command);
        }
    }
}