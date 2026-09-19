using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagicMatchTracker.Data.Configuration;

public sealed class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
	public void Configure(EntityTypeBuilder<Player> builder)
	{
		builder.HasKey(p => p.Id);

		builder.HasMany(p => p.Decks)
			.WithOne(d => d.Owner);
	}
}