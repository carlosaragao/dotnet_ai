using Microsoft.SemanticKernel;
using SupportFlowAI.Application.DTOs.SemanticKernel;
using SupportFlowAI.Application.Interfaces;

namespace SupportFlowAI.Application.UseCases;

public sealed class InspectTicketWithKernelUseCase
{
    private readonly ISupportFlowKernelFactory _kernelFactory;

    public InspectTicketWithKernelUseCase(ISupportFlowKernelFactory kernelFactory)
    {
        _kernelFactory = kernelFactory;
    }

    public async Task<KernelPluginExecutionResponse> HandleAsync(
        Guid ticketId,
        CancellationToken cancellationToken)
    {
        var kernel = _kernelFactory.CreateKernel();

        var result = await kernel.InvokeAsync(
            pluginName: "Tickets",
            functionName: "get_ticket",
            arguments: new KernelArguments
            {
                ["ticketId"] = ticketId.ToString()
            },
            cancellationToken: cancellationToken
        );

        return new KernelPluginExecutionResponse(
            "Tickets",
            "get_ticket",
            result.ToString()
        );
    }
}