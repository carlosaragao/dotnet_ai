namespace SupportFlowAI.Application.AI;

public sealed class TokenBudgetOptions
{
    public int MaxEstimatedInputTokens { get; set; } = 3000;
    public int MaxOutputTokens { get; set; } = 700;
}