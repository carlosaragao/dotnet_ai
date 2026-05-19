using System.ComponentModel;
using Microsoft.SemanticKernel;
using SupportFlowAI.Application.DTOs.FoundryAI;
using SupportFlowAI.Application.Interfaces;

namespace SupportFlowAI.Infrastructure.SemanticKernel.Plugins;

public sealed class AiTicketKernelPlugin
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IFoundryAiService _foundryAiService;

    public AiTicketKernelPlugin(
        ITicketRepository ticketRepository,
        IFoundryAiService foundryAiService)
    {
        _ticketRepository = ticketRepository;
        _foundryAiService = foundryAiService;
    }

    [KernelFunction("summarize_ticket")]
    [Description("Gera um resumo executivo de um chamado.")]
    public async Task<string> SummarizeTicketAsync(
        [Description("Identificador do chamado em formato Guid.")] string ticketId,
        CancellationToken cancellationToken)
    {
        var ticket = await GetTicketOrThrowAsync(ticketId, cancellationToken);

        var response = await _foundryAiService.GenerateAsync(
            new FoundryAiCompletionRequest(
                Input: $"""
                Gere um resumo executivo do chamado abaixo.

                Título:
                {ticket.Title}

                Descrição:
                {ticket.Description}

                Categoria atual:
                {ticket.Category}

                Prioridade atual:
                {ticket.Priority}
                """,
                Instructions: "Você é um analista de suporte corporativo. Seja objetivo e profissional.",
                Temperature: 0.2,
                MaxOutputTokens: 300
            ),
            cancellationToken
        );

        return response.Response;
    }

    [KernelFunction("generate_ticket_answer")]
    [Description("Gera uma resposta inicial profissional para o usuário de um chamado.")]
    public async Task<string> GenerateTicketAnswerAsync(
        [Description("Identificador do chamado em formato Guid.")] string ticketId,
        CancellationToken cancellationToken)
    {
        var ticket = await GetTicketOrThrowAsync(ticketId, cancellationToken);

        var response = await _foundryAiService.GenerateAsync(
            new FoundryAiCompletionRequest(
                Input: $"""
                Gere uma resposta inicial para o usuário do chamado.

                Título:
                {ticket.Title}

                Descrição:
                {ticket.Description}

                Solicitante:
                {ticket.RequestedBy}

                Categoria:
                {ticket.Category}

                Prioridade:
                {ticket.Priority}
                """,
                Instructions: """
                Você é um assistente corporativo de suporte técnico.
                Não invente informações.
                Indique o próximo passo de forma clara.
                """,
                Temperature: 0.2,
                MaxOutputTokens: 400
            ),
            cancellationToken
        );

        return response.Response;
    }

    [KernelFunction("recommend_next_action")]
    [Description("Recomenda a próxima ação operacional para tratar um chamado.")]
    public async Task<string> RecommendNextActionAsync(
        [Description("Identificador do chamado em formato Guid.")] string ticketId,
        CancellationToken cancellationToken)
    {
        var ticket = await GetTicketOrThrowAsync(ticketId, cancellationToken);

        var response = await _foundryAiService.GenerateAsync(
            new FoundryAiCompletionRequest(
                Input: $"""
                Analise o chamado abaixo e recomende a próxima ação operacional.

                Título:
                {ticket.Title}

                Descrição:
                {ticket.Description}

                Categoria:
                {ticket.Category}

                Prioridade:
                {ticket.Priority}
                """,
                Instructions: """
                Você é um especialista em triagem de chamados.
                Retorne uma recomendação curta contendo:
                - equipe provável;
                - ação imediata;
                - risco se não for tratado.
                """,
                Temperature: 0.2,
                MaxOutputTokens: 350
            ),
            cancellationToken
        );

        return response.Response;
    }

    private async Task<SupportFlowAI.Domain.Entities.Ticket> GetTicketOrThrowAsync(
        string ticketId,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(ticketId, out var id))
            throw new ArgumentException("TicketId inválido.");

        var ticket = await _ticketRepository.GetByIdAsync(id, cancellationToken);

        return ticket ?? throw new KeyNotFoundException("Chamado não encontrado.");
    }
}