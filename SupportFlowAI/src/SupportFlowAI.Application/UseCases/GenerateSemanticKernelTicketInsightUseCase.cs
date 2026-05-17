using Microsoft.SemanticKernel;
using SupportFlowAI.Application.DTOs;
using SupportFlowAI.Application.Interfaces;

namespace SupportFlowAI.Application.UseCases;

public sealed class GenerateSemanticKernelTicketInsightUseCase
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ICognitiveTextAnalyzer _cognitiveTextAnalyzer;
    private readonly Kernel _kernel;

    public GenerateSemanticKernelTicketInsightUseCase(
        ITicketRepository ticketRepository,
        ICognitiveTextAnalyzer cognitiveTextAnalyzer,
        Kernel kernel)
    {
        _ticketRepository = ticketRepository;
        _cognitiveTextAnalyzer = cognitiveTextAnalyzer;
        _kernel = kernel;
    }

    public async Task<SemanticKernelTicketInsightResponse> HandleAsync(
        Guid ticketId,
        CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId, cancellationToken);

        if (ticket is null)
            throw new KeyNotFoundException("Chamado não encontrado.");

        var cognitiveAnalysis = await _cognitiveTextAnalyzer.AnalyzeAsync(
            $"{ticket.Title}. {ticket.Description}",
            cancellationToken
        );

        var prompt = $$"""
        Você é um especialista em suporte corporativo e IA aplicada.

        Analise o chamado abaixo combinando:
        1. O conteúdo original do chamado.
        2. A análise cognitiva feita pelo Azure AI Language.

        Chamado:
        Título: {{ticket.Title}}
        Descrição: {{ticket.Description}}
        Solicitante: {{ticket.RequestedBy}}

        Análise do Azure AI Language:
        Sentimento: {{cognitiveAnalysis.Sentiment}}
        Score positivo: {{cognitiveAnalysis.PositiveScore}}
        Score neutro: {{cognitiveAnalysis.NeutralScore}}
        Score negativo: {{cognitiveAnalysis.NegativeScore}}
        Frases-chave: {{string.Join(", ", cognitiveAnalysis.KeyPhrases)}}
        Entidades: {{string.Join(", ", cognitiveAnalysis.Entities)}}

        Gere uma resposta em português contendo:
        - resumo executivo do chamado;
        - risco operacional percebido;
        - provável equipe responsável;
        - recomendação de próxima ação;
        - resposta sugerida ao usuário.

        Seja objetivo e profissional.
        """;

        var result = await _kernel.InvokePromptAsync(
            prompt,
            cancellationToken: cancellationToken
        );

        return new SemanticKernelTicketInsightResponse(
            ticket.Id,
            ticket.Title,
            cognitiveAnalysis.Sentiment,
            cognitiveAnalysis.PositiveScore,
            cognitiveAnalysis.NeutralScore,
            cognitiveAnalysis.NegativeScore,
            cognitiveAnalysis.KeyPhrases,
            cognitiveAnalysis.Entities,
            result.ToString()
        );
    }
}