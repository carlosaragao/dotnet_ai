using System.Collections.Concurrent;
using SupportFlowAI.Application.AI;
using SupportFlowAI.Application.Interfaces;

namespace SupportFlowAI.Infrastructure.FoundryAI;

public sealed class InMemoryAiUsageRepository : IAiUsageRepository
{
    private readonly ConcurrentBag<AiUsageRecord> _records = [];

    public Task AddAsync(AiUsageRecord record, CancellationToken cancellationToken)
    {
        _records.Add(record);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<AiUsageRecord>> ListAsync(
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<AiUsageRecord> result = _records
            .OrderByDescending(record => record.CreatedAt)
            .ToList();

        return Task.FromResult(result);
    }
}