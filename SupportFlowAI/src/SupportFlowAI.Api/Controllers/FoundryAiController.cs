using Microsoft.AspNetCore.Mvc;
using SupportFlowAI.Application.DTOs.FoundryAI;
using SupportFlowAI.Application.Interfaces;
using SupportFlowAI.Application.UseCases;

namespace SupportFlowAI.Api.Controllers;

[ApiController]
[Route("api/foundry-ai")]
public sealed class FoundryAiController : ControllerBase
{
    private readonly IFoundryAiService _foundryAiService;
    private readonly GenerateFoundryTicketAnswerUseCase _generateTicketAnswerUseCase;

    public FoundryAiController(
        IFoundryAiService foundryAiService,
        GenerateFoundryTicketAnswerUseCase generateTicketAnswerUseCase)
    {
        _foundryAiService = foundryAiService;
        _generateTicketAnswerUseCase = generateTicketAnswerUseCase;
    }

    [HttpPost("responses")]
    public async Task<ActionResult<FoundryAiCompletionResponse>> GenerateAsync(
        [FromBody] FoundryAiCompletionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _foundryAiService.GenerateAsync(
                request,
                cancellationToken
            );

            return Ok(response);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return StatusCode(502, new { error = exception.Message });
        }
    }

    [HttpPost("tickets/{ticketId:guid}/answer")]
    public async Task<ActionResult<FoundryAiCompletionResponse>> GenerateTicketAnswerAsync(
        Guid ticketId,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _generateTicketAnswerUseCase.HandleAsync(
                ticketId,
                cancellationToken
            );

            return Ok(response);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { error = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return StatusCode(502, new { error = exception.Message });
        }
    }
}