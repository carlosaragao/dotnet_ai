using Microsoft.AspNetCore.Mvc;
using SupportFlowAI.Application.DTOs.LocalAI;
using SupportFlowAI.Application.UseCases;
using SupportFlowAI.Application.Interfaces;

namespace SupportFlowAI.Api.Controllers;

[ApiController]
[Route("api/local-ai")]
public sealed class LocalAiController : ControllerBase
{
    private readonly IOllamaChatService _ollamaChatService;
    private readonly GenerateLocalTicketAnswerUseCase _generateLocalTicketAnswerUseCase;
    private readonly BenchmarkLocalModelsUseCase _benchmarkLocalModelsUseCase;
    private readonly IOllamaHealthService _healthService;

    public LocalAiController(
        IOllamaChatService ollamaChatService,
        GenerateLocalTicketAnswerUseCase generateLocalTicketAnswerUseCase,
        BenchmarkLocalModelsUseCase benchmarkLocalModelsUseCase,
        IOllamaHealthService healthService)
    {
        _ollamaChatService = ollamaChatService;
        _generateLocalTicketAnswerUseCase = generateLocalTicketAnswerUseCase;
        _benchmarkLocalModelsUseCase = benchmarkLocalModelsUseCase;
        _healthService = healthService;
    }

    [HttpPost("chat")]
    public async Task<ActionResult<LocalAiChatResponse>> ChatAsync(
        [FromBody] LocalAiChatRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _ollamaChatService.SendAsync(
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
    public async Task<ActionResult<LocalAiChatResponse>> GenerateTicketAnswerAsync(
        Guid ticketId,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _generateLocalTicketAnswerUseCase.HandleAsync(
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

    [HttpPost("benchmark")]
    public async Task<ActionResult<LocalModelBenchmarkResponse>> BenchmarkAsync(
        [FromBody] LocalModelBenchmarkRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _benchmarkLocalModelsUseCase.HandleAsync(
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

    [HttpGet("health")]
    public async Task<ActionResult<LocalAiHealthResponse>> HealthAsync(
        CancellationToken cancellationToken)
    {
        var response = await _healthService.CheckAsync(cancellationToken);
        return Ok(response);
    }
}