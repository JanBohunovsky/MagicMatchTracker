namespace MagicMatchTracker.Client.Data.Models;

public enum EliminationMethod
{
	Combat,
	Burn,
	Commander,
	DeckedOut,
	Poison,
	Effect,
	Concede,
	Other = byte.MaxValue,
}

public static class EliminationMethodExtensions
{
	extension(EliminationMethod method)
	{
		public string DisplayName => method switch
		{
			EliminationMethod.Combat => "Combat",
			EliminationMethod.Burn => "Burn",
			EliminationMethod.Commander => "Commander",
			EliminationMethod.DeckedOut => "Decked out",
			EliminationMethod.Poison => "Poison",
			EliminationMethod.Effect => "Effect",
			EliminationMethod.Concede => "Concede",
			EliminationMethod.Other => "Other",
			_ => "Unknown",
		};

		public string Description => method switch
		{
			EliminationMethod.Combat => "Took lethal damage via combat",
			EliminationMethod.Burn => "Got to 0 or less life due to non-combat damage or loss of life effect",
			EliminationMethod.Commander => "Accumulated 21+ combat damage from a single commander",
			EliminationMethod.DeckedOut => "Tried to draw a card from an empty library",
			EliminationMethod.Poison => "Accumulated 10+ poison counters",
			EliminationMethod.Effect => "An effect caused this player to lose or another player to win",
			EliminationMethod.Concede => "The player conceded",
			EliminationMethod.Other => "Lost in a way that's not covered by the other reasons",
			_ => "Unknown elimination reason",
		};

		public string Icon => method switch
		{
			EliminationMethod.Combat => "swords",
			EliminationMethod.Burn => "mode_heat",
			EliminationMethod.Commander => "crown",
			EliminationMethod.DeckedOut => "cyclone",
			EliminationMethod.Poison => "water_drop",
			EliminationMethod.Effect => "star_shine",
			EliminationMethod.Concede => "door_back",
			EliminationMethod.Other => "skull",
			_ => "skull",
		};
	}
}