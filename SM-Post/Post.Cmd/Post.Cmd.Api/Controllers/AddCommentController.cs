using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AddCommentController : ControllerBase
    {
         private readonly ILogger<AddCommentController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;

        public AddCommentController(ILogger<AddCommentController> logger, ICommandDispatcher commandDispatcher)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AddCommentAsync(Guid id, [FromBody] AddCommentCommand command)
        {
            try
            {
                command.Id = id;
                _logger.LogInformation("Received command: {@command}", command);

                await _commandDispatcher.SendAsync(command);

                return StatusCode(StatusCodes.Status200OK, new BaseResponse
                {
                    Message = "Add comment to post completed successfully",
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
                _logger.LogError(ex, "Error while adding comment to post");
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = "Internal server error",
                });
            }
        }
    }
}