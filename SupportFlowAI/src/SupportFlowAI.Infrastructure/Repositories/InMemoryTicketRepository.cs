using System.Collections.Concurrent;
using SupportFlowAI.Application.Interfaces;
using SupportFlowAI.Domain.Entities;

namespace SupportFlowAI.Infrastructure.Repositories;

public sealed class InMemoryTicketRepository : ITicketRepository
{
    private readonly ConcurrentDictionary<Guid, Ticket> _tickets = new();

    public Task AddAsync(Ticket ticket, CancellationToken cancellationToken)
    {
        _tickets.TryAdd(ticket.Id, ticket);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Ticket ticket, CancellationToken cancellationToken)
    {
        _tickets[ticket.Id] = ticket;
        return Task.CompletedTask;
    }

    public Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _tickets.TryGetValue(id, out var ticket);
        return Task.FromResult(ticket);
    }

    public Task<IReadOnlyCollection<Ticket>> ListAsync(CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Ticket> tickets = _tickets.Values
            .OrderByDescending(ticket => ticket.CreatedAt)
            .ToList();

        return Task.FromResult(tickets);
    }
}