using Microsoft.Extensions.DependencyInjection;
using SupportFlowAI.Application.Interfaces;
using SupportFlowAI.Application.UseCases;
using SupportFlowAI.Infrastructure.AI;
using SupportFlowAI.Infrastructure.Repositories;

namespace SupportFlowAI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSupportFlowInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ITicketRepository, InMemoryTicketRepository>();

        services.AddScoped<IAiTicketAnalyzer, FakeAiTicketAnalyzer>();

        services.AddScoped<CreateTicketUseCase>();
        services.AddScoped<GetTicketByIdUseCase>();
        services.AddScoped<ListTicketsUseCase>();
        services.AddScoped<AnalyzeTicketUseCase>();

        return services;
    }
}