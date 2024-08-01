using System;
using System.Net;
using System.Reflection;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.TypeSystem;
using Microsoft.Extensions.Logging;


namespace MDLabs.ResearchTeam.ProjectAnalysis
{
	/// <summary>
	/// Provides extension methods to retrieve source code from project types and methods.
	/// </summary>
	public static class SourceCodeDecompiler
	{
		/// <summary>
		/// Retrieves the source code for a given type definition.
		/// </summary>
		/// <param name="classType">The type definition to retrieve the source code for.</param>
		/// <param name="projectAssembly">The project assembly containing the type definition.</param>
		/// <param name="logger">The logger to use for logging errors.</param>
		/// <returns>The source code for the given type definition.</returns>
		/// <remarks>
		/// The GetSourceCode method is used to retrieve the source code for a given type definition.
		/// </remarks>
		public static string? GetSourceCode(this ITypeDefinition classType, Assembly projectAssembly, ILogger logger)
        {
            // Path to the assembly containing the class
            string assemblyPath = projectAssembly.Location;

            // Initialize the decompiler with the assembly path
            var decompiler = new CSharpDecompiler(assemblyPath, new DecompilerSettings());

            try
            {
                // Decompile the class and get the source code as a string
                var sourceCode = decompiler.DecompileTypeAsString(classType.FullTypeName);

                return WebUtility.HtmlDecode(sourceCode);
            }
            catch (Exception ex)
            {
                logger.LogError($"Could not decompile class. {ex.Message}", ex);

                return null;
            }
        }

		/// <summary>
		/// Retrieves the source code for a given method.
		/// </summary>
		/// <param name="methodInfo">The method to retrieve the source code for.</param>
		/// <param name="projectAssembly">The project assembly containing the method.</param>
		/// <param name="logger">The logger to use for logging errors.</param>
		/// <returns>The source code for the given method.</returns>
		/// <remarks>
		/// The GetSourceCode method is used to retrieve the source code for a given method.
		/// </remarks>
		public static string? GetSourceCode(this IMethod methodInfo, Assembly projectAssembly, ILogger logger)
		{
			// Path to the assembly containing the method
			string assemblyPath = projectAssembly.Location;

			// Initialize the decompiler with the assembly path
			var decompiler = new CSharpDecompiler(assemblyPath, new DecompilerSettings());

			// Find the type definition in the decompiler's type system
			var typeDefinition = decompiler
				.TypeSystem
				.MainModule
				.Compilation
				.FindType(methodInfo.GetType())
				.GetDefinition();

			if (typeDefinition == null)
			{
				throw new Exception("Could not resolve main module type definition. ");
			}

			// Decompile the method and get the source code as a string
			var handle = methodInfo.MetadataToken;

			try
			{
				if (handle.IsNil)
				{
					logger.LogDebug($"Method handle is nil `{methodInfo.Name}`.");

					return null;
				}

				var sourceCode
					= WebUtility.HtmlDecode(decompiler.DecompileAsString(handle));

				return sourceCode;
			}
			catch (Exception ex)
			{
				logger.LogError($"Could not decompile method. {ex.Message}", ex);

				return null;
			}

		}
	}
}

