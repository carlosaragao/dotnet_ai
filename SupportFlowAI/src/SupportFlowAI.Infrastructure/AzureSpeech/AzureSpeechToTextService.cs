using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Options;
using SupportFlowAI.Application.AI.Media;
using SupportFlowAI.Application.Interfaces;

namespace SupportFlowAI.Infrastructure.AzureSpeech;

public sealed class AzureSpeechToTextService : ISpeechToTextService
{
    private readonly AzureSpeechOptions _options;

    public AzureSpeechToTextService(IOptions<AzureSpeechOptions> options)
    {
        _options = options.Value;

        if (string.IsNullOrWhiteSpace(_options.Key))
            throw new InvalidOperationException("Azure Speech Key não configurada.");

        if (string.IsNullOrWhiteSpace(_options.Region))
            throw new InvalidOperationException("Azure Speech Region não configurada.");
    }

    public async Task<SpeechToTextResult> TranscribeAsync(
        Stream audioStream,
        string fileName,
        CancellationToken cancellationToken)
    {
        if (audioStream is null || !audioStream.CanRead)
            throw new ArgumentException("Áudio inválido.");

        var tempFilePath = Path.Combine(
            Path.GetTempPath(),
            $"{Guid.NewGuid()}_{fileName}"
        );

        await using (var fileStream = File.Create(tempFilePath))
        {
            await audioStream.CopyToAsync(fileStream, cancellationToken);
        }

        try
        {
            var speechConfig = SpeechConfig.FromSubscription(
                _options.Key,
                _options.Region
            );

            speechConfig.SpeechRecognitionLanguage = _options.Language;

            using var audioConfig = AudioConfig.FromWavFileInput(tempFilePath);
            using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

            var result = await recognizer.RecognizeOnceAsync();

            if (result.Reason == ResultReason.RecognizedSpeech)
            {
                return new SpeechToTextResult(
                    result.Text,
                    _options.Language,
                    "Azure AI Speech"
                );
            }

            if (result.Reason == ResultReason.NoMatch)
            {
                return new SpeechToTextResult(
                    "Nenhuma fala foi reconhecida no áudio.",
                    _options.Language,
                    "Azure AI Speech"
                );
            }

            if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = CancellationDetails.FromResult(result);

                throw new InvalidOperationException(
                    $"Transcrição cancelada. Reason: {cancellation.Reason}. Details: {cancellation.ErrorDetails}"
                );
            }

            return new SpeechToTextResult(
                result.Text,
                _options.Language,
                "Azure AI Speech"
            );
        }
        finally
        {
            if (File.Exists(tempFilePath))
                File.Delete(tempFilePath);
        }
    }
}