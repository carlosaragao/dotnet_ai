namespace SupportFlowAI.Application.DTOs;

public sealed record SemanticKernelTicketInsightResponse(
    Guid TicketId,
    string Title,
    string AzureSentiment,
    double PositiveScore,
    double NeutralScore,
    double NegativeScore,
    IReadOnlyCollection<string> KeyPhrases,
    IReadOnlyCollection<string> Entities,
    string SemanticKernelInsight
);