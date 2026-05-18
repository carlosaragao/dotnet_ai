using SupportFlowAI.Domain.ValueObjects;

namespace SupportFlowAI.Application.AI;

public sealed record WorkflowMemoryRecord(
    Guid Id,
    string Content,
    EmbeddingVector? Embedding,
    DateTime CreatedAt
);