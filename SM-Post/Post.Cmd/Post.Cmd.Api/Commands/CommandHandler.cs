using CQRS.Core.Handlers;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands
{
    public class CommandHandler : ICommandHandler
    {
        private readonly IEventSourcingHandler<PostAggregate> _eventSourcingHandler;

        public CommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
        {
            _eventSourcingHandler = eventSourcingHandler;
        }

        public async Task HandleAsync(NewPostCommand command)
        {
            var aggergate = new PostAggregate(command.Id, command.Author, command.Message);
            await _eventSourcingHandler.SaveAsync(aggergate);
        }

        public async Task HandleAsync(EditMessageCommand command)
        {
            var aggergate = await _eventSourcingHandler.GetByIdAsync(command.Id);
            aggergate.EditMessage(command.Message);

            await _eventSourcingHandler.SaveAsync(aggergate);
        }

        public async Task HandleAsync(LikePostCommand command)
        {
            var aggergate = await _eventSourcingHandler.GetByIdAsync(command.Id);
            aggergate.LikePost();

            await _eventSourcingHandler.SaveAsync(aggergate);
        }

        public async Task HandleAsync(DeletePostCommand command)
        {
            var aggergate = await _eventSourcingHandler.GetByIdAsync(command.Id);
            aggergate.DeletePost(command.Username);

            await _eventSourcingHandler.SaveAsync(aggergate);
        }

        public async Task HandleAsync(AddCommentCommand command)
        {
            var aggergate = await _eventSourcingHandler.GetByIdAsync(command.Id);
            aggergate.AddComment(command.UserName, command.Comment);

            await _eventSourcingHandler.SaveAsync(aggergate);
        }

        public async Task HandleAsync(EditCommentCommand command)
        {
            var aggergate = await _eventSourcingHandler.GetByIdAsync(command.Id);
            aggergate.EditComment(command.CommentId, command.Comment, command.UserName);

            await _eventSourcingHandler.SaveAsync(aggergate);
        }

        public async Task HandleAsync(RemoveCommentCommand command)
        {
            var aggergate = await _eventSourcingHandler.GetByIdAsync(command.Id);
            aggergate.RemoveComment(command.CommentId, command.UserName);

            await _eventSourcingHandler.SaveAsync(aggergate);
        }
    }
}