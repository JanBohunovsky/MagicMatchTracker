using System.Reflection;

namespace MagicMatchTracker.Infrastructure.Services;

public sealed class ApplicationVersionProvider
{
	private const string UnknownVersion = "<unknown>";

	public string Version { get; }
	public string VersionPrefix { get; }
	public string? VersionSuffix { get; }

	public ApplicationVersionProvider()
	{
		var assembly = Assembly.GetExecutingAssembly();
		var informationalVersionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
		var informationalVersion = informationalVersionAttribute?.InformationalVersion;

		Version = informationalVersion ?? UnknownVersion;
		VersionPrefix = Version;

		if (informationalVersion.IsNotEmpty() && informationalVersion.Split('-', StringSplitOptions.RemoveEmptyEntries) is [var prefix, var suffix])
		{
			VersionPrefix = prefix;
			VersionSuffix = suffix;
		}
	}
}