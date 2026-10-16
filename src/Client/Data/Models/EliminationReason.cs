namespace MagicMatchTracker.Client.Data.Models;

public enum EliminationReason
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

public static class EliminationReasonExtensions
{
	extension(EliminationReason reason)
	{
		public string DisplayName => reason switch
		{
			EliminationReason.Combat => "Combat",
			EliminationReason.Burn => "Burn",
			EliminationReason.Commander => "Commander",
			EliminationReason.DeckedOut => "Decked out",
			EliminationReason.Poison => "Poison",
			EliminationReason.Effect => "Effect",
			EliminationReason.Concede => "Concede",
			EliminationReason.Other => "Other",
			_ => "Unknown",
		};

		public string Description => reason switch
		{
			EliminationReason.Combat => "Took lethal damage via combat",
			EliminationReason.Burn => "Got to 0 or less life due to non-combat damage or loss of life effect",
			EliminationReason.Commander => "Accumulated 21+ combat damage from a single commander",
			EliminationReason.DeckedOut => "Tried to draw a card from an empty library",
			EliminationReason.Poison => "Accumulated 10+ poison counters",
			EliminationReason.Effect => "An effect caused this player to lose or another player to win",
			EliminationReason.Concede => "The player conceded",
			EliminationReason.Other => "Lost in a way that's not covered by the other reasons",
			_ => "Unknown elimination reason",
		};

		public string Icon => reason switch
		{
			EliminationReason.Combat => "swords",
			EliminationReason.Burn => "mode_heat",
			EliminationReason.Commander => "crown",
			EliminationReason.DeckedOut => "cyclone",
			EliminationReason.Poison => "water_drop",
			EliminationReason.Effect => "star_shine",
			EliminationReason.Concede => "door_back",
			EliminationReason.Other => "skull",
			_ => "skull",
		};
	}
}