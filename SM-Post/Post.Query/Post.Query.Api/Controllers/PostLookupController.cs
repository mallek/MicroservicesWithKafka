using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Common.DTOs;
using Post.Query.Api.DTOs;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PostLookupController : ControllerBase
    {
        private readonly ILogger<PostLookupController> _logger;
        private readonly IQueryDispatcher<PostEntity> _queryDispatcher;

        public PostLookupController(ILogger<PostLookupController> logger, IQueryDispatcher<PostEntity> queryDispatcher)
        {
            _logger = logger;
            _queryDispatcher = queryDispatcher;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPostsAsync()
        {
            try
            {
                var posts = await _queryDispatcher.SendAsync<FindAllPostsQuery>(new FindAllPostsQuery());
                return PostsResponse(posts);
            }
            catch (Exception e)
            {
                return ErrorResponse(e, "Error while getting all posts");
            }
        }

        [HttpGet("byId/{postId}")]
        public async Task<IActionResult> GetPostByIdAsync(Guid postId)
        {
            try
            {
                var posts = await _queryDispatcher.SendAsync<FindPostByIdQuery>(new FindPostByIdQuery { Id = postId });
                if (posts == null)
                {
                    return NoContent();
                }

                return Ok(new PostLookupResponse
                {
                    Posts = posts,
                    Message = $"Successfully retrieved post with id {postId}"
                });
            }
            catch (Exception e)
            {
                return ErrorResponse(e, "Error while getting post by id");
            }
        }

        [HttpGet("byAuthor/{author}")]
        public async Task<IActionResult> GetPostsByAuthorAsync(string author)
        {
            try
            {
                var posts = await _queryDispatcher.SendAsync<FindPostsByAuthorQuery>(new FindPostsByAuthorQuery { Author = author });
                return PostsResponse(posts);
            }
            catch (Exception e)
            {
                return ErrorResponse(e, "Error while getting posts by author");
            }
        }

        [HttpGet("withComments")]
        public async Task<IActionResult> GetPostsWithCommentsAsync()
        {
            try
            {
                var posts = await _queryDispatcher.SendAsync<FindPostsWithCommentsQuery>(new FindPostsWithCommentsQuery());
                return PostsResponse(posts);
            }
            catch (Exception e)
            {
                return ErrorResponse(e, "Error while getting posts with comments");
            }
        }

        [HttpGet("withLikes/{numberOfLikes}")]
        public async Task<IActionResult> GetPostsWithLikesAsync(int numberOfLikes)
        {
            try
            {
                var posts = await _queryDispatcher.SendAsync<FindPostsWithLikesQuery>(new FindPostsWithLikesQuery { NumberOfLikes = numberOfLikes });
                return PostsResponse(posts);
            }
            catch (Exception e)
            {
                return ErrorResponse(e, "Error while getting posts with likes");
            }
        }

        private IActionResult PostsResponse(List<PostEntity> posts)
        {
            if (posts == null || !posts.Any())
            {
                return NoContent();
            }

            var count = posts.Count;

            return Ok(new PostLookupResponse
            {
                Posts = posts,
                Message = $"Successfully retrieved {count} post{(count > 1 ? "s" : string.Empty)} with comments"
            });
        }

        private IActionResult ErrorResponse(Exception e, string SAFE_ERROR_MESSAGE)
        {
            _logger.LogError(e, SAFE_ERROR_MESSAGE);
            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
            {
                Message = SAFE_ERROR_MESSAGE
            });
        }
    }
}