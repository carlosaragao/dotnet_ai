namespace SupportFlowAI.Infrastructure.AzureAI;

public sealed class AzureLanguageOptions
{
    public const string SectionName = "AzureLanguage";

    public string Endpoint { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
}