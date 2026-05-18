namespace SupportFlowAI.Infrastructure.AzureSpeech;

public sealed class AzureSpeechOptions
{
    public const string SectionName = "AzureSpeech";

    public string Key { get; set; } = string.Empty;
    public string Region { get; set; } = "eastus";
    public string Language { get; set; } = "pt-BR";
}