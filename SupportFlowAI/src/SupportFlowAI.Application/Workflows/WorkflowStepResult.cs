namespace SupportFlowAI.Application.Workflows;

public sealed record WorkflowStepResult(
    int Order,
    string Plugin,
    string Function,
    string Result
);