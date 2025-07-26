using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagicMatchTracker.Data.Configuration;

public sealed class DeckConfiguration : IEntityTypeConfiguration<Deck>
{
	public void Configure(EntityTypeBuilder<Deck> builder)
	{
		builder.HasKey(d => d.Id);

		builder.Navigation(d => d.Owner)
			.AutoInclude();
	}
}