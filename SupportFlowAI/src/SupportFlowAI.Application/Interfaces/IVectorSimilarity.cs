using SupportFlowAI.Domain.ValueObjects;

namespace SupportFlowAI.Application.Interfaces;

public interface IVectorSimilarity
{
    double CosineSimilarity(EmbeddingVector first, EmbeddingVector second);
}