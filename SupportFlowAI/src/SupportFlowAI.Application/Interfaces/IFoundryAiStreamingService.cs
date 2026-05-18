namespace SupportFlowAI.Application.Interfaces;

public interface IFoundryAiStreamingService
{
    IAsyncEnumerable<string> StreamAsync(
        string input,
        string? instructions,
        double? temperature,
        int? maxOutputTokens,
        CancellationToken cancellationToken);
}