using Microsoft.ML;
using SupportFlowAI.Application.Interfaces;
using SupportFlowAI.Application.ML;
using SupportFlowAI.Infrastructure.ML.Data;

namespace SupportFlowAI.Infrastructure.ML;

public sealed class MlNetTicketModelTrainer : ITicketMlModelTrainer
{
    private readonly TicketMlModelPaths _paths;

    public MlNetTicketModelTrainer(TicketMlModelPaths paths)
    {
        _paths = paths;
    }

    public Task TrainAsync(CancellationToken cancellationToken)
    {
        var mlContext = new MLContext(seed: 42);

        Directory.CreateDirectory(Path.GetDirectoryName(_paths.CategoryModelPath)!);
        Directory.CreateDirectory(Path.GetDirectoryName(_paths.EffortModelPath)!);

        TrainCategoryModel(mlContext);
        TrainEffortModel(mlContext);

        return Task.CompletedTask;
    }

    private void TrainCategoryModel(MLContext mlContext)
    {
        var data = TicketTrainingDataFactory.CreateCategoryData();
        var dataView = mlContext.Data.LoadFromEnumerable(data);

        var pipeline = mlContext.Transforms.Conversion.MapValueToKey(
                outputColumnName: "Label",
                inputColumnName: nameof(TicketCategoryTrainingData.Category))
            .Append(mlContext.Transforms.Text.FeaturizeText(
                outputColumnName: "Features",
                inputColumnName: nameof(TicketCategoryTrainingData.Text)))
            .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy(
                labelColumnName: "Label",
                featureColumnName: "Features"))
            .Append(mlContext.Transforms.Conversion.MapKeyToValue(
                outputColumnName: "PredictedLabel",
                inputColumnName: "PredictedLabel"));

        var model = pipeline.Fit(dataView);

        mlContext.Model.Save(model, dataView.Schema, _paths.CategoryModelPath);
    }

    private void TrainEffortModel(MLContext mlContext)
    {
        var data = TicketTrainingDataFactory.CreateEffortData();
        var dataView = mlContext.Data.LoadFromEnumerable(data);

        var pipeline = mlContext.Transforms.CopyColumns(
                outputColumnName: "Label",
                inputColumnName: nameof(TicketEffortTrainingData.EffortHours))
            .Append(mlContext.Transforms.Text.FeaturizeText(
                outputColumnName: "TextFeatures",
                inputColumnName: nameof(TicketEffortTrainingData.Text)))
            .Append(mlContext.Transforms.Categorical.OneHotEncoding(
                outputColumnName: "CategoryFeatures",
                inputColumnName: nameof(TicketEffortTrainingData.Category)))
            .Append(mlContext.Transforms.Categorical.OneHotEncoding(
                outputColumnName: "PriorityFeatures",
                inputColumnName: nameof(TicketEffortTrainingData.Priority)))
            .Append(mlContext.Transforms.Concatenate(
                "Features",
                "TextFeatures",
                "CategoryFeatures",
                "PriorityFeatures"))
            .Append(mlContext.Regression.Trainers.Sdca(
                labelColumnName: "Label",
                featureColumnName: "Features"));

        var model = pipeline.Fit(dataView);

        mlContext.Model.Save(model, dataView.Schema, _paths.EffortModelPath);
    }
}