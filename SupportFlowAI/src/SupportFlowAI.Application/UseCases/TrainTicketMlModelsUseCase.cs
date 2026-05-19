using SupportFlowAI.Application.DTOs;
using SupportFlowAI.Application.Interfaces;
using SupportFlowAI.Application.ML;

namespace SupportFlowAI.Application.UseCases;

public sealed class TrainTicketMlModelsUseCase
{
    private readonly ITicketMlModelTrainer _trainer;
    private readonly TicketMlModelPaths _paths;

    public TrainTicketMlModelsUseCase(
        ITicketMlModelTrainer trainer,
        TicketMlModelPaths paths)
    {
        _trainer = trainer;
        _paths = paths;
    }

    public async Task<TrainMlModelsResponse> HandleAsync(
        CancellationToken cancellationToken)
    {
        await _trainer.TrainAsync(cancellationToken);

        return new TrainMlModelsResponse(
            "Modelos ML.NET treinados com sucesso.",
            _paths.CategoryModelPath,
            _paths.EffortModelPath
        );
    }
}