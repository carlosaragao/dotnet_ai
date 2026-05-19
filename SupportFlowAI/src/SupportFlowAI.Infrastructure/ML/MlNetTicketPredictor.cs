using Microsoft.ML;
using SupportFlowAI.Application.DTOs;
using SupportFlowAI.Application.Interfaces;
using SupportFlowAI.Application.ML;
using SupportFlowAI.Domain.Enums;

namespace SupportFlowAI.Infrastructure.ML;

public sealed class MlNetTicketPredictor : ITicketMlPredictor
{
    private readonly ITicketRepository _ticketRepository;
    private readonly TicketMlModelPaths _paths;

    public MlNetTicketPredictor(
        ITicketRepository ticketRepository,
        TicketMlModelPaths paths)
    {
        _ticketRepository = ticketRepository;
        _paths = paths;
    }

    public async Task<TicketMlPredictionResponse> PredictAsync(
        Guid ticketId,
        CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId, cancellationToken);

        if (ticket is null)
            throw new KeyNotFoundException("Chamado não encontrado.");

        if (!File.Exists(_paths.CategoryModelPath) || !File.Exists(_paths.EffortModelPath))
            throw new InvalidOperationException("Os modelos ML.NET ainda não foram treinados.");

        var mlContext = new MLContext();

        var categoryModel = mlContext.Model.Load(
            _paths.CategoryModelPath,
            out _);

        var effortModel = mlContext.Model.Load(
            _paths.EffortModelPath,
            out _);

        var categoryEngine = mlContext.Model.CreatePredictionEngine
            <TicketCategoryTrainingData, TicketCategoryPrediction>(categoryModel);

        var effortEngine = mlContext.Model.CreatePredictionEngine
            <TicketEffortTrainingData, TicketEffortPrediction>(effortModel);

        var categoryInput = new TicketCategoryTrainingData
        {
            Text = $"{ticket.Title}. {ticket.Description}"
        };

        var categoryPrediction = categoryEngine.Predict(categoryInput);

        var priority = ticket.Priority == TicketPriority.Critical
            ? "Critical"
            : ticket.Priority.ToString();

        var effortInput = new TicketEffortTrainingData
        {
            Text = $"{ticket.Title}. {ticket.Description}",
            Category = categoryPrediction.PredictedCategory,
            Priority = priority
        };

        var effortPrediction = effortEngine.Predict(effortInput);

        return new TicketMlPredictionResponse(
            ticket.Id,
            ticket.Title,
            ticket.Description,
            categoryPrediction.PredictedCategory,
            MathF.Max(0, effortPrediction.EstimatedEffortHours),
            "ML.NET local model"
        );
    }
}