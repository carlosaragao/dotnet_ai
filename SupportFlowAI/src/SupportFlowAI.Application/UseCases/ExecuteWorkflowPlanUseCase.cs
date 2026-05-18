using System.Text.Json;
using Microsoft.SemanticKernel;
using SupportFlowAI.Application.DTOs.Workflows;
using SupportFlowAI.Application.Interfaces;
using SupportFlowAI.Application.Workflows;

namespace SupportFlowAI.Application.UseCases;

public sealed class ExecuteWorkflowPlanUseCase
{
    private readonly ISupportFlowKernelFactory _kernelFactory;

    public ExecuteWorkflowPlanUseCase(ISupportFlowKernelFactory kernelFactory)
    {
        _kernelFactory = kernelFactory;
    }

    public async Task<WorkflowExecutionResponse> HandleAsync(
        WorkflowPlan plan,
        CancellationToken cancellationToken)
    {
        var kernel = _kernelFactory.CreateKernel();

        var results = new List<WorkflowStepResult>();
        var lastResult = string.Empty;

        foreach (var step in plan.Steps.OrderBy(step => step.Order))
        {
            var arguments = new KernelArguments();

            foreach (var argument in step.Arguments)
            {
                arguments[argument.Key] = ConvertJsonArgument(argument.Value, lastResult);
            }

            var result = await kernel.InvokeAsync(
                pluginName: step.Plugin,
                functionName: step.Function,
                arguments: arguments,
                cancellationToken: cancellationToken
            );

            lastResult = result.ToString();

            results.Add(new WorkflowStepResult(
                step.Order,
                step.Plugin,
                step.Function,
                lastResult
            ));
        }

        return new WorkflowExecutionResponse(
            plan.Goal,
            results,
            lastResult
        );
    }

    private static object? ConvertJsonArgument(
        JsonElement value,
        string lastResult)
    {
        if (value.ValueKind == System.Text.Json.JsonValueKind.String)
        {
            var stringValue = value.GetString();

            return stringValue == "$lastResult"
                ? lastResult
                : stringValue;
        }

        if (value.ValueKind == System.Text.Json.JsonValueKind.Number)
        {
            if (value.TryGetInt32(out var intValue))
                return intValue;

            if (value.TryGetDouble(out var doubleValue))
                return doubleValue;
        }

        if (value.ValueKind == System.Text.Json.JsonValueKind.True)
            return true;

        if (value.ValueKind == System.Text.Json.JsonValueKind.False)
            return false;

        if (value.ValueKind == System.Text.Json.JsonValueKind.Null)
            return null;

        return value.ToString();
    }
}