using SupportFlowAI.Application.DTOs.LlmConcepts;
using SupportFlowAI.Application.Interfaces;

namespace SupportFlowAI.Application.UseCases;

public sealed class CompareEmbeddingsUseCase
{
    private readonly IEmbeddingGenerator _embeddingGenerator;
    private readonly IVectorSimilarity _vectorSimilarity;

    public CompareEmbeddingsUseCase(
        IEmbeddingGenerator embeddingGenerator,
        IVectorSimilarity vectorSimilarity)
    {
        _embeddingGenerator = embeddingGenerator;
        _vectorSimilarity = vectorSimilarity;
    }

    public async Task<CompareEmbeddingsResponse> HandleAsync(
        CompareEmbeddingsRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.FirstText))
            throw new ArgumentException("O primeiro texto é obrigatório.");

        if (string.IsNullOrWhiteSpace(request.SecondText))
            throw new ArgumentException("O segundo texto é obrigatório.");

        var firstEmbedding = await _embeddingGenerator.GenerateAsync(
            request.FirstText,
            cancellationToken
        );

        var secondEmbedding = await _embeddingGenerator.GenerateAsync(
            request.SecondText,
            cancellationToken
        );

        var similarity = _vectorSimilarity.CosineSimilarity(
            firstEmbedding.Vector,
            secondEmbedding.Vector
        );

        return new CompareEmbeddingsResponse(
            request.FirstText,
            request.SecondText,
            firstEmbedding.Model,
            firstEmbedding.Vector.Dimensions,
            similarity,
            InterpretSimilarity(similarity),
            firstEmbedding.TotalTokens,
            secondEmbedding.TotalTokens
        );
    }

    private static string InterpretSimilarity(double similarity)
    {
        return similarity switch
        {
            >= 0.85 => "Os textos são semanticamente muito próximos.",
            >= 0.70 => "Os textos possuem relação semântica relevante.",
            >= 0.50 => "Os textos possuem alguma proximidade semântica.",
            _ => "Os textos parecem semanticamente distantes."
        };
    }
}