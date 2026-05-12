using Microsoft.ML.Data;

namespace SupportFlowAI.Application.ML;

public sealed class TicketEffortPrediction
{
    [ColumnName("Score")]
    public float EstimatedEffortHours { get; set; }
}