using SupportFlowAI.Application.DTOs.Media;
using SupportFlowAI.Application.Interfaces;
using SupportFlowAI.Domain.Entities;

namespace SupportFlowAI.Application.UseCases;

public sealed class CreateTicketFromImageUseCase
{
    private readonly IOcrService _ocrService;
    private readonly ITicketRepository _ticketRepository;

    public CreateTicketFromImageUseCase(
        IOcrService ocrService,
        ITicketRepository ticketRepository)
    {
        _ocrService = ocrService;
        _ticketRepository = ticketRepository;
    }

    public async Task<CreateTicketFromMediaResponse> HandleAsync(
        Stream imageStream,
        string requestedBy,
        CancellationToken cancellationToken)
    {
        var ocr = await _ocrService.ExtractTextAsync(
            imageStream,
            cancellationToken
        );

        if (string.IsNullOrWhiteSpace(ocr.ExtractedText))
            throw new InvalidOperationException("Nenhum texto foi encontrado na imagem.");

        var ticket = new Ticket(
            title: "Chamado criado a partir de imagem",
            description: ocr.ExtractedText,
            requestedBy: requestedBy
        );

        await _ticketRepository.AddAsync(ticket, cancellationToken);

        return new CreateTicketFromMediaResponse(
            ticket.Id,
            ticket.Title,
            ocr.ExtractedText,
            "Image/OCR"
        );
    }
}