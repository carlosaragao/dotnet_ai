using SupportFlowAI.Application.Workflows;

namespace SupportFlowAI.Application.DTOs.Workflows;

public sealed record WorkflowExecutionResponse(
    string Goal,
    IReadOnlyCollection<WorkflowStepResult> Results,
    string FinalSummary
);