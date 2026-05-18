namespace SupportFlowAI.Application.DTOs.Media;

public sealed record SpeechToTextResponse(
    string Transcription,
    string Language,
    string Provider
);