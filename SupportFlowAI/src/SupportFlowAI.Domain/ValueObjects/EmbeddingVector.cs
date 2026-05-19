using SupportFlowAI.Domain.Exceptions;

namespace SupportFlowAI.Domain.ValueObjects;

public sealed record EmbeddingVector
{
    public IReadOnlyList<float> Values { get; }

    public int Dimensions => Values.Count;

    public EmbeddingVector(IReadOnlyList<float> values)
    {
        if (values is null || values.Count == 0)
            throw new DomainException("O vetor de embedding não pode ser vazio.");

        Values = values.ToArray();
    }
}