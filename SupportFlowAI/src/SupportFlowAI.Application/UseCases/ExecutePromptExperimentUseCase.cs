using SupportFlowAI.Application.AI;
using SupportFlowAI.Application.DTOs.PromptLab;
using SupportFlowAI.Application.Interfaces;
using SupportFlowAI.Application.Prompts;

namespace SupportFlowAI.Application.UseCases;

public sealed class ExecutePromptExperimentUseCase
{
    private readonly IAiTextGenerator _textGenerator;
    private readonly PromptTemplateBuilder _promptTemplateBuilder;

    public ExecutePromptExperimentUseCase(
        IAiTextGenerator textGenerator,
        PromptTemplateBuilder promptTemplateBuilder)
    {
        _textGenerator = textGenerator;
        _promptTemplateBuilder = promptTemplateBuilder;
    }

    public async Task<PromptExperimentResponse> HandleAsync(
        PromptExperimentRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Input))
            throw new ArgumentException("A entrada é obrigatória.");

        if (string.IsNullOrWhiteSpace(request.Task))
            throw new ArgumentException("A tarefa é obrigatória.");

        if (request.Temperature is < 0 or > 2)
            throw new ArgumentException("A temperature deve estar entre 0 e 2.");

        if (request.TopP is < 0 or > 1)
            throw new ArgumentException("O top_p deve estar entre 0 e 1.");

        if (request.MaxOutputTokens <= 0)
            throw new ArgumentException("MaxOutputTokens deve ser maior que zero.");

        var (systemPrompt, finalPrompt) = _promptTemplateBuilder.Build(
            request.Task,
            request.Input,
            request.Style
        );

        var result = await _textGenerator.GenerateAsync(
            new AiTextGenerationRequest(
                Prompt: finalPrompt,
                SystemPrompt: systemPrompt,
                Temperature: request.Temperature,
                MaxOutputTokens: request.MaxOutputTokens,
                Model: request.Model,
                TopP: request.TopP
            ),
            cancellationToken
        );

        return new PromptExperimentResponse(
            result.Model,
            request.Task,
            request.Style,
            request.Temperature,
            request.TopP,
            request.MaxOutputTokens,
            systemPrompt,
            finalPrompt,
            result.Content,
            result.PromptTokens,
            result.CompletionTokens,
            result.TotalTokens,
            result.FinishReason
        );
    }
}