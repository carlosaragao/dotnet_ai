using Microsoft.AspNetCore.Mvc;
using SupportFlowAI.Application.DTOs.SemanticKernel;
using SupportFlowAI.Application.UseCases;

namespace SupportFlowAI.Api.Controllers;

[ApiController]
[Route("api/semantic-kernel/plugins")]
public sealed class SemanticKernelPluginsController : ControllerBase
{
    private readonly InspectTicketWithKernelUseCase _inspectTicketUseCase;
    private readonly GenerateTicketAnswerWithKernelUseCase _generateAnswerUseCase;

    public SemanticKernelPluginsController(
        InspectTicketWithKernelUseCase inspectTicketUseCase,
        GenerateTicketAnswerWithKernelUseCase generateAnswerUseCase)
    {
        _inspectTicketUseCase = inspectTicketUseCase;
        _generateAnswerUseCase = generateAnswerUseCase;
    }

    [HttpPost("tickets/{ticketId:guid}/inspect")]
    public async Task<ActionResult<KernelPluginExecutionResponse>> InspectTicketAsync(
        Guid ticketId,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _inspectTicketUseCase.HandleAsync(
                ticketId,
                cancellationToken
            );

            return Ok(response);
        }
        catch (Exception exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpPost("tickets/{ticketId:guid}/answer")]
    public async Task<ActionResult<KernelPluginExecutionResponse>> GenerateAnswerAsync(
        Guid ticketId,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _generateAnswerUseCase.HandleAsync(
                ticketId,
                cancellationToken
            );

            return Ok(response);
        }
        catch (Exception exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }
}