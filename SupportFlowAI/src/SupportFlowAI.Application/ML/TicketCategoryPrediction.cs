using Microsoft.ML.Data;

namespace SupportFlowAI.Application.ML;

public sealed class TicketCategoryPrediction
{
    [ColumnName("PredictedLabel")]
    public string PredictedCategory { get; set; } = string.Empty;

    public float[] Score { get; set; } = [];
}