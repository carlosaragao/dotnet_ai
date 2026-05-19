namespace SupportFlowAI.Application.Workflows;

public sealed record WorkflowPlan(
    string Goal,
    IReadOnlyCollection<WorkflowPlanStep> Steps
);