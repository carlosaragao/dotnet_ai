using SupportFlowAI.Application.DTOs.LocalAI;
using SupportFlowAI.Application.Interfaces;

namespace SupportFlowAI.Application.UseCases;

public sealed class GenerateLocalTicketAnswerUseCase
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IOllamaChatService _ollamaChatService;

    public GenerateLocalTicketAnswerUseCase(
        ITicketRepository ticketRepository,
        IOllamaChatService ollamaChatService)
    {
        _ticketRepository = ticketRepository;
        _ollamaChatService = ollamaChatService;
    }

    public async Task<LocalAiChatResponse> HandleAsync(
        Guid ticketId,
        CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId, cancellationToken);

        if (ticket is null)
            throw new KeyNotFoundException("Chamado não encontrado.");

        var systemPrompt = """
        Você é um assistente local de suporte técnico.
        A resposta deve ser objetiva, profissional e segura.
        Não invente informações.
        Não solicite dados sensíveis adicionais.
        """;

        var prompt = $"""
        Gere uma resposta inicial para o chamado abaixo.

        Título:
        {ticket.Title}

        Descrição:
        {ticket.Description}

        Solicitante:
        {ticket.RequestedBy}

        Categoria atual:
        {ticket.Category}

        Prioridade atual:
        {ticket.Priority}

        Importante:
        Esta resposta está sendo gerada localmente, sem envio do conteúdo para a nuvem.
        """;

        return await _ollamaChatService.SendAsync(
            new LocalAiChatRequest(
                Prompt: prompt,
                SystemPrompt: systemPrompt,
                Model: null,
                Temperature: 0.2,
                MaxOutputTokens: 400
            ),
            cancellationToken
        );
    }
}