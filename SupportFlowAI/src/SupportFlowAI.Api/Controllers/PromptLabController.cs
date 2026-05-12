using Microsoft.AspNetCore.Mvc;
using SupportFlowAI.Application.DTOs.PromptLab;
using SupportFlowAI.Application.UseCases;

namespace SupportFlowAI.Api.Controllers;

[ApiController]
[Route("api/prompt-lab")]
public sealed class PromptLabController : ControllerBase
{
    private readonly ExecutePromptExperimentUseCase _executePromptExperimentUseCase;
    private readonly ListPromptLabModelsUseCase _listPromptLabModelsUseCase;

    public PromptLabController(
        ExecutePromptExperimentUseCase executePromptExperimentUseCase,
        ListPromptLabModelsUseCase listPromptLabModelsUseCase)
    {
        _executePromptExperimentUseCase = executePromptExperimentUseCase;
        _listPromptLabModelsUseCase = listPromptLabModelsUseCase;
    }

    [HttpGet("models")]
    public ActionResult<AvailableModelsResponse> ListModels()
    {
        return Ok(_listPromptLabModelsUseCase.Handle());
    }

    [HttpPost("experiment")]
    public async Task<ActionResult<PromptExperimentResponse>> ExecuteExperimentAsync(
        [FromBody] PromptExperimentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _executePromptExperimentUseCase.HandleAsync(
                request,
                cancellationToken
            );

            return Ok(response);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }
}