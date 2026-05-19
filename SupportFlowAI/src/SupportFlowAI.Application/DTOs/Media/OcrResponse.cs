namespace SupportFlowAI.Application.DTOs.Media;

public sealed record OcrResponse(
    string ExtractedText,
    IReadOnlyCollection<string> Lines,
    string Provider
);