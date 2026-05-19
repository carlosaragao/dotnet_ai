namespace SupportFlowAI.Application.DTOs;

public sealed record TicketMlPredictionResponse(
    Guid TicketId,
    string Title,
    string Description,
    string PredictedCategory,
    float EstimatedEffortHours,
    string ModelType
);