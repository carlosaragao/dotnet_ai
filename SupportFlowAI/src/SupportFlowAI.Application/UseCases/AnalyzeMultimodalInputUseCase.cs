using SupportFlowAI.Application.DTOs.FoundryAI;
using SupportFlowAI.Application.DTOs.Multimodal;
using SupportFlowAI.Application.Interfaces;

namespace SupportFlowAI.Application.UseCases;

public sealed class AnalyzeMultimodalInputUseCase
{
    private readonly IOcrService _ocrService;
    private readonly ISpeechToTextService _speechToTextService;
    private readonly IFoundryAiService _foundryAiService;

    public AnalyzeMultimodalInputUseCase(
        IOcrService ocrService,
        ISpeechToTextService speechToTextService,
        IFoundryAiService foundryAiService)
    {
        _ocrService = ocrService;
        _speechToTextService = speechToTextService;
        _foundryAiService = foundryAiService;
    }

    public async Task<MultimodalAnalysisResponse> HandleAsync(
        string? inputText,
        Stream? imageStream,
        Stream? audioStream,
        string? audioFileName,
        CancellationToken cancellationToken)
    {
        string? extractedImageText = null;
        string? audioTranscription = null;

        if (imageStream is not null)
        {
            var ocr = await _ocrService.ExtractTextAsync(
                imageStream,
                cancellationToken
            );

            extractedImageText = ocr.ExtractedText;
        }

        if (audioStream is not null && !string.IsNullOrWhiteSpace(audioFileName))
        {
            var transcription = await _speechToTextService.TranscribeAsync(
                audioStream,
                audioFileName,
                cancellationToken
            );

            audioTranscription = transcription.Transcription;
        }

        if (string.IsNullOrWhiteSpace(inputText)
            && string.IsNullOrWhiteSpace(extractedImageText)
            && string.IsNullOrWhiteSpace(audioTranscription))
        {
            throw new ArgumentException("É necessário informar texto, imagem ou áudio.");
        }

        var instructions = """
        Você é um analista corporativo de suporte técnico especializado em interpretar múltiplas fontes de informação.

        Você receberá texto digitado, texto extraído de imagem por OCR e/ou transcrição de áudio.
        Sua tarefa é consolidar tudo em uma análise única.

        Responda em português, com estrutura clara e objetiva.
        """;

        var prompt = $"""
        Analise as informações abaixo.

        Texto informado pelo usuário:
        {inputText}

        Texto extraído da imagem por OCR:
        {extractedImageText}

        Transcrição do áudio:
        {audioTranscription}

        Gere:
        - resumo consolidado;
        - problema principal;
        - categoria provável;
        - prioridade provável;
        - evidências encontradas em cada mídia;
        - recomendação de próxima ação;
        - resposta sugerida ao usuário.
        """;

        var foundryResponse = await _foundryAiService.GenerateAsync(
            new FoundryAiCompletionRequest(
                Input: prompt,
                Instructions: instructions,
                Temperature: 0.2,
                MaxOutputTokens: 700
            ),
            cancellationToken
        );

        return new MultimodalAnalysisResponse(
            inputText,
            extractedImageText,
            audioTranscription,
            foundryResponse.Response,
            foundryResponse.Provider,
            foundryResponse.InputTokens,
            foundryResponse.OutputTokens,
            foundryResponse.TotalTokens
        );
    }
}