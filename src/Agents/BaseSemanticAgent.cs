using System.Collections.Generic;
using System.Threading.Tasks;
using AutoGen.Core;
using AutoGen.OpenAI;

namespace MDLabs.ResearchTeam.Agents
{
    public abstract class BaseSemanticAgent : MiddlewareAgent
    {
        protected abstract string _instructions { get; }


        /// <summary>
        /// Initializes a new instance of the BaseSemanticAgent class.
        /// </summary>
        /// <param name="innerAgent">The inner agent to use for the middleware agent.</param>
        /// <param name="name">The name of the agent.</param>
        /// <param name="middlewares">The middlewares to use for the middleware agent.</param>
        /// <remarks>
        /// The BaseSemanticAgent constructor is used to initialize a new instance of the BaseSemanticAgent class.
        /// </remarks>
        public BaseSemanticAgent(IAgent innerAgent, string? name = null, IEnumerable<IMiddleware>? middlewares = null) : base(innerAgent, name, middlewares)
        {

        }

        /// <summary>
        /// Prompts the agent with a given prompt and receiver.
        /// </summary>
        /// <param name="prompt">The prompt to send to the agent.</param>
        /// <param name="receiver">The receiver of the prompt.</param>
        /// <returns>The response from the agent.</returns>
        /// <remarks>
        /// The PromptAsync method is used to send a prompt to the agent and receive a response.
        /// </remarks>
        public async Task<IEnumerable<IMessage>> PromptAsync(string prompt,
            IAgent receiver)
        {
            var li = new List<IMessage>()
            {
                new TextMessage(Role.System, _instructions),
                // new TextMessage(Role.User, prompt)
            };


            var rex = await this.InitiateChatAsync(receiver,
                _instructions,
                1);
                
            var response = await this.SendAsync(
                receiver: receiver,
                chatHistory: rex,
                message: prompt,
                maxRound: 3);

            return response;
        }
    }
}

