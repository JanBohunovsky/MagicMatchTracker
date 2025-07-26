namespace MagicMatchTracker.Data.Models;

[Flags]
public enum Colours
{
	Colourless = 0,
	White = 1 << 0,
	Blue = 1 << 1,
	Black = 1 << 2,
	Red = 1 << 3,
	Green = 1 << 4,
}

public static class ColoursExtensions
{
	public static string ToColourIdentityName(this Colours colours) => colours switch
	{
		Colours.Colourless => "Colourless",

		// Mono
		Colours.White => "Mono White",
		Colours.Blue => "Mono Blue",
		Colours.Black => "Mono Black",
		Colours.Red => "Mono Red",
		Colours.Green => "Mono Green",

		// 2 Colour
		Colours.White | Colours.Blue => "Azorius",
		Colours.Blue | Colours.Black => "Dimir",
		Colours.Black | Colours.Red => "Rakdos",
		Colours.Red | Colours.Green => "Gruul",
		Colours.Green | Colours.White => "Selesnya",
		Colours.White | Colours.Black => "Orzhov",
		Colours.Blue | Colours.Red => "Izzet",
		Colours.Black | Colours.Green => "Golgari",
		Colours.Red | Colours.White => "Boros",
		Colours.Green | Colours.Blue => "Simic",

		// 3 Colour
		Colours.White | Colours.Blue | Colours.Black => "Esper",
		Colours.Blue | Colours.Black | Colours.Red => "Grixis",
		Colours.Black | Colours.Red | Colours.Green => "Jund",
		Colours.Red | Colours.Green | Colours.White => "Naya",
		Colours.Green | Colours.White | Colours.Blue => "Bant",
		Colours.White | Colours.Black | Colours.Green => "Abzan",
		Colours.Blue | Colours.Red | Colours.White => "Jeskai",
		Colours.Black | Colours.Green | Colours.Blue => "Sultai",
		Colours.Red | Colours.White | Colours.Black => "Mardu",
		Colours.Green | Colours.Blue | Colours.Red => "Temur",

		// 4+ Colours
		Colours.White | Colours.Blue | Colours.Black | Colours.Red => "Artifice",
		Colours.Blue | Colours.Black | Colours.Red | Colours.Green => "Chaos",
		Colours.Black | Colours.Red | Colours.Green | Colours.White => "Aggression",
		Colours.Red | Colours.Green | Colours.White | Colours.Blue => "Altruism",
		Colours.Green | Colours.White | Colours.Blue | Colours.Black => "Growth",
		Colours.White | Colours.Blue | Colours.Black | Colours.Red | Colours.Green => "Five Colour",

		_ => throw new ArgumentException($"Invalid colour combination: {colours}", nameof(colours)),
	};
}