using SupportFlowAI.Application.AI;

namespace SupportFlowAI.Application.Interfaces;

public interface IAiUsageRepository
{
    Task AddAsync(AiUsageRecord record, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<AiUsageRecord>> ListAsync(CancellationToken cancellationToken);
}