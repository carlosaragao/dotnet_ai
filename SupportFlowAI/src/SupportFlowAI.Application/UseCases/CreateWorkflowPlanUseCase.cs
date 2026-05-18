using System.Text.Json;
using SupportFlowAI.Application.DTOs.FoundryAI;
using SupportFlowAI.Application.DTOs.Workflows;
using SupportFlowAI.Application.Interfaces;
using SupportFlowAI.Application.Workflows;

namespace SupportFlowAI.Application.UseCases;

public sealed class CreateWorkflowPlanUseCase
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IFoundryAiService _foundryAiService;

    public CreateWorkflowPlanUseCase(IFoundryAiService foundryAiService)
    {
        _foundryAiService = foundryAiService;
    }

    public async Task<WorkflowPlanResponse> HandleAsync(
        WorkflowPlanRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Goal))
            throw new ArgumentException("O objetivo do workflow é obrigatório.");

        var instructions = """
        Você é um planner de workflows corporativos.

        Sua tarefa é gerar um plano JSON válido para o Semantic Kernel executar.
        Não retorne markdown.
        Não retorne explicações fora do JSON.

        Plugins disponíveis:

        1. Tickets.get_ticket
        Argumentos:
        - ticketId

        2. Tickets.list_open_tickets
        Argumentos:
        - nenhum

        3. AI.summarize_ticket
        Argumentos:
        - ticketId

        4. AI.recommend_next_action
        Argumentos:
        - ticketId

        5. AI.generate_ticket_answer
        Argumentos:
        - ticketId

        6. Memory.search_memory
        Argumentos:
        - query
        - topK

        7. Memory.save_memory
        Argumentos:
        - content

        O JSON deve seguir exatamente este formato:
        {
        "goal": "objetivo do usuário",
        "steps": [
            {
            "order": 1,
            "plugin": "Tickets",
            "function": "get_ticket",
            "arguments": {
                "ticketId": "guid"
            },
            "reason": "motivo do passo"
            }
        ]
        }

        Regras:
        - Se existir ticketId, use-o nos passos relacionados ao chamado.
        - Para criar uma triagem completa, use get_ticket, summarize_ticket, recommend_next_action e generate_ticket_answer.
        - Ao final, salve algo relevante em Memory.save_memory.
        - Para Memory.save_memory, se depender do resultado anterior, use "$lastResult" como content.
        - Os argumentos podem ser strings ou números.
        - Use número para argumentos numéricos, como topK.
        - Use string para argumentos textuais, como ticketId, query e content.
        """;

        var input = $"""
        Objetivo:
        {request.Goal}

        TicketId:
        {request.TicketId}

        Contexto adicional:
        {request.AdditionalContext}
        """;

        var response = await _foundryAiService.GenerateAsync(
            new FoundryAiCompletionRequest(
                Input: input,
                Instructions: instructions,
                Temperature: 0.1,
                MaxOutputTokens: 900
            ),
            cancellationToken
        );

        var rawPlan = CleanJson(response.Response);

        var plan = JsonSerializer.Deserialize<WorkflowPlan>(
            rawPlan,
            JsonOptions
        );

        if (plan is null)
            throw new InvalidOperationException("Não foi possível interpretar o plano gerado pelo modelo.");

        return new WorkflowPlanResponse(plan, rawPlan);
    }

    private static string CleanJson(string content)
    {
        return content
            .Replace("```json", string.Empty)
            .Replace("```", string.Empty)
            .Trim();
    }
}