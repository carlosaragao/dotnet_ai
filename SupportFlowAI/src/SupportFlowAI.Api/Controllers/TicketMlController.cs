using Microsoft.AspNetCore.Mvc;
using SupportFlowAI.Application.DTOs;
using SupportFlowAI.Application.UseCases;

namespace SupportFlowAI.Api.Controllers;

[ApiController]
[Route("api/ml/tickets")]
public sealed class TicketMlController : ControllerBase
{
    private readonly TrainTicketMlModelsUseCase _trainUseCase;
    private readonly PredictTicketWithMlUseCase _predictUseCase;

    public TicketMlController(
        TrainTicketMlModelsUseCase trainUseCase,
        PredictTicketWithMlUseCase predictUseCase)
    {
        _trainUseCase = trainUseCase;
        _predictUseCase = predictUseCase;
    }

    [HttpPost("train")]
    public async Task<ActionResult<TrainMlModelsResponse>> TrainAsync(
        CancellationToken cancellationToken)
    {
        var response = await _trainUseCase.HandleAsync(cancellationToken);
        return Ok(response);
    }

    [HttpPost("{ticketId:guid}/predict")]
    public async Task<ActionResult<TicketMlPredictionResponse>> PredictAsync(
        Guid ticketId,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _predictUseCase.HandleAsync(ticketId, cancellationToken);
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