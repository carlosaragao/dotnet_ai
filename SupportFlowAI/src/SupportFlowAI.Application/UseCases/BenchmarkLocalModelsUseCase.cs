using System.Diagnostics;
using SupportFlowAI.Application.DTOs.LocalAI;
using SupportFlowAI.Application.Interfaces;

namespace SupportFlowAI.Application.UseCases;

public sealed class BenchmarkLocalModelsUseCase
{
    private readonly IOllamaChatService _ollamaChatService;

    public BenchmarkLocalModelsUseCase(IOllamaChatService ollamaChatService)
    {
        _ollamaChatService = ollamaChatService;
    }

    public async Task<LocalModelBenchmarkResponse> HandleAsync(
        LocalModelBenchmarkRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Prompt))
            throw new ArgumentException("O prompt é obrigatório.");

        if (request.Models is null || request.Models.Count == 0)
            throw new ArgumentException("Informe ao menos um modelo para o benchmark.");

        var runs = request.Runs <= 0 ? 1 : request.Runs;
        var results = new List<LocalModelBenchmarkItemResponse>();

        foreach (var model in request.Models)
        {
            for (var run = 1; run <= runs; run++)
            {
                var stopwatch = Stopwatch.StartNew();

                var response = await _ollamaChatService.SendAsync(
                    new LocalAiChatRequest(
                        Prompt: request.Prompt,
                        SystemPrompt: "Você é um assistente local. Responda de forma objetiva.",
                        Model: model,
                        Temperature: request.Temperature,
                        MaxOutputTokens: request.MaxOutputTokens
                    ),
                    cancellationToken
                );

                stopwatch.Stop();

                results.Add(new LocalModelBenchmarkItemResponse(
                    Model: model,
                    Run: run,
                    ElapsedMilliseconds: stopwatch.ElapsedMilliseconds,
                    PromptEvalCount: response.PromptEvalCount,
                    EvalCount: response.EvalCount,
                    TokensPerSecond: response.TokensPerSecond,
                    ResponsePreview: CreatePreview(response.Response)
                ));
            }
        }

        return new LocalModelBenchmarkResponse(
            request.Prompt,
            results
        );
    }

    private static string CreatePreview(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        return text.Length <= 200
            ? text
            : text[..200] + "...";
    }
}