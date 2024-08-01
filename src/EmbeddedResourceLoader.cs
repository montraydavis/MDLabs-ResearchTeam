using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;


namespace MDLabs.ResearchTeam;

/// <summary>
/// Provides methods to load embedded resources using glob patterns.
/// </summary>
public class EmbeddedResourceLoader
{

    /// <summary>
    /// The types of resources that can be loaded.
    /// </summary>
    public enum EmbeddedResourceType
    {
        CSharpClassInfo,
        DocGenMethodInstruct,
        DocGenInstruct,
        AnalysisInstruct
    }

    /// <summary>
    /// Initializes a new instance of the EmbeddedResourceLoader class.
    /// </summary>
    /// <param name="_logger">The logger to use for logging.</param>
    /// <remarks>
    /// The constructor initializes the logger and loads all resources into the cache.
    /// </remarks>
    public EmbeddedResourceLoader(ILogger _logger)
    {
        this.logger = _logger;
    }

    private readonly Dictionary<EmbeddedResourceType, string> _resourceCache = new Dictionary<EmbeddedResourceType, string>();
    private ILogger logger;
    public const string CSHARP_CLASS_INFO = "*CSharpClassInfo.hbs";
    public const string DOC_GEN_METHOD_INSTRUCT = "*DocGen-MethodInstruct.md";
    public const string DOC_GEN_INSTRUCT = "*DocGen-Instruct.md";
    public const string ANALYSIS_INSTRUCT = "*Analysis-Instruct.md";


    /// <summary>
    /// Loads all resources into the cache.
    /// </summary>
    /// <remarks>
    /// The LoadAllResources method is used to load all resources into the cache.
    /// </remarks>
    private void LoadAllResources()
    {
        var resourceMap = new Dictionary<EmbeddedResourceType, string>
            {
                { EmbeddedResourceType.CSharpClassInfo, CSHARP_CLASS_INFO },
                { EmbeddedResourceType.DocGenMethodInstruct, DOC_GEN_METHOD_INSTRUCT },
                { EmbeddedResourceType.DocGenInstruct, DOC_GEN_INSTRUCT },
                { EmbeddedResourceType.AnalysisInstruct, ANALYSIS_INSTRUCT }
            };

        foreach (var kvp in resourceMap)
        {
            _resourceCache[kvp.Key] = LoadResourceAsString(typeof(Worker).Assembly, kvp.Value);
        }
    }

    /// <summary>
    /// Retrieves a resource from the cache.
    /// </summary>
    /// <param name="resourceType">The type of the resource to retrieve.</param>
    /// <returns>The resource as a string.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the resource is not found in the cache.</exception>
    /// <remarks>
    /// The GetResource method is used to retrieve a resource from the cache.
    /// </remarks>
    public string GetResource(EmbeddedResourceType resourceType)
    {
        if (_resourceCache.TryGetValue(resourceType, out var resource))
        {
            return resource;
        }
        throw new KeyNotFoundException($"Resource '{resourceType}' not found.");
    }

    /// <summary>
    /// Loads an embedded resource as a string using a glob pattern to match the resource name.
    /// </summary>
    /// <param name="assembly">The assembly containing the embedded resource.</param>
    /// <param name="globPattern">The glob pattern to match the resource name.</param>
    /// <returns>The content of the resource as a string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when assembly or globPattern is null.</exception>
    /// <exception cref="FileNotFoundException">Thrown when no matching resource is found.</exception>
    /// <remarks>
    /// The LoadResourceAsString method is used to load an embedded resource as a string using a glob pattern to match the resource name.
    /// </remarks>
    /// <seealso cref="LoadResourceAsBytes(Assembly, string)"/>
    public string LoadResourceAsString(Assembly assembly, string globPattern)
    {
        if (assembly == null) throw new ArgumentNullException(nameof(assembly));
        if (string.IsNullOrWhiteSpace(globPattern)) throw new ArgumentNullException(nameof(globPattern));

        this.logger.LogDebug("Attempting to load resource as string. Pattern: {GlobPattern}", globPattern);

        var resourceName = FindResourceName(assembly, globPattern);
        if (resourceName == null)
        {
            this.logger.LogWarning("Resource not found. Pattern: {GlobPattern}", globPattern);
            throw new FileNotFoundException($"Resource matching pattern '{globPattern}' not found.");
        }

        this.logger.LogDebug("Resource found. Name: {ResourceName}", resourceName);

        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream ?? throw new InvalidOperationException($"Failed to get stream for resource: {resourceName}"));

        var content = reader.ReadToEnd();
        this.logger.LogDebug("Resource loaded successfully. Length: {ContentLength} characters", content.Length);
        return content;
    }

    /// <summary>
    /// Loads an embedded resource as a byte array using a glob pattern to match the resource name.
    /// </summary>
    /// <param name="assembly">The assembly containing the embedded resource.</param>
    /// <param name="globPattern">The glob pattern to match the resource name.</param>
    /// <returns>The content of the resource as a byte array.</returns>
    /// <exception cref="ArgumentNullException">Thrown when assembly or globPattern is null.</exception>
    /// <exception cref="FileNotFoundException">Thrown when no matching resource is found.</exception>
    public byte[] LoadResourceAsBytes(Assembly assembly, string globPattern)
    {
        if (assembly == null) throw new ArgumentNullException(nameof(assembly));
        if (string.IsNullOrWhiteSpace(globPattern)) throw new ArgumentNullException(nameof(globPattern));

        this.logger.LogDebug("Attempting to load resource as bytes. Pattern: {GlobPattern}", globPattern);

        var resourceName = FindResourceName(assembly, globPattern);
        if (resourceName == null)
        {
            this.logger.LogWarning("Resource not found. Pattern: {GlobPattern}", globPattern);
            throw new FileNotFoundException($"Resource matching pattern '{globPattern}' not found.");
        }

        this.logger.LogDebug("Resource found. Name: {ResourceName}", resourceName);

        using (var stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream == null)
            {
                throw new InvalidOperationException($"Failed to get stream for resource: {resourceName}");
            }

            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            this.logger.LogDebug("Resource loaded successfully. Size: {ByteCount} bytes", bytes.Length);
            return bytes;
        }
    }

    /// <summary>
    /// Finds the full name of a resource matching the given glob pattern.
    /// </summary>
    /// <param name="assembly">The assembly to search for resources.</param>
    /// <param name="globPattern">The glob pattern to match against resource names.</param>
    /// <returns>The full name of the matching resource, or null if no match is found.</returns>
    /// <remarks>
    /// The FindResourceName method is used to find the full name of a resource matching the given glob pattern.
    /// </remarks>
    private string FindResourceName(Assembly assembly, string globPattern)
    {
        this.logger.LogDebug("Converting glob pattern to regex: {GlobPattern}", globPattern);
        var regex = new Regex("^" + Regex.Escape(globPattern).Replace("\\*", ".*").Replace("\\?", ".") + "$",
            RegexOptions.IgnoreCase);

        this.logger.LogDebug("Searching for matching resource...");
        foreach (var resourceName in assembly.GetManifestResourceNames())
        {
            this.logger.LogTrace("Checking resource: {ResourceName}", resourceName);
            if (regex.IsMatch(resourceName))
            {
                this.logger.LogDebug("Match found: {ResourceName}", resourceName);
                return resourceName;
            }
        }

        this.logger.LogDebug("No matching resource found");

        throw new Exception($"Resource matching pattern '{globPattern}' not found.");
    }
}
