using System.Collections.Generic;
using System.Linq;

namespace SpaceStrategy
{
	public class BasicScenario : Scenario
	{
		readonly int _maxTurnCountForEachPlayer = 8;

		public BasicScenario(Game game)
			: base(game)
		{
			Name = "Cruiser clash";
			SingleShipMaxPoints = 185;
			TotalPointsLimit = SingleShipMaxPoints * 4;
			MinPlayerCount = 2;
			MaxPlayerCount = 2;
			MaxPlayerSpaceshipCount = 4;
		}

		public override IList<SpaceshipClass> GetAvailableSpaceshipClasses(string raceId)
		{
			GameDataSet.SpaceshipClassDataTable classRows = Game.GameData.GetSpaceshipClassesByRaceId(raceId);
			IEnumerable<SpaceshipClass> availableClasses = classRows.Select(a => new SpaceshipClass(a));

			return availableClasses.Where(a => a.Points <= SingleShipMaxPoints && a.Category == SpaceshipCategory.CapitalShip).ToList();
		}

		public override bool IsComplete()
		{
			foreach (Player player in Game.Players)
				if (player.Spaceships.All(a => a.IsDestroyed != CatastrophicDamage.None)) {
					return true;
				}

			if (Game.CompletedTurnsCount >= _maxTurnCountForEachPlayer * Game.Players.Count) {
				return true;
			}

			return false;
		}
	}


	public abstract class Scenario
	{
		protected Scenario(Game game)
		{
			Game = game;
		}

		public int MaxPlayerCount { get; protected set; }

		public int MaxPlayerSpaceshipCount { get; protected set; }

		public int MinPlayerCount { get; protected set; }

		public string Name { get; protected set; }

		public int SingleShipMaxPoints { get; protected set; }

		public int TotalPointsLimit { get; protected set; }

		protected Game Game { get; }

		public abstract IList<SpaceshipClass> GetAvailableSpaceshipClasses(string raceId);

		public abstract bool IsComplete();
	}
}