using SupportFlowAI.Application.AI;

namespace SupportFlowAI.Application.Interfaces;

public interface IWorkflowMemoryStore
{
    Task SaveAsync(string content, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<string>> SearchAsync(
        string query,
        int topK,
        CancellationToken cancellationToken);
}