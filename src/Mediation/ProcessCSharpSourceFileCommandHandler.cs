using System.Threading;
using System.Threading.Tasks;
using MDLabs.ResearchTeam.ProjectAnalysis;
using MediatR;

namespace MDLabs.ResearchTeam.Mediation;

/// <summary>
/// Handles the process C# source file command.
/// </summary>
public class ProcessCSharpSourceFileCommandHandler : IRequestHandler<ProcessCSharpSourceFileCommand, ProjectClassDefinitionInfo>
{
    public Task<ProjectClassDefinitionInfo> Handle(ProcessCSharpSourceFileCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(request.Info);
    }
}
