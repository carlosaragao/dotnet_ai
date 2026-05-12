using Microsoft.ML.Data;

namespace SupportFlowAI.Application.ML;

public sealed class TicketCategoryTrainingData
{
    [LoadColumn(0)]
    public string Text { get; set; } = string.Empty;

    [LoadColumn(1)]
    public string Category { get; set; } = string.Empty;
}