using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.TypeSystem;
using Microsoft.Extensions.Logging;


namespace MDLabs.ResearchTeam.ProjectAnalysis
{
	/// <summary>
	/// A class for analyzing C# projects.
	/// </summary>
	public class CSharpProjectAnalyzer(ILogger logger)
	{
        protected Assembly ProjectAssembly => MDLabsAssemblyEntyPoint.EntryAssembly;

        /// <summary>
        /// Retrieves the classes from the project assembly.
        /// </summary>
        /// <param name="includeMethods">Flag to include methods along with classes.</param>
        /// <returns>An IEnumerable of Type representing the project classes.</returns>
		/// <remarks>
		/// The GetProjectClasses method is used to retrieve the project classes from the project assembly.
		/// </remarks>
        public IEnumerable<Type> GetProjectClasses(bool includeMethods = false)
		{
			var projectClasses = this.ProjectAssembly
				.GetTypes()
				.Where(t =>
				{
					var assemblyNs = this.ProjectAssembly.GetName().Name;
					var nsParts = assemblyNs?.Split('.') ?? [];

					if (string.IsNullOrWhiteSpace(assemblyNs)
						|| nsParts.Length == 0)
					{
						return false;
					}

					var isClass = t.IsClass
						&& t.FullName?.StartsWith(nsParts[0]) == true;

					return isClass;
				})
				.ToArray();

			return projectClasses;
		}

		/// <summary>
		/// Asynchronously retrieves project class definition information.
		/// </summary>
		/// <returns>An asynchronous enumerable of ProjectClassDefinitionInfo.</returns>
		/// <remarks>
		/// The GetProjectTypeDefinitionsAsync method is used to retrieve the project class definition information.
		/// </remarks>
		public async IAsyncEnumerable<ProjectClassDefinitionInfo> GetProjectTypeDefinitionsAsync()
		{
			var decompiler = new CSharpDecompiler(ProjectAssembly.Location, new DecompilerSettings());
			var typeSystem = decompiler.TypeSystem;

			var allTypeDefinitions = typeSystem.GetAllTypeDefinitions();
			var projectNamespace = ProjectAssembly.GetName().Name;

			foreach (var typeDefinition in allTypeDefinitions)
			{
				if (typeDefinition.Kind != TypeKind.Class || !typeDefinition.FullName.StartsWith(projectNamespace))
				{
					continue;
				}

				var classMethods = new List<ProjectMethodDefinitionInfo>();

				await foreach (var methodInfo in GetMethodInfosAsync(typeDefinition))
				{
					classMethods.Add(methodInfo);
				}

				yield return new ProjectClassDefinitionInfo(
					typeDefinition,
					ProjectAssembly,
					typeDefinition.Methods,
					classMethods,
					logger);
			}
		}

		/// <summary>
		/// Retrieves the method information for a given type definition.
		/// </summary>
		/// <param name="typeDefinition">The type definition to retrieve the method information for.</param>
		/// <returns>An asynchronous enumerable of ProjectMethodDefinitionInfo.</returns>
		/// <remarks>
		/// The GetMethodInfosAsync method is used to retrieve the method information for a given type definition.
		/// </remarks>
		private async IAsyncEnumerable<ProjectMethodDefinitionInfo> GetMethodInfosAsync(ITypeDefinition typeDefinition)
		{
			foreach (var projectMethod in typeDefinition.Methods)
			{
				var methodSourceCode = await Task.Run(() => projectMethod.GetSourceCode(ProjectAssembly, logger));

				yield return new ProjectMethodDefinitionInfo(
					ProjectAssembly,
					projectMethod,
					methodSourceCode,
					logger);
			}
		}
	}
}