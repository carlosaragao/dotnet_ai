namespace SupportFlowAI.Application.DTOs.Multimodal;

public sealed record MultimodalAnalysisResponse(
    string? InputText,
    string? ExtractedImageText,
    string? AudioTranscription,
    string ConsolidatedAnalysis,
    string Provider,
    int InputTokens,
    int OutputTokens,
    int TotalTokens
);