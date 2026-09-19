using System.Text;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MagicMatchTracker.Data.Converters;

[UsedImplicitly]
public sealed class ColoursConverter() : ValueConverter<Colours, string>(
	colours => Convert(colours),
	value => ConvertBack(value),
	new ConverterMappingHints(size: 5, unicode: false))
{
	private static string Convert(Colours colours)
	{
		if (colours is Colours.Colourless)
			return "C";

		var sb = new StringBuilder();
		if (colours.HasFlag(Colours.White))
			sb.Append('W');
		if (colours.HasFlag(Colours.Blue))
			sb.Append('U');
		if (colours.HasFlag(Colours.Black))
			sb.Append('B');
		if (colours.HasFlag(Colours.Red))
			sb.Append('R');
		if (colours.HasFlag(Colours.Green))
			sb.Append('G');

		return sb.ToString();
	}

	private static Colours ConvertBack(string colours)
	{
		if (colours.Length is < 1 or > 5)
			throw new ArgumentException($"Colour string must be between 1 and 5 characters: \"{colours}\"", nameof(colours));

		if (colours == "C")
			return Colours.Colourless;

		var result = Colours.Colourless;
		foreach (var c in colours)
		{
			result |= GetFlag(c);
		}

		return result;

		Colours GetFlag(char c) => c switch
		{
			'W' => Colours.White,
			'U' => Colours.Blue,
			'B' => Colours.Black,
			'R' => Colours.Red,
			'G' => Colours.Green,
			_ => throw new ArgumentException($"Invalid colour character: '{c}'", nameof(colours)),
		};
	}
}