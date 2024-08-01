using System.Reflection;
using System.Text.Json.Serialization;
using ICSharpCode.Decompiler.TypeSystem;
using Microsoft.Extensions.Logging;
using System.Net;


namespace MDLabs.ResearchTeam.ProjectAnalysis
{
	/// <summary>
	/// A class representing the definition of a method.
	/// </summary>
	public class ProjectMethodDefinitionInfo(
		Assembly projectAssembly,
		IMethod projectMethod,
		string? sourceCode,
		ILogger _logger,
		IType? parentType = null)
	{
		[JsonIgnore]
		public IMethod ProjectMethod => projectMethod;
		[JsonIgnore]
		public IType? ParentType { get; } = parentType;

		public string Name => this.ProjectMethod.Name;
		public string MethodName =>
			this.ProjectMethod.FullName;

		public string? SourceCode =>
			sourceCode;

	}
}

