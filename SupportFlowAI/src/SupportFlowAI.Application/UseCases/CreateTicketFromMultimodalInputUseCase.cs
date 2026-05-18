using SupportFlowAI.Application.DTOs.Multimodal;
using SupportFlowAI.Application.Interfaces;
using SupportFlowAI.Domain.Entities;

namespace SupportFlowAI.Application.UseCases;

public sealed class CreateTicketFromMultimodalInputUseCase
{
    private readonly AnalyzeMultimodalInputUseCase _analyzeMultimodalInputUseCase;
    private readonly ITicketRepository _ticketRepository;

    public CreateTicketFromMultimodalInputUseCase(
        AnalyzeMultimodalInputUseCase analyzeMultimodalInputUseCase,
        ITicketRepository ticketRepository)
    {
        _analyzeMultimodalInputUseCase = analyzeMultimodalInputUseCase;
        _ticketRepository = ticketRepository;
    }

    public async Task<CreateMultimodalTicketResponse> HandleAsync(
        string? inputText,
        Stream? imageStream,
        Stream? audioStream,
        string? audioFileName,
        string requestedBy,
        CancellationToken cancellationToken)
    {
        var analysis = await _analyzeMultimodalInputUseCase.HandleAsync(
            inputText,
            imageStream,
            audioStream,
            audioFileName,
            cancellationToken
        );

        var description = $"""
        Texto informado:
        {analysis.InputText}

        Texto extraído de imagem:
        {analysis.ExtractedImageText}

        Transcrição de áudio:
        {analysis.AudioTranscription}

        Análise consolidada:
        {analysis.ConsolidatedAnalysis}
        """;

        var ticket = new Ticket(
            "Chamado criado a partir de múltiplas mídias",
            description,
            requestedBy
        );

        await _ticketRepository.AddAsync(ticket, cancellationToken);

        return new CreateMultimodalTicketResponse(
            ticket.Id,
            ticket.Title,
            ticket.Description,
            analysis.ConsolidatedAnalysis
        );
    }
}