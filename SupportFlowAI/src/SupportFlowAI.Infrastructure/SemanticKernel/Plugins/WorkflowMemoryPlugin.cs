using System.ComponentModel;
using System.Text.Json;
using Microsoft.SemanticKernel;
using SupportFlowAI.Application.Interfaces;

namespace SupportFlowAI.Infrastructure.SemanticKernel.Plugins;

public sealed class WorkflowMemoryPlugin
{
    private readonly IWorkflowMemoryStore _memoryStore;

    public WorkflowMemoryPlugin(IWorkflowMemoryStore memoryStore)
    {
        _memoryStore = memoryStore;
    }

    [KernelFunction("save_memory")]
    [Description("Salva uma informação relevante na memória semântica do workflow.")]
    public async Task<string> SaveMemoryAsync(
        [Description("Conteúdo que deve ser armazenado na memória.")] string content,
        CancellationToken cancellationToken)
    {
        await _memoryStore.SaveAsync(content, cancellationToken);

        return "Memória salva com sucesso.";
    }

    [KernelFunction("search_memory")]
    [Description("Busca informações semanticamente relacionadas na memória do workflow.")]
    public async Task<string> SearchMemoryAsync(
        [Description("Consulta textual para buscar memórias relacionadas.")] string query,
        [Description("Quantidade máxima de resultados.")] int topK,
        CancellationToken cancellationToken)
    {
        var results = await _memoryStore.SearchAsync(
            query,
            topK,
            cancellationToken
        );

        return JsonSerializer.Serialize(results, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }
}