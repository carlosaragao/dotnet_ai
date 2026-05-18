using System.Collections.Concurrent;
using SupportFlowAI.Application.AI;
using SupportFlowAI.Application.Interfaces;

namespace SupportFlowAI.Infrastructure.SemanticKernel.Memory;

public sealed class InMemoryWorkflowMemoryStore : IWorkflowMemoryStore
{
    private readonly ConcurrentBag<WorkflowMemoryRecord> _records = [];
    private readonly IEmbeddingGenerator _embeddingGenerator;
    private readonly IVectorSimilarity _vectorSimilarity;

    public InMemoryWorkflowMemoryStore(
        IEmbeddingGenerator embeddingGenerator,
        IVectorSimilarity vectorSimilarity)
    {
        _embeddingGenerator = embeddingGenerator;
        _vectorSimilarity = vectorSimilarity;
    }

    public async Task SaveAsync(
        string content,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(content))
            return;

        var embedding = await _embeddingGenerator.GenerateAsync(
            content,
            cancellationToken
        );

        _records.Add(new WorkflowMemoryRecord(
            Guid.NewGuid(),
            content,
            embedding.Vector,
            DateTime.UtcNow
        ));
    }

    public async Task<IReadOnlyCollection<string>> SearchAsync(
        string query,
        int topK,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        var queryEmbedding = await _embeddingGenerator.GenerateAsync(
            query,
            cancellationToken
        );

        var results = _records
            .Where(record => record.Embedding is not null)
            .Select(record => new
            {
                record.Content,
                Similarity = _vectorSimilarity.CosineSimilarity(
                    queryEmbedding.Vector,
                    record.Embedding!
                )
            })
            .OrderByDescending(item => item.Similarity)
            .Take(topK <= 0 ? 3 : topK)
            .Select(item => item.Content)
            .ToList();

        return results;
    }
}