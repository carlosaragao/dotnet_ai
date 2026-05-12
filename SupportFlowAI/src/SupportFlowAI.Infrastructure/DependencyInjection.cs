using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SupportFlowAI.Application.Interfaces;
using SupportFlowAI.Application.Prompts;
using SupportFlowAI.Application.UseCases;
using SupportFlowAI.Infrastructure.OpenAI;
using SupportFlowAI.Infrastructure.Repositories;
using SupportFlowAI.Application.ML;
using SupportFlowAI.Infrastructure.ML;

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
        services.AddSingleton<TicketMlModelPaths>();

        services.AddHttpClient<IAiTextGenerator, OpenAiTextGenerator>(ConfigureOpenAiClient);

        services.AddScoped<IAiTicketAnalyzer, OpenAiTicketAnalyzer>();

        services.AddHttpClient<IEmbeddingGenerator, OpenAiEmbeddingGenerator>(ConfigureOpenAiClient);

        services.AddSingleton<IVectorSimilarity, CosineVectorSimilarity>();

        services.AddSingleton<PromptTemplateBuilder>();
        services.AddSingleton<IAiModelSettings, OpenAiModelSettings>();

        services.AddScoped<ITicketMlModelTrainer, MlNetTicketModelTrainer>();
        services.AddScoped<ITicketMlPredictor, MlNetTicketPredictor>();

        services.AddScoped<TrainTicketMlModelsUseCase>();
        services.AddScoped<PredictTicketWithMlUseCase>();

        services.AddScoped<CreateTicketUseCase>();
        services.AddScoped<GetTicketByIdUseCase>();
        services.AddScoped<ListTicketsUseCase>();
        services.AddScoped<AnalyzeTicketUseCase>();
        services.AddScoped<ExplainLlmConceptUseCase>();
        services.AddScoped<CompareEmbeddingsUseCase>();
        services.AddScoped<ExecutePromptExperimentUseCase>();
        services.AddScoped<ListPromptLabModelsUseCase>();
        
        
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