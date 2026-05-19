namespace SupportFlowAI.Application.AI;

public sealed record CognitiveTextAnalysisResult(
    string Sentiment,
    double PositiveScore,
    double NeutralScore,
    double NegativeScore,
    IReadOnlyCollection<string> KeyPhrases,
    IReadOnlyCollection<string> Entities
);