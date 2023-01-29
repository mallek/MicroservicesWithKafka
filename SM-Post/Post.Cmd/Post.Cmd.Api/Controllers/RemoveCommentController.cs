using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RemoveCommentController : ControllerBase
    {
        private readonly ILogger<RemoveCommentController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;

        public RemoveCommentController(ILogger<RemoveCommentController> logger, ICommandDispatcher commandDispatcher)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveCommentAsync(Guid id, [FromBody] RemoveCommentCommand command)
        {
            try
            {
                command.Id = id;
                _logger.LogInformation("Received command: {@command}", command);

                await _commandDispatcher.SendAsync(command);

                return StatusCode(StatusCodes.Status200OK, new BaseResponse
                {
                    Message = "Comment removed successfully",
                });
            }
            catch(AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "cound not retrieve aggregate, client passed an incorrect comment ID");
                return StatusCode(StatusCodes.Status404NotFound, new BaseResponse
                {
                    Message = ex.Message,
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Client made bad request");
                return StatusCode(StatusCodes.Status400BadRequest, new BaseResponse
                {
                    Message = ex.Message,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while removing comment");
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = "Error while removing comment",
                });
            }
        }
        
    }
}