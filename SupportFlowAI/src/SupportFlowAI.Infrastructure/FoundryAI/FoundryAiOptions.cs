namespace SupportFlowAI.Infrastructure.FoundryAI;

public sealed class FoundryAiOptions
{
    public const string SectionName = "FoundryAi";

    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public double DefaultTemperature { get; set; } = 0.2;
    public int DefaultMaxOutputTokens { get; set; } = 500;
}