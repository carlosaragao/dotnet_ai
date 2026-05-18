using Microsoft.AspNetCore.Mvc;
using SupportFlowAI.Application.AI;
using SupportFlowAI.Application.DTOs.FoundryAI;
using SupportFlowAI.Application.Interfaces;
using SupportFlowAI.Application.UseCases;

namespace SupportFlowAI.Api.Controllers;

[ApiController]
[Route("api/foundry-ai")]
public sealed class FoundryAiController : ControllerBase
{
    private readonly IFoundryAiService _foundryAiService;
    private readonly IFoundryAiStreamingService _streamingService;
    private readonly IAiUsageRepository _usageRepository;
    private readonly GenerateFoundryTicketAnswerUseCase _generateTicketAnswerUseCase;

    public FoundryAiController(
        IFoundryAiService foundryAiService,
        IFoundryAiStreamingService streamingService,
        IAiUsageRepository usageRepository,
        GenerateFoundryTicketAnswerUseCase generateTicketAnswerUseCase)
    {
        _foundryAiService = foundryAiService;
        _streamingService = streamingService;
        _usageRepository = usageRepository;
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

    [HttpPost("responses/stream")]
    public async Task StreamAsync(
        [FromBody] FoundryAiStreamRequest request,
        CancellationToken cancellationToken)
    {
        Response.ContentType = "text/plain; charset=utf-8";

        await foreach (var delta in _streamingService.StreamAsync(
            request.Input,
            request.Instructions,
            request.Temperature,
            request.MaxOutputTokens,
            cancellationToken))
        {
            await Response.WriteAsync(delta, cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
    }

    [HttpGet("usage")]
    public async Task<ActionResult<IReadOnlyCollection<AiUsageRecord>>> ListUsageAsync(
        CancellationToken cancellationToken)
    {
        var records = await _usageRepository.ListAsync(cancellationToken);
        return Ok(records);
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