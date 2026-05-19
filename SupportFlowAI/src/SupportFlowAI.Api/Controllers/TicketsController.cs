using Microsoft.AspNetCore.Mvc;
using SupportFlowAI.Application.DTOs;
using SupportFlowAI.Application.UseCases;

namespace SupportFlowAI.Api.Controllers;

[ApiController]
[Route("api/tickets")]
public sealed class TicketsController : ControllerBase
{
    private readonly CreateTicketUseCase _createTicketUseCase;
    private readonly GetTicketByIdUseCase _getTicketByIdUseCase;
    private readonly ListTicketsUseCase _listTicketsUseCase;
    private readonly AnalyzeTicketUseCase _analyzeTicketUseCase;

    public TicketsController(
        CreateTicketUseCase createTicketUseCase,
        GetTicketByIdUseCase getTicketByIdUseCase,
        ListTicketsUseCase listTicketsUseCase,
        AnalyzeTicketUseCase analyzeTicketUseCase)
    {
        _createTicketUseCase = createTicketUseCase;
        _getTicketByIdUseCase = getTicketByIdUseCase;
        _listTicketsUseCase = listTicketsUseCase;
        _analyzeTicketUseCase = analyzeTicketUseCase;
    }

    [HttpPost]
    public async Task<ActionResult<TicketResponse>> CreateAsync(
        [FromBody] CreateTicketRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _createTicketUseCase.HandleAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetByIdAsync),
            new { id = response.Id },
            response
        );
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TicketResponse>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await _getTicketByIdUseCase.HandleAsync(id, cancellationToken);

        if (response is null)
            return NotFound();

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<TicketResponse>>> ListAsync(
        CancellationToken cancellationToken)
    {
        var response = await _listTicketsUseCase.HandleAsync(cancellationToken);

        return Ok(response);
    }

    [HttpPost("{id:guid}/analyze-basic")]
    public async Task<ActionResult<AnalyzeTicketResponse>> AnalyzeBasicAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _analyzeTicketUseCase.HandleAsync(id, cancellationToken);
            return Ok(response);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}