using Microsoft.AspNetCore.Mvc;
using SupportFlowAI.Application.DTOs.Media;
using SupportFlowAI.Application.Interfaces;
using SupportFlowAI.Application.UseCases;

namespace SupportFlowAI.Api.Controllers;

[ApiController]
[Route("api/media")]
public sealed class MediaProcessingController : ControllerBase
{
    private readonly IOcrService _ocrService;
    private readonly ISpeechToTextService _speechToTextService;
    private readonly CreateTicketFromImageUseCase _createTicketFromImageUseCase;
    private readonly CreateTicketFromAudioUseCase _createTicketFromAudioUseCase;

    public MediaProcessingController(
        IOcrService ocrService,
        ISpeechToTextService speechToTextService,
        CreateTicketFromImageUseCase createTicketFromImageUseCase,
        CreateTicketFromAudioUseCase createTicketFromAudioUseCase)
    {
        _ocrService = ocrService;
        _speechToTextService = speechToTextService;
        _createTicketFromImageUseCase = createTicketFromImageUseCase;
        _createTicketFromAudioUseCase = createTicketFromAudioUseCase;
    }

    [HttpPost("image/ocr")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<OcrResponse>> ExtractTextFromImageAsync(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file.Length == 0)
            return BadRequest(new { error = "Arquivo inválido." });

        await using var stream = file.OpenReadStream();

        var result = await _ocrService.ExtractTextAsync(
            stream,
            cancellationToken
        );

        return Ok(new OcrResponse(
            result.ExtractedText,
            result.Lines,
            result.Provider
        ));
    }

    [HttpPost("audio/transcribe")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<SpeechToTextResponse>> TranscribeAudioAsync(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file.Length == 0)
            return BadRequest(new { error = "Arquivo inválido." });

        await using var stream = file.OpenReadStream();

        var result = await _speechToTextService.TranscribeAsync(
            stream,
            file.FileName,
            cancellationToken
        );

        return Ok(new SpeechToTextResponse(
            result.Transcription,
            result.Language,
            result.Provider
        ));
    }

    [HttpPost("image/create-ticket")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<CreateTicketFromMediaResponse>> CreateTicketFromImageAsync(
        IFormFile file,
        [FromForm] string requestedBy,
        CancellationToken cancellationToken)
    {
        if (file.Length == 0)
            return BadRequest(new { error = "Arquivo inválido." });

        await using var stream = file.OpenReadStream();

        var response = await _createTicketFromImageUseCase.HandleAsync(
            stream,
            requestedBy,
            cancellationToken
        );

        return Ok(response);
    }

    [HttpPost("audio/create-ticket")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<CreateTicketFromMediaResponse>> CreateTicketFromAudioAsync(
        IFormFile file,
        [FromForm] string requestedBy,
        CancellationToken cancellationToken)
    {
        if (file.Length == 0)
            return BadRequest(new { error = "Arquivo inválido." });

        await using var stream = file.OpenReadStream();

        var response = await _createTicketFromAudioUseCase.HandleAsync(
            stream,
            file.FileName,
            requestedBy,
            cancellationToken
        );

        return Ok(response);
    }
}