namespace MagicMatchTracker.Data.Models;

public enum LoseCondition
{
	CombatDamage,
	LifeLoss,
	CommanderDamage,
	DeckedOut,
	Poison,
	WinEffect,
	LoseEffect,
	Concede,
	Other = byte.MaxValue,
}

public static class LoseConditionExtensions
{
	extension(LoseCondition condition)
	{
		public string DisplayName => condition switch
		{
			LoseCondition.CombatDamage => "Combat damage",
			LoseCondition.LifeLoss => "Loss of life",
			LoseCondition.CommanderDamage => "Commander damage",
			LoseCondition.DeckedOut => "Decked out",
			LoseCondition.Poison => "Poison",
			LoseCondition.WinEffect => "\"Win the game\" effect",
			LoseCondition.LoseEffect => "\"Lose the game\" effect",
			LoseCondition.Concede => "Concede",
			LoseCondition.Other => "Other",
			_ => throw new ArgumentException($"Invalid lose condition: {condition}", nameof(condition)),
		};

		public string Description => condition switch
		{
			LoseCondition.CombatDamage => "Took lethal damage via combat",
			LoseCondition.LifeLoss => "Got to 0 or less life due to non-combat damage or life loss effect",
			LoseCondition.CommanderDamage => "Accumulated 21+ combat damage from a single commander",
			LoseCondition.DeckedOut => "Tried to draw a card from an empty library",
			LoseCondition.Poison => "Accumulated 10+ poison counters",
			LoseCondition.WinEffect => "An effect caused another player to win the game",
			LoseCondition.LoseEffect => "An effect caused the player to lose the game",
			LoseCondition.Concede => "The player has conceded",
			LoseCondition.Other => "Lost in a way that's not covered by the other conditions",
			_ => throw new ArgumentException($"Invalid lose condition: {condition}", nameof(condition)),
		};

		public bool HasKiller => condition is not LoseCondition.Concede;
	}
}