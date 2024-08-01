using System;
using System.Threading;
using System.Threading.Tasks;
using MDLabs.ResearchTeam.Agents;
using MDLabs.ResearchTeam.Mediation;
using MDLabs.ResearchTeam.ProjectAnalysis;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MDLabs.ResearchTeam;

public class Worker : BackgroundService
{
    protected CSharpProjectAnalyzer ProjectAnalyzer { get; }
    public CSharpProcessingMediatR ProcessingMediatR { get; }

    private readonly CSharpAnalysisAgent _analysisAgent;
    private readonly CSharpDocumentationAgent _documentationAgent;
    private readonly ILogger<Worker> _logger;

    public Worker(
        CSharpProjectAnalyzer projectAnalyzer,
        CSharpAnalysisAgent analysisAgent,
        CSharpDocumentationAgent documentationAgent,
        CSharpProcessingMediatR mediatR,
        ILogger<Worker> logger)
    {
        this.ProjectAnalyzer = projectAnalyzer;
        this.ProcessingMediatR = mediatR;
        
        _analysisAgent = analysisAgent;
        _documentationAgent = documentationAgent;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
        var typeDefs = this.ProjectAnalyzer
            .GetProjectTypeDefinitionsAsync();

        await foreach (var typeDef in typeDefs)
        {
            try{
                await this.ProcessingMediatR.DispatchInfoEvent(typeDef);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error processing type definition.");
            }
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}

