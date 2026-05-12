namespace SupportFlowAI.Application.Prompts;

public sealed class PromptTemplateBuilder
{
    public (string SystemPrompt, string FinalPrompt) Build(
        string task,
        string input,
        PromptStyle style)
    {
        return style switch
        {
            PromptStyle.ZeroShot => BuildZeroShot(task, input),
            PromptStyle.RoleBased => BuildRoleBased(task, input),
            PromptStyle.FewShot => BuildFewShot(task, input),
            PromptStyle.StructuredJson => BuildStructuredJson(task, input),
            _ => BuildZeroShot(task, input)
        };
    }

    private static (string SystemPrompt, string FinalPrompt) BuildZeroShot(
        string task,
        string input)
    {
        var systemPrompt = """
        Você é um assistente de IA especializado em análise de chamados corporativos.
        Responda de forma objetiva e tecnicamente correta.
        """;

        var finalPrompt = $"""
        Tarefa:
        {task}

        Entrada:
        {input}
        """;

        return (systemPrompt, finalPrompt);
    }

    private static (string SystemPrompt, string FinalPrompt) BuildRoleBased(
        string task,
        string input)
    {
        var systemPrompt = """
        Você é um arquiteto de software sênior especialista em .NET e IA aplicada.
        Analise a entrada considerando impacto técnico, clareza, risco e ação recomendada.
        """;

        var finalPrompt = $"""
        Execute a tarefa abaixo assumindo o papel definido no system prompt.

        Tarefa:
        {task}

        Entrada:
        {input}
        """;

        return (systemPrompt, finalPrompt);
    }

    private static (string SystemPrompt, string FinalPrompt) BuildFewShot(
        string task,
        string input)
    {
        var systemPrompt = """
        Você é um classificador de chamados. Use os exemplos como referência.
        Responda seguindo o padrão demonstrado.
        """;

        var finalPrompt = $"""
        Exemplos:

        Entrada: "Não consigo acessar o sistema, minha senha aparece como inválida."
        Saída: Categoria: Access | Prioridade: High | Sentimento: Negative

        Entrada: "Preciso da segunda via da nota fiscal."
        Saída: Categoria: Finance | Prioridade: Medium | Sentimento: Neutral

        Entrada: "O sistema está retornando erro 500 para todos os usuários."
        Saída: Categoria: Infrastructure | Prioridade: Critical | Sentimento: Negative

        Agora execute a tarefa:

        Tarefa:
        {task}

        Entrada:
        {input}
        """;

        return (systemPrompt, finalPrompt);
    }

    private static (string SystemPrompt, string FinalPrompt) BuildStructuredJson(
        string task,
        string input)
    {
        var systemPrompt = """
        Você é um assistente corporativo.
        Retorne exclusivamente um JSON válido, sem markdown e sem texto adicional.
        """;

        var finalPrompt = $$"""
        Tarefa:
        {{task}}

        Entrada:
        {{input}}

        Retorne no formato:
        {
            "category": "Access | Finance | Infrastructure | TechnicalSupport | HumanResources",
            "priority": "Low | Medium | High | Critical",
            "sentiment": "Neutral | Positive | Negative",
            "summary": "Resumo curto",
            "recommendedAction": "Ação recomendada"
        }
        """;

        return (systemPrompt, finalPrompt);
    }
}