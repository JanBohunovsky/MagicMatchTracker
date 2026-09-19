namespace MagicMatchTracker.Data.Seed.Parsers;

public sealed class LoseConditionParser
{
	public LoseCondition? Parse(int loseConditionId) => loseConditionId switch
	{
		0 => null,
		1 => LoseCondition.CombatDamage,
		2 => LoseCondition.LifeLoss,
		3 => LoseCondition.CommanderDamage,
		4 => LoseCondition.DeckedOut,
		5 => LoseCondition.WinEffect,
		6 => LoseCondition.Poison,
		7 => LoseCondition.LoseEffect,
		8 => LoseCondition.Other,
		9 => LoseCondition.Concede,
		_ => throw new FormatException($"Invalid lose condition ID: {loseConditionId}"),
	};
}