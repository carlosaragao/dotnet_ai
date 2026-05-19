namespace SupportFlowAI.Application.Interfaces;

public interface ITextTokenEstimator
{
    int EstimateTokens(string text);
}