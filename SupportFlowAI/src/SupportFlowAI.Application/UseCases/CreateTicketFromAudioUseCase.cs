using SupportFlowAI.Application.DTOs.Media;
using SupportFlowAI.Application.Interfaces;
using SupportFlowAI.Domain.Entities;

namespace SupportFlowAI.Application.UseCases;

public sealed class CreateTicketFromAudioUseCase
{
    private readonly ISpeechToTextService _speechToTextService;
    private readonly ITicketRepository _ticketRepository;

    public CreateTicketFromAudioUseCase(
        ISpeechToTextService speechToTextService,
        ITicketRepository ticketRepository)
    {
        _speechToTextService = speechToTextService;
        _ticketRepository = ticketRepository;
    }

    public async Task<CreateTicketFromMediaResponse> HandleAsync(
        Stream audioStream,
        string fileName,
        string requestedBy,
        CancellationToken cancellationToken)
    {
        var transcription = await _speechToTextService.TranscribeAsync(
            audioStream,
            fileName,
            cancellationToken
        );

        if (string.IsNullOrWhiteSpace(transcription.Transcription))
            throw new InvalidOperationException("Nenhuma transcrição foi gerada.");

        var ticket = new Ticket(
            title: "Chamado criado a partir de áudio",
            description: transcription.Transcription,
            requestedBy: requestedBy
        );

        await _ticketRepository.AddAsync(ticket, cancellationToken);

        return new CreateTicketFromMediaResponse(
            ticket.Id,
            ticket.Title,
            transcription.Transcription,
            "Audio/Speech-to-Text"
        );
    }
}