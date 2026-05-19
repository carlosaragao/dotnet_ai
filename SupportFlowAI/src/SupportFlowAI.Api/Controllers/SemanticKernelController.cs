using Microsoft.AspNetCore.Mvc;
using SupportFlowAI.Application.DTOs;
using SupportFlowAI.Application.UseCases;

namespace SupportFlowAI.Api.Controllers;

[ApiController]
[Route("api/semantic-kernel")]
public sealed class SemanticKernelController : ControllerBase
{
    private readonly GenerateSemanticKernelTicketInsightUseCase _useCase;

    public SemanticKernelController(
        GenerateSemanticKernelTicketInsightUseCase useCase)
    {
        _useCase = useCase;
    }

    [HttpPost("tickets/{ticketId:guid}/insights")]
    public async Task<ActionResult<SemanticKernelTicketInsightResponse>> GenerateInsightAsync(
        Guid ticketId,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _useCase.HandleAsync(ticketId, cancellationToken);
            return Ok(response);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { error = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }
}