using Microsoft.AspNetCore.Mvc;
using SupportFlowAI.Application.DTOs.LlmConcepts;
using SupportFlowAI.Application.UseCases;

namespace SupportFlowAI.Api.Controllers;

[ApiController]
[Route("api/llm-concepts")]
public sealed class LlmConceptsController : ControllerBase
{
    private readonly ExplainLlmConceptUseCase _explainLlmConceptUseCase;
    private readonly CompareEmbeddingsUseCase _compareEmbeddingsUseCase;

    public LlmConceptsController(
        ExplainLlmConceptUseCase explainLlmConceptUseCase,
        CompareEmbeddingsUseCase compareEmbeddingsUseCase)
    {
        _explainLlmConceptUseCase = explainLlmConceptUseCase;
        _compareEmbeddingsUseCase = compareEmbeddingsUseCase;
    }

    [HttpPost("explain")]
    public async Task<ActionResult<ExplainLlmConceptResponse>> ExplainAsync(
        [FromBody] ExplainLlmConceptRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _explainLlmConceptUseCase.HandleAsync(
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

    [HttpPost("compare-embeddings")]
    public async Task<ActionResult<CompareEmbeddingsResponse>> CompareEmbeddingsAsync(
        [FromBody] CompareEmbeddingsRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _compareEmbeddingsUseCase.HandleAsync(
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