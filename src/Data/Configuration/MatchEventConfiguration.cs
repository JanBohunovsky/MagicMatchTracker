using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagicMatchTracker.Data.Configuration;

public sealed class MatchEventConfiguration : IEntityTypeConfiguration<MatchEvent>
{
	public void Configure(EntityTypeBuilder<MatchEvent> builder)
	{
		builder.HasKey(me => me.Id);

		builder.OwnsOne(me => me.Data, d => d.ToJson());

		builder.HasOne<MatchParticipation>(me => me.Participation)
			.WithMany(m => m.Events)
			.OnDelete(DeleteBehavior.Restrict);
	}
}