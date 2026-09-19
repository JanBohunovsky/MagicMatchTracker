using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagicMatchTracker.Data.Configuration;

public sealed class MatchParticipationConfiguration : IEntityTypeConfiguration<MatchParticipation>
{
	public void Configure(EntityTypeBuilder<MatchParticipation> builder)
	{
		builder.HasKey(
			$"{nameof(MatchParticipation.Match)}{nameof(Match.Id)}",
			$"{nameof(MatchParticipation.Player)}{nameof(Player.Id)}");

		builder.HasOne<Match>(mp => mp.Match)
			.WithMany(m => m.Participations)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasOne<Player>(mp => mp.Player)
			.WithMany()
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasOne<Deck>(mp => mp.Deck)
			.WithMany()
			.OnDelete(DeleteBehavior.Restrict);

		builder.OwnsOne<MatchParticipationEndState>(mp => mp.EndState);

		builder.Navigation(mp => mp.Match)
			.AutoInclude();

		builder.Navigation(mp => mp.Player)
			.AutoInclude();

		builder.Navigation(mp => mp.Deck)
			.AutoInclude();
	}
}