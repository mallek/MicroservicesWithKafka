using CQRS.Core.Commands;

namespace Post.Cmd.Api.Commands
{
    public class EditCommentCommand : BaseCommand
    {
        public Guid CommentId { get; set; }
        public string Comment { get; set; } = "No comment";
        public string UserName { get; set; } = "Unknown";
    }
}