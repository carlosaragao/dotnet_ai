using Microsoft.AspNetCore.Mvc;
using SupportFlowAI.Application.DTOs.Workflows;
using SupportFlowAI.Application.UseCases;

namespace SupportFlowAI.Api.Controllers;

[ApiController]
[Route("api/semantic-kernel/workflows")]
public sealed class SemanticKernelWorkflowController : ControllerBase
{
    private readonly CreateWorkflowPlanUseCase _createPlanUseCase;
    private readonly RunSupportAgentWorkflowUseCase _runWorkflowUseCase;

    public SemanticKernelWorkflowController(
        CreateWorkflowPlanUseCase createPlanUseCase,
        RunSupportAgentWorkflowUseCase runWorkflowUseCase)
    {
        _createPlanUseCase = createPlanUseCase;
        _runWorkflowUseCase = runWorkflowUseCase;
    }

    [HttpPost("plan")]
    public async Task<ActionResult<WorkflowPlanResponse>> CreatePlanAsync(
        [FromBody] WorkflowPlanRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _createPlanUseCase.HandleAsync(
                request,
                cancellationToken
            );

            return Ok(response);
        }
        catch (Exception exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpPost("run")]
    public async Task<ActionResult<WorkflowExecutionResponse>> RunAsync(
        [FromBody] WorkflowPlanRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _runWorkflowUseCase.HandleAsync(
                request,
                cancellationToken
            );

            return Ok(response);
        }
        catch (Exception exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }
}