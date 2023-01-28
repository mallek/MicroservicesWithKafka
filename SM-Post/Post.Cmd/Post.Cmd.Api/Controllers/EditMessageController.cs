using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EditMessageController : ControllerBase
    {
        private readonly ILogger<EditMessageController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;

        public EditMessageController(ILogger<EditMessageController> logger, ICommandDispatcher commandDispatcher)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] EditMessageCommand command)
        {
            try
            {
                command.Id = id;
                _logger.LogInformation("Received command: {@command}", command);

                await _commandDispatcher.SendAsync(command);

                return StatusCode(StatusCodes.Status200OK, new BaseResponse
                {
                    Message = "Post edited successfully",
                });
            }
            catch(AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "cound not retrieve aggregate, client passed an incorrect post ID");
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
                _logger.LogError(ex, "Error while editing post");
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = "Internal server error",
                });
            }
        }
    }
}