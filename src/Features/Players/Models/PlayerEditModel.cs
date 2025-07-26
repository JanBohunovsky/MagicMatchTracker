using FluentValidation;

namespace MagicMatchTracker.Features.Players.Models;

public sealed class PlayerEditModel
{
	private readonly Player _model;

	public string Name { get; set; }
	public string AvatarUri { get; set; }

	public PlayerEditModel(Player player)
	{
		_model = player;
		Name = player.Name;
		AvatarUri = player.AvatarUri ?? string.Empty;
	}

	public Player ApplyChanges()
	{
		_model.Name = Name.Trim();
		_model.AvatarUri = AvatarUri.TrimToNull();

		return _model;
	}
}

[UsedImplicitly]
public sealed class PlayerEditModelValidator : AbstractValidator<PlayerEditModel>
{
	public PlayerEditModelValidator()
	{
		RuleFor(m => m.Name)
			.NotEmpty()
			.WithMessage("Enter the name");

		RuleFor(m => m.AvatarUri)
			.Must(u => u.StartsWith("https://") || u.StartsWith("http://"))
			.When(m => !string.IsNullOrEmpty(m.AvatarUri))
			.WithMessage("URL starts with http:// or https://");
	}
}