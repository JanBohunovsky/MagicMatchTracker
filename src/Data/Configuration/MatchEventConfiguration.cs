using MagicMatchTracker.Data.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagicMatchTracker.Data.Configuration;

public sealed class MatchEventConfiguration : IEntityTypeConfiguration<MatchEvent>
{
	public void Configure(EntityTypeBuilder<MatchEvent> builder)
	{
		builder.HasKey(me => me.Id);

		// EF Core doesn't support JSON inheritance yet, so we have to use legacy POCO mapping.
		// We also have to use custom converter, because the JsonSerializer that's used by EF Core/Npgsql
		// doesn't respect the `JsonPolymorphic` attribute.
		builder.Property(me => me.Data)
			.HasConversion<MatchEventDataJsonConverter>()
			.HasColumnType("jsonb");

		builder.HasOne<MatchParticipation>(me => me.Participation)
			.WithMany(m => m.Events)
			.OnDelete(DeleteBehavior.Restrict);
	}
}