using Microsoft.SemanticKernel;
using SupportFlowAI.Application.Interfaces;
using SupportFlowAI.Infrastructure.SemanticKernel.Plugins;

namespace SupportFlowAI.Infrastructure.SemanticKernel;

public sealed class SupportFlowKernelFactory : ISupportFlowKernelFactory
{
    private readonly TicketKernelPlugin _ticketPlugin;
    private readonly AiTicketKernelPlugin _aiTicketPlugin;
    private readonly WorkflowMemoryPlugin _memoryPlugin;

    public SupportFlowKernelFactory(
        TicketKernelPlugin ticketPlugin,
        AiTicketKernelPlugin aiTicketPlugin,
        WorkflowMemoryPlugin memoryPlugin)
    {
        _ticketPlugin = ticketPlugin;
        _aiTicketPlugin = aiTicketPlugin;
        _memoryPlugin = memoryPlugin;
    }

    public Kernel CreateKernel()
    {
        var builder = Kernel.CreateBuilder();

        var kernel = builder.Build();

        kernel.Plugins.AddFromObject(_ticketPlugin, "Tickets");
        kernel.Plugins.AddFromObject(_aiTicketPlugin, "AI");
        kernel.Plugins.AddFromObject(_memoryPlugin, "Memory");

        return kernel;
    }
}