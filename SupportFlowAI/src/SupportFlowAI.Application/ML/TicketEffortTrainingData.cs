using Microsoft.ML.Data;

namespace SupportFlowAI.Application.ML;

public sealed class TicketEffortTrainingData
{
    [LoadColumn(0)]
    public string Text { get; set; } = string.Empty;

    [LoadColumn(1)]
    public string Category { get; set; } = string.Empty;

    [LoadColumn(2)]
    public string Priority { get; set; } = string.Empty;

    [LoadColumn(3)]
    public float EffortHours { get; set; }
}