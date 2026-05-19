namespace SupportFlowAI.Application.AI.Media;

public sealed record OcrResult(
    string ExtractedText,
    IReadOnlyCollection<string> Lines,
    string Provider
);