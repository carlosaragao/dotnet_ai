namespace SupportFlowAI.Application.DTOs.Workflows;

public sealed record WorkflowPlanRequest(
    string Goal,
    Guid? TicketId,
    string? AdditionalContext
);