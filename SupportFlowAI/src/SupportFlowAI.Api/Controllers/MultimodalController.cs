using Microsoft.AspNetCore.Mvc;
using SupportFlowAI.Application.DTOs.Multimodal;
using SupportFlowAI.Application.UseCases;

namespace SupportFlowAI.Api.Controllers;

[ApiController]
[Route("api/multimodal")]
public sealed class MultimodalController : ControllerBase
{
    private readonly AnalyzeMultimodalInputUseCase _analyzeUseCase;
    private readonly CreateTicketFromMultimodalInputUseCase _createTicketUseCase;

    public MultimodalController(
        AnalyzeMultimodalInputUseCase analyzeUseCase,
        CreateTicketFromMultimodalInputUseCase createTicketUseCase)
    {
        _analyzeUseCase = analyzeUseCase;
        _createTicketUseCase = createTicketUseCase;
    }

    [HttpPost("analyze")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<MultimodalAnalysisResponse>> AnalyzeAsync(
        [FromForm] string? inputText,
        IFormFile? image,
        IFormFile? audio,
        CancellationToken cancellationToken)
    {
        await using var imageStream = image is not null
            ? image.OpenReadStream()
            : null;

        await using var audioStream = audio is not null
            ? audio.OpenReadStream()
            : null;

        var response = await _analyzeUseCase.HandleAsync(
            inputText,
            imageStream,
            audioStream,
            audio?.FileName,
            cancellationToken
        );

        return Ok(response);
    }

    [HttpPost("create-ticket")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<CreateMultimodalTicketResponse>> CreateTicketAsync(
        [FromForm] string? inputText,
        IFormFile? image,
        IFormFile? audio,
        [FromForm] string requestedBy,
        CancellationToken cancellationToken)
    {
        await using var imageStream = image is not null
            ? image.OpenReadStream()
            : null;

        await using var audioStream = audio is not null
            ? audio.OpenReadStream()
            : null;

        var response = await _createTicketUseCase.HandleAsync(
            inputText,
            imageStream,
            audioStream,
            audio?.FileName,
            requestedBy,
            cancellationToken
        );

        return Ok(response);
    }
}