using Microsoft.SemanticKernel;
using SupportFlowAI.Application.DTOs.SemanticKernel;
using SupportFlowAI.Application.Interfaces;

namespace SupportFlowAI.Application.UseCases;

public sealed class GenerateTicketAnswerWithKernelUseCase
{
    private readonly ISupportFlowKernelFactory _kernelFactory;

    public GenerateTicketAnswerWithKernelUseCase(ISupportFlowKernelFactory kernelFactory)
    {
        _kernelFactory = kernelFactory;
    }

    public async Task<KernelPluginExecutionResponse> HandleAsync(
        Guid ticketId,
        CancellationToken cancellationToken)
    {
        var kernel = _kernelFactory.CreateKernel();

        var result = await kernel.InvokeAsync(
            pluginName: "AI",
            functionName: "generate_ticket_answer",
            arguments: new KernelArguments
            {
                ["ticketId"] = ticketId.ToString()
            },
            cancellationToken: cancellationToken
        );

        return new KernelPluginExecutionResponse(
            "AI",
            "generate_ticket_answer",
            result.ToString()
        );
    }
}