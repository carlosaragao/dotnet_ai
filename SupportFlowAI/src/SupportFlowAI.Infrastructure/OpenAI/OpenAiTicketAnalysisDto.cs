using System.Text.Json.Serialization;

namespace SupportFlowAI.Infrastructure.OpenAI;

public sealed record OpenAiTicketAnalysisDto(
    [property: JsonPropertyName("category")] string Category,
    [property: JsonPropertyName("priority")] string Priority,
    [property: JsonPropertyName("sentiment")] string Sentiment,
    [property: JsonPropertyName("confidence")] double Confidence,
    [property: JsonPropertyName("summary")] string Summary,
    [property: JsonPropertyName("suggestedResponse")] string SuggestedResponse
);