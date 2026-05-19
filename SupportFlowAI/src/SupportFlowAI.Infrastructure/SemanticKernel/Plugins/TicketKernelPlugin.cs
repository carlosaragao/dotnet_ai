using System.ComponentModel;
using System.Text.Json;
using Microsoft.SemanticKernel;
using SupportFlowAI.Application.Interfaces;

namespace SupportFlowAI.Infrastructure.SemanticKernel.Plugins;

public sealed class TicketKernelPlugin
{
    private readonly ITicketRepository _ticketRepository;

    public TicketKernelPlugin(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    [KernelFunction("get_ticket")]
    [Description("Obtém os dados completos de um chamado pelo seu identificador.")]
    public async Task<string> GetTicketAsync(
        [Description("Identificador do chamado em formato Guid.")] string ticketId,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(ticketId, out var id))
            return "TicketId inválido.";

        var ticket = await _ticketRepository.GetByIdAsync(id, cancellationToken);

        if (ticket is null)
            return "Chamado não encontrado.";

        var result = new
        {
            ticket.Id,
            ticket.Title,
            ticket.Description,
            ticket.RequestedBy,
            ticket.Category,
            ticket.Priority,
            ticket.Status,
            ticket.CreatedAt,
            Analysis = ticket.Analysis is null
                ? null
                : new
                {
                    ticket.Analysis.Category,
                    ticket.Analysis.Priority,
                    ticket.Analysis.Sentiment,
                    ticket.Analysis.Confidence,
                    ticket.Analysis.Summary,
                    ticket.Analysis.SuggestedResponse,
                    ticket.Analysis.ModelName
                }
        };

        return JsonSerializer.Serialize(result, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    [KernelFunction("list_open_tickets")]
    [Description("Lista os chamados abertos ou em andamento.")]
    public async Task<string> ListOpenTicketsAsync(CancellationToken cancellationToken)
    {
        var tickets = await _ticketRepository.ListAsync(cancellationToken);

        var result = tickets
            .Where(ticket => ticket.Status.ToString() is "Open" or "InProgress")
            .Select(ticket => new
            {
                ticket.Id,
                ticket.Title,
                ticket.Category,
                ticket.Priority,
                ticket.Status,
                ticket.CreatedAt
            })
            .ToList();

        return JsonSerializer.Serialize(result, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }
}