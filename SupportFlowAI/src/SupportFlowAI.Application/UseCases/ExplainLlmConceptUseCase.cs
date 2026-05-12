using SupportFlowAI.Application.AI;
using SupportFlowAI.Application.DTOs.LlmConcepts;
using SupportFlowAI.Application.Interfaces;

namespace SupportFlowAI.Application.UseCases;

public sealed class ExplainLlmConceptUseCase
{
    private readonly IAiTextGenerator _textGenerator;

    public ExplainLlmConceptUseCase(IAiTextGenerator textGenerator)
    {
        _textGenerator = textGenerator;
    }

    public async Task<ExplainLlmConceptResponse> HandleAsync(
        ExplainLlmConceptRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Concept))
            throw new ArgumentException("O conceito é obrigatório.");

        var systemPrompt = """
        Você é um professor de pós-graduação explicando conceitos de Large Language Models
        para desenvolvedores .NET.

        Explique de forma técnica, objetiva e aplicada.
        Sempre conecte a explicação com decisões de desenvolvimento de software.
        """;

        var prompt = $"""
        Explique o conceito abaixo no contexto de LLMs:

        Conceito:
        {request.Concept}

        Texto de exemplo, se existir:
        {request.ExampleText}
        """;

        var result = await _textGenerator.GenerateAsync(
            new AiTextGenerationRequest(
                Prompt: prompt,
                SystemPrompt: systemPrompt,
                Temperature: 0.2,
                MaxOutputTokens: 600
            ),
            cancellationToken
        );

        return new ExplainLlmConceptResponse(
            request.Concept,
            result.Content,
            result.Model,
            result.PromptTokens,
            result.CompletionTokens,
            result.TotalTokens
        );
    }
}