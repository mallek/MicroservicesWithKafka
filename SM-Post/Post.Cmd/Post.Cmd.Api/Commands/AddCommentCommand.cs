using CQRS.Core.Commands;

namespace Post.Cmd.Api.Commands
{
    public class AddCommentCommand : BaseCommand
    {
        public string Comment { get; set; } = "No comment";
        public string UserName { get; set; } = "Unknown";
    }
}