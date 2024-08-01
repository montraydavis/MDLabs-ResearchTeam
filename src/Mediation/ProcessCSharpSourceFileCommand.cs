using MDLabs.ResearchTeam.ProjectAnalysis;
using MediatR;

namespace MDLabs.ResearchTeam.Mediation;

/// <summary>
/// Represents a command to process a C# source file.
/// </summary>
public class ProcessCSharpSourceFileCommand(ProjectClassDefinitionInfo info) : IRequest<ProjectClassDefinitionInfo>
{
    public ProjectClassDefinitionInfo Info => info;

}
