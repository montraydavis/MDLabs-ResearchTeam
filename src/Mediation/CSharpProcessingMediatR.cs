using System;
using System.Threading.Tasks;
using MDLabs.ResearchTeam.ProjectAnalysis;
using MediatR;

namespace MDLabs.ResearchTeam.Mediation;

/// <summary>
/// Provides methods to safely communicate C# processing to multi-threaded agent networks.
/// </summary>
public class CSharpProcessingMediatR
{
    private readonly IMediator _mediator;

    public CSharpProcessingMediatR(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Dispatches an info event.
    /// </summary>
    /// <param name="typeDef">The type definition to dispatch the info event for.</param>
    /// <returns>The result of the info command.</returns>
    /// <remarks>
    /// The DispatchInfoEvent method is used to dispatch an info event.
    /// </remarks>
    public async Task DispatchInfoEvent(ProjectClassDefinitionInfo typeDef)
    {
        // Send command
        var command = new ProcessCSharpSourceFileCommand(typeDef);
        var result = await _mediator.Send(command);

        Console.WriteLine($"Command result: Class - {result.ClassName}");

        // Send notification
        await _mediator.Publish(new CSharpFileProcessingNotification(typeDef));
    }
}
