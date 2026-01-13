using System.Reflection;

namespace MagicMatchTracker.Infrastructure.Services;

public sealed class ApplicationVersionProvider
{
	public string Version { get; }

	public ApplicationVersionProvider()
	{
		var assembly = Assembly.GetExecutingAssembly();
		var informationalVersionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
		Version = informationalVersionAttribute?.InformationalVersion ?? "<unknown>";
	}
}