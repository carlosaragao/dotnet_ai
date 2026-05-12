using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SupportFlowAI.Application.Interfaces;
using SupportFlowAI.Application.UseCases;
using SupportFlowAI.Infrastructure.OpenAI;
using SupportFlowAI.Infrastructure.Repositories;

namespace SupportFlowAI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSupportFlowInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<OpenAiOptions>(options =>
            configuration.GetSection(OpenAiOptions.SectionName).Bind(options)
        );

        services.PostConfigure<OpenAiOptions>(options =>
        {
            if (string.IsNullOrWhiteSpace(options.ApiKey))
                options.ApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? string.Empty;

            if (string.IsNullOrWhiteSpace(options.ApiKey))
                throw new InvalidOperationException("A variável OPENAI_API_KEY não foi configurada.");
        });

        services.AddSingleton<ITicketRepository, InMemoryTicketRepository>();

        services.AddHttpClient<IAiTextGenerator, OpenAiTextGenerator>(ConfigureOpenAiClient);

        services.AddScoped<IAiTicketAnalyzer, OpenAiTicketAnalyzer>();

        services.AddScoped<CreateTicketUseCase>();
        services.AddScoped<GetTicketByIdUseCase>();
        services.AddScoped<ListTicketsUseCase>();
        services.AddScoped<AnalyzeTicketUseCase>();

        return services;
    }

    private static void ConfigureOpenAiClient(IServiceProvider provider, HttpClient client)
    {
        var options = provider.GetRequiredService<IOptions<OpenAiOptions>>().Value;

        client.BaseAddress = new Uri(options.BaseUrl);
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", options.ApiKey);
    }
}