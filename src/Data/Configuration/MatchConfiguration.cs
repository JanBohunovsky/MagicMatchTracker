using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagicMatchTracker.Data.Configuration;

public sealed class MatchConfiguration : IEntityTypeConfiguration<Match>
{
	public void Configure(EntityTypeBuilder<Match> builder)
	{
		builder.HasKey(m => m.Id);
	}
}