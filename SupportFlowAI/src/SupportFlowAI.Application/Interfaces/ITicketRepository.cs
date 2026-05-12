using SupportFlowAI.Domain.Entities;

namespace SupportFlowAI.Application.Interfaces;

public interface ITicketRepository
{
    Task AddAsync(Ticket ticket, CancellationToken cancellationToken);
    Task UpdateAsync(Ticket ticket, CancellationToken cancellationToken);
    Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Ticket>> ListAsync(CancellationToken cancellationToken);
}