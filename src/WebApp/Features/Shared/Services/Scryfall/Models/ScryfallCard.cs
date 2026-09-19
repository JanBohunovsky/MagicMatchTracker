using System.Diagnostics.CodeAnalysis;

namespace MagicMatchTracker.Features.Shared.Services.Scryfall.Models;

public record ScryfallCard(string Name, string ImageStatus, ScryfallImageUris? ImageUris)
{
	[MemberNotNullWhen(true, nameof(ImageUris))]
	public bool HasImage => ImageStatus != "missing"
		&& ImageStatus != "placeholder"
		&& ImageUris is not null;
}