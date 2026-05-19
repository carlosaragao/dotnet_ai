using Microsoft.SemanticKernel;

namespace SupportFlowAI.Application.Interfaces;

public interface ISupportFlowKernelFactory
{
    Kernel CreateKernel();
}