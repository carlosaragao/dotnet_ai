namespace SupportFlowAI.Infrastructure.Ollama;

public sealed class OllamaOptions
{
    public const string SectionName = "Ollama";

    public string BaseUrl { get; set; } = "http://localhost:11434/api/";
    public string DefaultModel { get; set; } = "llama3.2:3b";
    public double DefaultTemperature { get; set; } = 0.2;
    public int DefaultMaxOutputTokens { get; set; } = 400;
}