using SupportFlowAI.Application.DTOs.Workflows;

namespace SupportFlowAI.Application.UseCases;

public sealed class RunSupportAgentWorkflowUseCase
{
    private readonly CreateWorkflowPlanUseCase _createWorkflowPlanUseCase;
    private readonly ExecuteWorkflowPlanUseCase _executeWorkflowPlanUseCase;

    public RunSupportAgentWorkflowUseCase(
        CreateWorkflowPlanUseCase createWorkflowPlanUseCase,
        ExecuteWorkflowPlanUseCase executeWorkflowPlanUseCase)
    {
        _createWorkflowPlanUseCase = createWorkflowPlanUseCase;
        _executeWorkflowPlanUseCase = executeWorkflowPlanUseCase;
    }

    public async Task<WorkflowExecutionResponse> HandleAsync(
        WorkflowPlanRequest request,
        CancellationToken cancellationToken)
    {
        var planResponse = await _createWorkflowPlanUseCase.HandleAsync(
            request,
            cancellationToken
        );

        return await _executeWorkflowPlanUseCase.HandleAsync(
            planResponse.Plan,
            cancellationToken
        );
    }
}