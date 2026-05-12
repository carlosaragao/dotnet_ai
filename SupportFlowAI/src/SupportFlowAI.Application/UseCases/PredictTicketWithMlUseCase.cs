using SupportFlowAI.Application.DTOs;
using SupportFlowAI.Application.Interfaces;

namespace SupportFlowAI.Application.UseCases;

public sealed class PredictTicketWithMlUseCase
{
    private readonly ITicketMlPredictor _predictor;

    public PredictTicketWithMlUseCase(ITicketMlPredictor predictor)
    {
        _predictor = predictor;
    }

    public Task<TicketMlPredictionResponse> HandleAsync(
        Guid ticketId,
        CancellationToken cancellationToken)
    {
        return _predictor.PredictAsync(ticketId, cancellationToken);
    }
}