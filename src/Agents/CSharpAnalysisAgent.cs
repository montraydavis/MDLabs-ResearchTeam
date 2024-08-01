using System.Collections.Generic;
using AutoGen.Core;

namespace MDLabs.ResearchTeam.Agents
{
    public class CSharpAnalysisAgent : BaseSemanticAgent
    {
        protected override string _instructions { get; }

        /// <summary>
        /// Initializes a new instance of the CSharpAnalysisAgent class.
        /// </summary>
        /// <param name="embeddedResourceLoader">The embedded resource loader to use for loading resources.</param>
        /// <param name="innerAgent">The inner agent to use for the middleware agent.</param>
        /// <remarks>
        /// The CSharpAnalysisAgent constructor is used to initialize a new instance of the CSharpAnalysisAgent class.
        /// </remarks>
        public CSharpAnalysisAgent(EmbeddedResourceLoader embeddedResourceLoader,
            IAgent innerAgent) : base(innerAgent, nameof(CSharpAnalysisAgent), null)
        {
            _instructions = embeddedResourceLoader
                .LoadResourceAsString(typeof(Worker).Assembly,
            "*Analysis-Instruct.md");
        }
    }
}

