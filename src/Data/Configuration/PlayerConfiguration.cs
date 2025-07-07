using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagicMatchTracker.Data.Configuration;

public sealed class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
	/// <inheritdoc />
	public void Configure(EntityTypeBuilder<Player> builder)
	{
		builder.HasKey(p => p.Id);
	}
}