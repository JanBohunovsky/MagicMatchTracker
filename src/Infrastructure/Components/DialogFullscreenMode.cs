namespace MagicMatchTracker.Infrastructure.Components;

public enum DialogFullscreenMode
{
	/// <summary>
	/// Dialog will never be fullscreen.
	/// </summary>
	Never,

	/// <summary>
	/// Dialog will be fullscreen on mobile devices.
	/// </summary>
	Mobile,

	/// <summary>
	/// Dialog will always be fullscreen.
	/// </summary>
	Always,
}

public static class DialogFullscreenModeExtensions
{
	public static string? GetCssClass(this DialogFullscreenMode mode) => mode switch
	{
		DialogFullscreenMode.Never => null,
		DialogFullscreenMode.Mobile => "modal-fullscreen-sm-down",
		DialogFullscreenMode.Always => "modal-fullscreen",
		_ => throw new ArgumentException($"Invalid dialog fullscreen mode: {mode}", nameof(mode)),
	};
}