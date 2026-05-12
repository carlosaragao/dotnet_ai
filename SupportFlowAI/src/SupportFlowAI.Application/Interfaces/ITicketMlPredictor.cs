using SupportFlowAI.Application.DTOs;

namespace SupportFlowAI.Application.Interfaces;

public interface ITicketMlPredictor
{
    Task<TicketMlPredictionResponse> PredictAsync(
        Guid ticketId,
        CancellationToken cancellationToken);
}