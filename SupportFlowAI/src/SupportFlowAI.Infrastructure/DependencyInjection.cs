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
using Microsoft.SemanticKernel;
using SupportFlowAI.Infrastructure.AzureAI;
using SupportFlowAI.Infrastructure.FoundryAI;
using SupportFlowAI.Application.AI;
using SupportFlowAI.Infrastructure.AzureVision;
using SupportFlowAI.Infrastructure.AzureSpeech;

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

        services.Configure<TokenBudgetOptions>(options =>
        {
            options.MaxEstimatedInputTokens = 3000;
            options.MaxOutputTokens = 700;
        });

        services.Configure<AzureLanguageOptions>(
            configuration.GetSection(AzureLanguageOptions.SectionName)
        );

        services.PostConfigure<AzureLanguageOptions>(options =>
{
            if (string.IsNullOrWhiteSpace(options.Endpoint))
                options.Endpoint = Environment.GetEnvironmentVariable("AZURE_AI_LANGUAGE_ENDPOINT") ?? string.Empty;

            if (string.IsNullOrWhiteSpace(options.Key))
                options.Key = Environment.GetEnvironmentVariable("AZURE_AI_LANGUAGE_KEY") ?? string.Empty;
        });

        services.Configure<FoundryAiOptions>(
            configuration.GetSection(FoundryAiOptions.SectionName)
        );

        services.PostConfigure<FoundryAiOptions>(options =>
        {
            if (string.IsNullOrWhiteSpace(options.Endpoint))
                options.Endpoint = Environment.GetEnvironmentVariable("FOUNDRY_AI_ENDPOINT") ?? string.Empty;

            if (string.IsNullOrWhiteSpace(options.ApiKey))
                options.ApiKey = Environment.GetEnvironmentVariable("FOUNDRY_AI_API_KEY") ?? string.Empty;

            if (string.IsNullOrWhiteSpace(options.Model))
                options.Model = Environment.GetEnvironmentVariable("FOUNDRY_AI_MODEL") ?? string.Empty;

            if (string.IsNullOrWhiteSpace(options.Endpoint))
                throw new InvalidOperationException("FOUNDRY_AI_ENDPOINT não configurado.");

            if (string.IsNullOrWhiteSpace(options.ApiKey))
                throw new InvalidOperationException("FOUNDRY_AI_API_KEY não configurado.");

            if (string.IsNullOrWhiteSpace(options.Model))
                throw new InvalidOperationException("FOUNDRY_AI_MODEL não configurado.");
        });

        services.Configure<AzureVisionOptions>(
            configuration.GetSection(AzureVisionOptions.SectionName)
        );

        services.PostConfigure<AzureVisionOptions>(options =>
        {
            if (string.IsNullOrWhiteSpace(options.Endpoint))
                options.Endpoint = Environment.GetEnvironmentVariable("AZURE_VISION_ENDPOINT") ?? string.Empty;

            if (string.IsNullOrWhiteSpace(options.Key))
                options.Key = Environment.GetEnvironmentVariable("AZURE_VISION_KEY") ?? string.Empty;
        });

        services.Configure<AzureSpeechOptions>(
            configuration.GetSection(AzureSpeechOptions.SectionName)
        );

        services.PostConfigure<AzureSpeechOptions>(options =>
        {
            if (string.IsNullOrWhiteSpace(options.Key))
                options.Key = Environment.GetEnvironmentVariable("AZURE_SPEECH_KEY") ?? string.Empty;

            if (string.IsNullOrWhiteSpace(options.Region))
                options.Region = Environment.GetEnvironmentVariable("AZURE_SPEECH_REGION") ?? string.Empty;
        });

        services.AddHttpClient<IFoundryAiService, FoundryAiService>((provider, client) =>
        {
            ConfigureFoundryHttpClient(provider, client);
        })
        .AddStandardResilienceHandler();

        services.AddHttpClient<IFoundryAiStreamingService, FoundryAiStreamingService>((provider, client) =>
        {
            ConfigureFoundryHttpClient(provider, client);
        })
        .AddStandardResilienceHandler();
        
        services.AddHttpClient<IAiTextGenerator, OpenAiTextGenerator>(ConfigureOpenAiClient);
        services.AddHttpClient<IEmbeddingGenerator, OpenAiEmbeddingGenerator>(ConfigureOpenAiClient);

        services.AddSingleton<ITicketRepository, InMemoryTicketRepository>();
        services.AddSingleton<TicketMlModelPaths>();
        services.AddSingleton<IVectorSimilarity, CosineVectorSimilarity>();
        services.AddSingleton<PromptTemplateBuilder>();
        services.AddSingleton<IAiModelSettings, OpenAiModelSettings>();
        services.AddSingleton<ICognitiveTextAnalyzer, AzureLanguageTextAnalyzer>();
        services.AddSingleton<Kernel>(provider =>
        {
            var openAiOptions = provider
                .GetRequiredService<Microsoft.Extensions.Options.IOptions<OpenAiOptions>>()
                .Value;

            var kernelBuilder = Kernel.CreateBuilder();

            kernelBuilder.AddOpenAIChatCompletion(
                modelId: openAiOptions.ChatModel,
                apiKey: openAiOptions.ApiKey
            );

            return kernelBuilder.Build();
        });
        services.AddSingleton<ITextTokenEstimator, SimpleTextTokenEstimator>();
        services.AddSingleton<IAiUsageRepository, InMemoryAiUsageRepository>();
        services.AddSingleton<IOcrService, AzureVisionOcrService>();
        services.AddSingleton<ISpeechToTextService, AzureSpeechToTextService>();

        services.AddScoped<IAiTicketAnalyzer, OpenAiTicketAnalyzer>();
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
        services.AddScoped<GenerateSemanticKernelTicketInsightUseCase>();
        services.AddScoped<GenerateFoundryTicketAnswerUseCase>();
        services.AddScoped<CreateTicketFromImageUseCase>();
        services.AddScoped<CreateTicketFromAudioUseCase>();
        services.AddScoped<AnalyzeMultimodalInputUseCase>();
        services.AddScoped<CreateTicketFromMultimodalInputUseCase>();
        
        return services;
    }

    private static void ConfigureOpenAiClient(IServiceProvider provider, HttpClient client)
    {
        var options = provider.GetRequiredService<IOptions<OpenAiOptions>>().Value;

        client.BaseAddress = new Uri(options.BaseUrl);
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", options.ApiKey);
    }

    private static void ConfigureFoundryHttpClient(IServiceProvider provider, HttpClient client)
    {
        var options = provider
            .GetRequiredService<IOptions<FoundryAiOptions>>()
            .Value;

        var endpoint = options.Endpoint.EndsWith("/")
            ? options.Endpoint
            : options.Endpoint + "/";

        client.BaseAddress = new Uri(endpoint);

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("api-key", options.ApiKey);
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")
        );
    }
}