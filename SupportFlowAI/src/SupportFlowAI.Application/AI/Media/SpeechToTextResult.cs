namespace SupportFlowAI.Application.AI.Media;

public sealed record SpeechToTextResult(
    string Transcription,
    string Language,
    string Provider
);