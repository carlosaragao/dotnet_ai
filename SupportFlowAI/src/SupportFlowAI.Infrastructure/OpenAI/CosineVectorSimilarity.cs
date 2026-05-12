using SupportFlowAI.Application.Interfaces;
using SupportFlowAI.Domain.ValueObjects;

namespace SupportFlowAI.Infrastructure.OpenAI;

public sealed class CosineVectorSimilarity : IVectorSimilarity
{
    public double CosineSimilarity(EmbeddingVector first, EmbeddingVector second)
    {
        if (first.Dimensions != second.Dimensions)
            throw new InvalidOperationException("Os vetores devem possuir a mesma dimensão.");

        var dotProduct = 0d;
        var firstMagnitude = 0d;
        var secondMagnitude = 0d;

        for (var i = 0; i < first.Dimensions; i++)
        {
            var a = first.Values[i];
            var b = second.Values[i];

            dotProduct += a * b;
            firstMagnitude += a * a;
            secondMagnitude += b * b;
        }

        if (firstMagnitude == 0 || secondMagnitude == 0)
            return 0;

        return dotProduct / (Math.Sqrt(firstMagnitude) * Math.Sqrt(secondMagnitude));
    }
}