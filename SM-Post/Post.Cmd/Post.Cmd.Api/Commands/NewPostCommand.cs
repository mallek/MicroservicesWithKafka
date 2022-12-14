using CQRS.Core.Commands;

namespace Post.Cmd.Api.Commands
{
    public class NewPostCommand : BaseCommand
    {
        public string Author { get; set; } = "Unknown";
        public string Message { get; set; } = "No message";
    }
}