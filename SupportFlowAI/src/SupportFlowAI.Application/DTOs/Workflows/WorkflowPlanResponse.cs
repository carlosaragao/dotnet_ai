using SupportFlowAI.Application.Workflows;

namespace SupportFlowAI.Application.DTOs.Workflows;

public sealed record WorkflowPlanResponse(
    WorkflowPlan Plan,
    string RawPlan
);