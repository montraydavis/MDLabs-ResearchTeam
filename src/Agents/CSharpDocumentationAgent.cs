using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoGen.Core;
using HandlebarsDotNet;
using MDLabs.ResearchTeam.Mediation;
using MediatR;

namespace MDLabs.ResearchTeam.Agents
{
    public class CSharpDocumentationAgent : BaseSemanticAgent, INotificationHandler<CSharpFileProcessingNotification>
    {
        private readonly EmbeddedResourceLoader _embeddedResourceLoader;

        protected override string _instructions { get; }
        
        /// <summary>
        /// Sends a message to the user and system.
        /// </summary>
        /// <param name="userContent">The content to send to the user.</param>
        /// <param name="systemContent">The content to send to the system.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The message sent.</returns>
        /// <remarks>
        /// The message is sent using the SendAsync method.
        /// </remarks>
        private async Task<IMessage> SendMessageAsync(string userContent, string systemContent, CancellationToken cancellationToken)
        {
            return await this.SendAsync(new TextMessage(Role.User, userContent), [new TextMessage(Role.System, systemContent)], cancellationToken);
        }

        /// <summary>
        /// Loads a resource asynchronously.
        /// </summary>
        /// <param name="resourceName">The name of the resource to load.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The loaded resource.</returns>
        /// <remarks>
        /// The resource is loaded using the LoadResourceAsString method.
        /// </remarks>
        private async Task<string> LoadResourceAsync(string resourceName, CancellationToken cancellationToken)
        {
            return await Task.Run(() => _embeddedResourceLoader.LoadResourceAsString(typeof(Worker).Assembly, resourceName), cancellationToken);
        }

        /// <summary>
        /// Generates the content using the Handlebars template.
        /// </summary>
        /// <summary>
        /// Generates the content using the Handlebars template.
        /// </summary>
        /// <param name="handlebars">The Handlebars instance.</param>
        /// <param name="template">The Handlebars template.</param>
        /// <param name="data">The data to pass to the template.</param>
        /// <returns>The generated content.</returns>
        /// <remarks>
        /// The content is generated using the Handlebars template.
        /// </remarks>
        private string CompileHandlebarsTemplate(string template, object data)
        {
            var handlebars = Handlebars.Create();

            var compiledTemplate = handlebars.Compile(template);
            return WebUtility.HtmlDecode(compiledTemplate(data));
        }

        /// <summary>
        /// Initializes a new instance of the CSharpDocumentationAgent class.
        /// </summary>
        /// <param name="embeddedResourceLoader">The embedded resource loader.</param>
        /// <param name="innerAgent">The inner agent.</param>
        /// <returns>The CSharpDocumentationAgent instance.</returns>
        /// <remarks>
        /// The CSharpDocumentationAgent class is a subclass of the BaseSemanticAgent class.
        /// </remarks>
        public CSharpDocumentationAgent(EmbeddedResourceLoader embeddedResourceLoader,
            IAgent innerAgent) : base(innerAgent, nameof(CSharpDocumentationAgent), null)
        {
            this._embeddedResourceLoader = embeddedResourceLoader;
            this._instructions = this._embeddedResourceLoader
                .GetResource(EmbeddedResourceLoader.EmbeddedResourceType.DocGenInstruct);
        }

        /// <summary>
        /// Handles the CSharpFileProcessingNotification notification.
        /// </summary>
        /// <param name="notification">The CSharpFileProcessingNotification notification.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The Task.</returns>
        /// <remarks>
        /// The CSharpFileProcessingNotification notification is handled using the Handle method.
        /// </remarks>
        public async Task Handle(CSharpFileProcessingNotification notification, CancellationToken cancellationToken)
        {
            var csharpClassInfo = this._embeddedResourceLoader.GetResource(EmbeddedResourceLoader.EmbeddedResourceType.CSharpClassInfo);
            var csharpMethodInfoGenInstruct = this._embeddedResourceLoader.GetResource(EmbeddedResourceLoader.EmbeddedResourceType.DocGenMethodInstruct);
            var docgenInstruct = this._embeddedResourceLoader.GetResource(EmbeddedResourceLoader.EmbeddedResourceType.DocGenInstruct);

            var csharpMethodInfoContent = CompileHandlebarsTemplate(csharpClassInfo, notification.Info);
            var csharpMethodInfoGenInstructContent = CompileHandlebarsTemplate(csharpMethodInfoGenInstruct, new
            {
                PreprocessedMethodInfo = csharpMethodInfoContent
            });

            var messageResponse = await SendMessageAsync(csharpMethodInfoGenInstructContent, docgenInstruct, cancellationToken);
        }
    }
}

