using SupportFlowAI.Domain.Enums;
using SupportFlowAI.Domain.Exceptions;
using SupportFlowAI.Domain.ValueObjects;

namespace SupportFlowAI.Domain.Entities;

public sealed class Ticket
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string RequestedBy { get; private set; } = string.Empty;
    public TicketCategory Category { get; private set; }
    public TicketPriority Priority { get; private set; }
    public TicketStatus Status { get; private set; }
    public TicketAnalysis? Analysis { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Ticket()
    {
    }

    public Ticket(string title, string description, string requestedBy)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("O título do chamado é obrigatório.");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("A descrição do chamado é obrigatória.");

        if (string.IsNullOrWhiteSpace(requestedBy))
            throw new DomainException("O solicitante do chamado é obrigatório.");

        Id = Guid.NewGuid();
        Title = title.Trim();
        Description = description.Trim();
        RequestedBy = requestedBy.Trim();
        Category = TicketCategory.Unknown;
        Priority = TicketPriority.Medium;
        Status = TicketStatus.Open;
        CreatedAt = DateTime.UtcNow;
    }

    public void ApplyAnalysis(TicketAnalysis analysis)
    {
        Analysis = analysis;
        Category = analysis.Category;
        Priority = analysis.Priority;
    }

    public void StartProgress()
    {
        if (Status != TicketStatus.Open)
            throw new DomainException("Apenas chamados abertos podem ser iniciados.");

        Status = TicketStatus.InProgress;
    }

    public void Resolve()
    {
        if (Status == TicketStatus.Closed)
            throw new DomainException("Chamados fechados não podem ser resolvidos novamente.");

        Status = TicketStatus.Resolved;
    }

    public void Close()
    {
        if (Status != TicketStatus.Resolved)
            throw new DomainException("Apenas chamados resolvidos podem ser fechados.");

        Status = TicketStatus.Closed;
    }
}