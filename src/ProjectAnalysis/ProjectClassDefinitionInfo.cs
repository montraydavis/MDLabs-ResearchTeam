using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.Json.Serialization;
using ICSharpCode.Decompiler.TypeSystem;
using Microsoft.Extensions.Logging;


namespace MDLabs.ResearchTeam.ProjectAnalysis
{
	
    public class ProjectClassDefinitionInfo(
		ITypeDefinition classType,
		Assembly projectAssembly,
		IEnumerable<IMethod> methodInfos,
		IEnumerable<ProjectMethodDefinitionInfo> methodDefinitionInfos,
		ILogger _logger,
		ProjectTypeDefinitionInfo? parentType = null)
	{
		[JsonIgnore]
		public IType ProjectType => classType;
		public ProjectTypeDefinitionInfo? ParentType => parentType;

		[JsonIgnore]
		public IMethod[] MethodInfos => methodInfos.ToArray();
		public ProjectMethodDefinitionInfo[] MethodDefinitionInfos => methodDefinitionInfos.ToArray();

		public string SourceCode =>
			this.GetSourceCode();

		public string Name =>
					classType.Name;
		public string ClassName =>
			classType.FullName;

		/// <summary>
		/// Gets the source code of the class.
		/// </summary>
		/// <returns>The source code of the class.</returns>
		public string GetSourceCode()
		{
			var errCannotDecompileSourceCode = $"Could not decompile source code for class `{ClassName}`";
			var sourceCode = WebUtility.HtmlDecode(classType.GetSourceCode(projectAssembly, _logger));

			return sourceCode ?? throw new Exception(errCannotDecompileSourceCode);
		}
	}
}

