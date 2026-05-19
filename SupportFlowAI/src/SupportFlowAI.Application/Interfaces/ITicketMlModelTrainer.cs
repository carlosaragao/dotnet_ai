namespace SupportFlowAI.Application.Interfaces;

public interface ITicketMlModelTrainer
{
    Task TrainAsync(CancellationToken cancellationToken);
}