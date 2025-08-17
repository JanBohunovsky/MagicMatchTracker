using MagicMatchTracker.Data.Converters;
using Microsoft.EntityFrameworkCore;

namespace MagicMatchTracker.Data;

public sealed class Database(DbContextOptions<Database> options) : DbContext(options)
{
	public DbSet<Player> Players => Set<Player>();
	public DbSet<Deck> Decks => Set<Deck>();
	public DbSet<Match> Matches => Set<Match>();
	public DbSet<MatchParticipation> MatchParticipations => Set<MatchParticipation>();
	public DbSet<MatchEvent> MatchEvents => Set<MatchEvent>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(Database).Assembly);
	}

	protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
	{
		configurationBuilder.Properties<DateTimeOffset>()
			.HaveConversion<DateTimeOffsetTimezoneConverter>();

		configurationBuilder.Properties<Colours>()
			.HaveConversion<ColoursConverter>();
	}
}