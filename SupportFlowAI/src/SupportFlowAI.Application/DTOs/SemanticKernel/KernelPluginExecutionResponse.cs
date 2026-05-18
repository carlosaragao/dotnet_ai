namespace SupportFlowAI.Application.DTOs.SemanticKernel;

public sealed record KernelPluginExecutionResponse(
    string Plugin,
    string Function,
    string Result
);