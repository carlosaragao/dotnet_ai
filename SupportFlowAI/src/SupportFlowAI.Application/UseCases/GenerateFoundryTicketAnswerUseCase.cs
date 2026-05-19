using SupportFlowAI.Application.DTOs.FoundryAI;
using SupportFlowAI.Application.Interfaces;

namespace SupportFlowAI.Application.UseCases;

public sealed class GenerateFoundryTicketAnswerUseCase
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IFoundryAiService _foundryAiService;

    public GenerateFoundryTicketAnswerUseCase(
        ITicketRepository ticketRepository,
        IFoundryAiService foundryAiService)
    {
        _ticketRepository = ticketRepository;
        _foundryAiService = foundryAiService;
    }

    public async Task<FoundryAiCompletionResponse> HandleAsync(
        Guid ticketId,
        CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId, cancellationToken);

        if (ticket is null)
            throw new KeyNotFoundException("Chamado não encontrado.");

        var instructions = """
        Você é um assistente corporativo de suporte técnico.
        Gere uma resposta inicial profissional para o usuário.
        Não invente dados que não estejam no chamado.
        Seja objetivo, educado e indique o próximo passo.
        """;

        var input = $"""
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
        """;

        return await _foundryAiService.GenerateAsync(
            new FoundryAiCompletionRequest(
                Input: input,
                Instructions: instructions,
                Temperature: 0.2,
                MaxOutputTokens: 400
            ),
            cancellationToken
        );
    }
}