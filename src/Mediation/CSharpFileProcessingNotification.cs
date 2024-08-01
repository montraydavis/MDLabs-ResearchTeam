using MDLabs.ResearchTeam.ProjectAnalysis;
using MediatR;

namespace MDLabs.ResearchTeam.Mediation;

/// <summary>
/// Represents a notification for C# file processing.
/// </summary>
public class CSharpFileProcessingNotification(ProjectClassDefinitionInfo info) : INotification
{
    public ProjectClassDefinitionInfo Info => info;
}
