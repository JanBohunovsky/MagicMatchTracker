using MagicMatchTracker.Infrastructure.Startup;

try
{
	var builder = WebApplication.CreateBuilder(args);
	builder.AddServices();

	var app = builder.Build();
	await app.ConfigureAsync();

	app.Run();
}
catch (Exception e) when (e is not HostAbortedException)
{
	// TODO: Log exception
}