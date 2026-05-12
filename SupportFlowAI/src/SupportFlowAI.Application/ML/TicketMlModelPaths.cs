namespace SupportFlowAI.Application.ML;

public sealed class TicketMlModelPaths
{
    public string CategoryModelPath { get; init; } =
        Path.Combine("artifacts", "models", "ticket-category-model.zip");

    public string EffortModelPath { get; init; } =
        Path.Combine("artifacts", "models", "ticket-effort-model.zip");
}