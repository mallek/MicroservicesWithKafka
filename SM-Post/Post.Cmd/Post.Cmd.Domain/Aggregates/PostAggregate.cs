using CQRS.Core.Domain;
using Post.Common.Events;

namespace Post.Cmd.Domain.Aggregates
{
    public class PostAggregate : AggregateRoot
    {
        private bool _active;
        private string? _author;
        private readonly Dictionary<Guid, Tuple<string, string>> _comments = new();

        public bool Active
        {
            get => _active; set => _active = value;
        }

        public PostAggregate()
        {

        }

        public PostAggregate(Guid id, string author, string message)
        {
            RaiseEvent(new PostCreatedEvent
            {
                Id = id,
                Author = author,
                Message = message,
                DatePosted = DateTime.Now
            });
        }

        public void Apply(PostCreatedEvent @event)
        {
            _id = @event.Id;
            _author = @event.Author;
            _active = true;
        }

        public void EditMessage(string message)
        {
            if (!_active)
            {
                throw new InvalidOperationException("Cannot edit a post that is not active");
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new InvalidOperationException($"The value of {nameof(message)} Message cannot be empty, please provide a valid {nameof(message)}");
            }

            RaiseEvent(new MessageUpdatedEvent
            {
                Id = _id,
                Message = message
            });
        }

        public void Apply(MessageUpdatedEvent @event)
        {
            _id = @event.Id;
        }

        //like post
        public void LikePost()
        {
            if (!_active)
            {
                throw new InvalidOperationException("Cannot like a post that is not active");
            }

            RaiseEvent(new PostLikedEvent
            {
                Id = _id
            });
        }

        public void Apply(PostLikedEvent @event)
        {
            _id = @event.Id;
        }

        public void AddComment(string username, string comment)
        {
            if (!_active)
            {
                throw new InvalidOperationException("Cannot add a comment to a post that is not active");
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new InvalidOperationException($"The value of {nameof(username)} cannot be empty, please provide a valid {nameof(username)}");
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                throw new InvalidOperationException($"The value of {nameof(comment)} cannot be empty, please provide a valid {nameof(comment)}");
            }

            RaiseEvent(new CommentAddedEvent
            {
                Id = _id,
                CommentId = Guid.NewGuid(),
                UserName = username,
                Comment = comment,
                CommentDate = DateTime.Now
            });
        }

        public void Apply(CommentAddedEvent @event)
        {
            _id = @event.Id;
            _comments.Add(@event.CommentId, new Tuple<string, string>(@event.UserName, @event.Comment));
        }

        //edit comment
        public void EditComment(Guid commentId, string comment, string username)
        {
            if (!_active)
            {
                throw new InvalidOperationException("Cannot edit a comment to a post that is not active");
            }

            if (!_comments[commentId].Item2.Equals(username))
            {
                throw new InvalidOperationException("You cannot edit a comment that is not yours");
            }

            if (!_comments.ContainsKey(commentId))
            {
                throw new InvalidOperationException($"The comment with id {commentId} does not exist");
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                throw new InvalidOperationException($"The value of {nameof(comment)} cannot be empty, please provide a valid {nameof(comment)}");
            }

            RaiseEvent(new CommentUpdatedEvent
            {
                Id = _id,
                CommentId = commentId,
                Comment = comment,
                UserName = username,
                EditDate = DateTime.Now
            });
        }

        public void Apply(CommentUpdatedEvent @event)
        {
            _id = @event.Id;
            _comments[@event.CommentId] = new Tuple<string, string>(@event.Comment, @event.UserName);
        }

        //remove comment
        public void RemoveComment(Guid commentId, string username)
        {
            if (!_active)
            {
                throw new InvalidOperationException("Cannot remove a comment to a post that is not active");
            }

            if (!_comments[commentId].Item2.Equals(username))
            {
                throw new InvalidOperationException("You cannot remove a comment that is not yours");
            }

            if (!_comments.ContainsKey(commentId))
            {
                throw new InvalidOperationException($"The comment with id {commentId} does not exist");
            }

            RaiseEvent(new CommentRemovedEvent
            {
                Id = _id,
                CommentId = commentId
            });
        }

        public void Apply(CommentRemovedEvent @event)
        {
            _id = @event.Id;
            _comments.Remove(@event.CommentId);
        }

        //delete post
        public void DeletePost(string username)
        {
            if (!_active)
            {
                throw new InvalidOperationException("Cannot delete a post that is not active");
            }

            if (_author == null)
            {
                throw new InvalidOperationException("Cannot delete a post that has no author");
            }

            if (!_author.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new InvalidOperationException("You cannot delete a post that is not yours");
            }

            RaiseEvent(new PostRemovedEvent
            {
                Id = _id
            });
        }

        public void Apply(PostRemovedEvent @event)
        {
            _id = @event.Id;
            _active = false;
        }
    }
}