using SupportFlowAI.Application.Interfaces;

namespace SupportFlowAI.Infrastructure.FoundryAI;

public sealed class SimpleTextTokenEstimator : ITextTokenEstimator
{
    public int EstimateTokens(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        return (int)Math.Ceiling(text.Length / 4.0);
    }
}