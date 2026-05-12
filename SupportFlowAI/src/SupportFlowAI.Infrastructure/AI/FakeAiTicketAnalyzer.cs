using SupportFlowAI.Application.Interfaces;
using SupportFlowAI.Domain.Entities;
using SupportFlowAI.Domain.Enums;
using SupportFlowAI.Domain.ValueObjects;

namespace SupportFlowAI.Infrastructure.AI;

public sealed class FakeAiTicketAnalyzer : IAiTicketAnalyzer
{
    public Task<TicketAnalysis> AnalyzeAsync(
        Ticket ticket,
        CancellationToken cancellationToken)
    {
        var description = ticket.Description.ToLowerInvariant();

        if (ContainsAny(description, "senha", "login", "acesso", "credencial", "autenticação"))
        {
            return Task.FromResult(new TicketAnalysis(
                TicketCategory.Access,
                TicketPriority.High,
                SentimentType.Negative,
                0.91,
                "O chamado indica dificuldade de acesso ao sistema ou problema de autenticação.",
                "Olá! Identificamos que seu chamado está relacionado a acesso. Recomendamos validar suas credenciais, redefinir a senha e, se o erro persistir, encaminhar para a equipe de identidade e acessos.",
                "fake-rule-based-ai-v1"
            ));
        }

        if (ContainsAny(description, "pagamento", "boleto", "fatura", "cobrança", "nota fiscal"))
        {
            return Task.FromResult(new TicketAnalysis(
                TicketCategory.Finance,
                TicketPriority.Medium,
                SentimentType.Neutral,
                0.84,
                "O chamado parece estar relacionado a dúvidas ou problemas financeiros.",
                "Olá! Seu chamado foi classificado como financeiro. Vamos direcioná-lo para a equipe responsável por pagamentos, cobranças e faturamento.",
                "fake-rule-based-ai-v1"
            ));
        }

        if (ContainsAny(description, "servidor", "rede", "instabilidade", "timeout", "indisponível", "erro 500"))
        {
            return Task.FromResult(new TicketAnalysis(
                TicketCategory.Infrastructure,
                TicketPriority.Critical,
                SentimentType.Negative,
                0.88,
                "O chamado indica possível falha de infraestrutura ou indisponibilidade sistêmica.",
                "Olá! Identificamos possível incidente de infraestrutura. O chamado será priorizado e encaminhado para análise técnica.",
                "fake-rule-based-ai-v1"
            ));
        }

        return Task.FromResult(new TicketAnalysis(
            TicketCategory.TechnicalSupport,
            TicketPriority.Medium,
            SentimentType.Neutral,
            0.72,
            "O chamado foi classificado como suporte técnico geral.",
            "Olá! Recebemos sua solicitação e ela será analisada pela equipe de suporte técnico.",
            "fake-rule-based-ai-v1"
        ));
    }

    private static bool ContainsAny(string source, params string[] terms)
    {
        return terms.Any(source.Contains);
    }
}