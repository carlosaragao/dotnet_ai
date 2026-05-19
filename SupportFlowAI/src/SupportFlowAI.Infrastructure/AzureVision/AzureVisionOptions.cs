namespace SupportFlowAI.Infrastructure.AzureVision;

public sealed class AzureVisionOptions
{
    public const string SectionName = "AzureVision";

    public string Endpoint { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
}