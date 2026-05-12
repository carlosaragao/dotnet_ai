using SupportFlowAI.Application.ML;

namespace SupportFlowAI.Infrastructure.ML.Data;

public static class TicketTrainingDataFactory
{
    public static IReadOnlyCollection<TicketCategoryTrainingData> CreateCategoryData()
    {
        return
        [
            new() { Text = "Não consigo fazer login no sistema, senha inválida.", Category = "Access" },
            new() { Text = "Usuário bloqueado após várias tentativas de autenticação.", Category = "Access" },
            new() { Text = "Preciso redefinir minha senha corporativa.", Category = "Access" },
            new() { Text = "Erro no segundo fator de autenticação.", Category = "Access" },

            new() { Text = "Preciso da segunda via da nota fiscal.", Category = "Finance" },
            new() { Text = "Boleto não foi gerado para pagamento.", Category = "Finance" },
            new() { Text = "Cobrança duplicada na fatura.", Category = "Finance" },
            new() { Text = "Dúvida sobre pagamento do serviço contratado.", Category = "Finance" },

            new() { Text = "Sistema está retornando erro 500.", Category = "Infrastructure" },
            new() { Text = "Aplicação indisponível para todos os usuários.", Category = "Infrastructure" },
            new() { Text = "Timeout ao acessar o servidor interno.", Category = "Infrastructure" },
            new() { Text = "Instabilidade geral na rede.", Category = "Infrastructure" },

            new() { Text = "Não consigo exportar o relatório.", Category = "TechnicalSupport" },
            new() { Text = "Tela apresenta erro ao salvar cadastro.", Category = "TechnicalSupport" },
            new() { Text = "Funcionalidade de busca não retorna resultados.", Category = "TechnicalSupport" },
            new() { Text = "Erro ao atualizar dados do cliente.", Category = "TechnicalSupport" },

            new() { Text = "Preciso atualizar meus dados cadastrais de colaborador.", Category = "HumanResources" },
            new() { Text = "Dúvida sobre férias e benefícios.", Category = "HumanResources" },
            new() { Text = "Solicitação de comprovante de vínculo empregatício.", Category = "HumanResources" },
            new() { Text = "Problema ao acessar informe de rendimentos.", Category = "HumanResources" }
        ];
    }

    public static IReadOnlyCollection<TicketEffortTrainingData> CreateEffortData()
    {
        return
        [
            new() { Text = "Redefinir senha de usuário.", Category = "Access", Priority = "Low", EffortHours = 1 },
            new() { Text = "Usuário bloqueado sem acesso ao sistema.", Category = "Access", Priority = "High", EffortHours = 3 },
            new() { Text = "Falha geral de autenticação afetando vários usuários.", Category = "Access", Priority = "Critical", EffortHours = 8 },

            new() { Text = "Segunda via de nota fiscal.", Category = "Finance", Priority = "Low", EffortHours = 2 },
            new() { Text = "Cobrança duplicada para cliente corporativo.", Category = "Finance", Priority = "Medium", EffortHours = 4 },
            new() { Text = "Erro em lote de faturamento mensal.", Category = "Finance", Priority = "High", EffortHours = 10 },

            new() { Text = "Erro 500 em endpoint interno.", Category = "Infrastructure", Priority = "High", EffortHours = 6 },
            new() { Text = "Sistema indisponível para todos os usuários.", Category = "Infrastructure", Priority = "Critical", EffortHours = 16 },
            new() { Text = "Timeout intermitente em servidor de aplicação.", Category = "Infrastructure", Priority = "Medium", EffortHours = 5 },

            new() { Text = "Erro ao exportar relatório.", Category = "TechnicalSupport", Priority = "Medium", EffortHours = 3 },
            new() { Text = "Falha ao salvar cadastro.", Category = "TechnicalSupport", Priority = "Medium", EffortHours = 4 },
            new() { Text = "Busca não retorna dados esperados.", Category = "TechnicalSupport", Priority = "Low", EffortHours = 2 },

            new() { Text = "Dúvida sobre férias.", Category = "HumanResources", Priority = "Low", EffortHours = 1 },
            new() { Text = "Erro no portal de benefícios.", Category = "HumanResources", Priority = "Medium", EffortHours = 3 },
            new() { Text = "Problema no informe de rendimentos.", Category = "HumanResources", Priority = "High", EffortHours = 5 }
        ];
    }
}