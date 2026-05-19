using System.Text.Json;

namespace SupportFlowAI.Application.Workflows;

public sealed record WorkflowPlanStep(
    int Order,
    string Plugin,
    string Function,
    Dictionary<string, JsonElement> Arguments,
    string Reason
);