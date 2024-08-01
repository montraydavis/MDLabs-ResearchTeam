using System.Reflection;
using AutoGen;
using AutoGen.Core;
using AutoGen.OpenAI;
using MDLabs.ResearchTeam.Agents;
using MDLabs.ResearchTeam.Mediation;
using MDLabs.ResearchTeam.ProjectAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MDLabs.ResearchTeam;


public class Program
{
    private static OpenAIConfig GPTConfig = new OpenAIConfig(OpenAISettings.OpenAIKey, OpenAISettings.OpenAIModel);

    public static void Main(string[] args)
    {
        var agent = new AssistantAgent(
            name: "user",
            // defaultReply: "Hello!",
            llmConfig: new ConversableAgentConfig()
            {
                ConfigList = [GPTConfig]
            },
            humanInputMode: HumanInputMode.NEVER);

        var logger = LoggerFactory.Create(builder => builder.AddConsole())
            .CreateLogger(typeof(Program));

        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddSingleton<IAgent>(c => agent);
        builder.Services.AddSingleton(c => logger);
        builder.Services.AddSingleton<IAgent>(c => agent);
        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(MDLabsAssemblyEntyPoint).Assembly));
            builder.Services.AddSingleton<EmbeddedResourceLoader>();
        builder.Services.AddSingleton<CSharpProjectAnalyzer>();
        builder.Services.AddSingleton<CSharpAnalysisAgent>();
        builder.Services.AddSingleton<CSharpDocumentationAgent>();
        builder.Services.AddSingleton<CSharpProcessingMediatR>();
        builder.Services.AddHostedService<Worker>();
        var host = builder.Build();
        host.Run();
    }
}
