using Azure;
using Azure.AI.Vision.ImageAnalysis;
using Microsoft.Extensions.Options;
using SupportFlowAI.Application.AI.Media;
using SupportFlowAI.Application.Interfaces;

namespace SupportFlowAI.Infrastructure.AzureVision;

public sealed class AzureVisionOcrService : IOcrService
{
    private readonly ImageAnalysisClient _client;

    public AzureVisionOcrService(IOptions<AzureVisionOptions> options)
    {
        var value = options.Value;

        if (string.IsNullOrWhiteSpace(value.Endpoint))
            throw new InvalidOperationException("Azure Vision Endpoint não configurado.");

        if (string.IsNullOrWhiteSpace(value.Key))
            throw new InvalidOperationException("Azure Vision Key não configurada.");

        _client = new ImageAnalysisClient(
            new Uri(value.Endpoint),
            new AzureKeyCredential(value.Key)
        );
    }

    public async Task<OcrResult> ExtractTextAsync(
        Stream imageStream,
        CancellationToken cancellationToken)
    {
        if (imageStream is null || !imageStream.CanRead)
            throw new ArgumentException("Imagem inválida.");

        using var memoryStream = new MemoryStream();
        await imageStream.CopyToAsync(memoryStream, cancellationToken);

        var imageData = BinaryData.FromBytes(memoryStream.ToArray());

        var result = await _client.AnalyzeAsync(
            imageData,
            VisualFeatures.Read,
            cancellationToken: cancellationToken
        );

        var lines = result.Value.Read?.Blocks
            .SelectMany(block => block.Lines)
            .Select(line => line.Text)
            .Where(text => !string.IsNullOrWhiteSpace(text))
            .ToArray() ?? [];

        var extractedText = string.Join(Environment.NewLine, lines);

        return new OcrResult(
            extractedText,
            lines,
            "Azure AI Vision"
        );
    }
}