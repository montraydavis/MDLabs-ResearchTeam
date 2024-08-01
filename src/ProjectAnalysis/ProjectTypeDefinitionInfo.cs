using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;


namespace MDLabs.ResearchTeam.ProjectAnalysis
{
	/// <summary>
	/// Represents the information about a project type definition.
	/// </summary>
	public class ProjectTypeDefinitionInfo(Type projectType,
		IEnumerable<ProjectMethodDefinitionInfo> methods,
		ProjectTypeDefinitionInfo? parentType = null)
	{
		public IEnumerable<ProjectMethodDefinitionInfo> Methods => methods;
		[JsonIgnore]
		public Type ProjectType => projectType;
		public ProjectTypeDefinitionInfo? ParentType => parentType;
	}
}

