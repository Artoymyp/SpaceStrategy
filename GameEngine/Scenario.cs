using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceStrategy
{
	public abstract class Scenario
	{
		protected Game Game { get; private set; }
		protected Scenario(Game game)
		{
			this.Game = game;
		}
		public string Name { get; protected set; }
		public int TotalPointsLimit { get; protected set; }
		public int SingleShipMaxPoints { get; protected set; }
		public int MinPlayerCount { get; protected set; }
		public int MaxPlayerCount { get; protected set; }
		public int MaxPlayerSpaceshipCount { get; protected set; }
		public abstract bool IsComplete();
		public abstract IList<SpaceshipClass> GetAvailableSpaceshipClasses(string raceId);
	}
	public class BasicScenario: Scenario
	{
		public BasicScenario(Game game)
			: base(game)
		{
			Name = "Cruiser clash";
			SingleShipMaxPoints = 185;
			TotalPointsLimit = SingleShipMaxPoints*4;
			MinPlayerCount = 2;
			MaxPlayerCount = 2;
			MaxPlayerSpaceshipCount = 4;
		}
		int _maxTurnCountForEachPlayer = 8;
		public override bool IsComplete()
		{
			foreach (var player in Game.Players) {
				if (!player.Spaceships.Any(a=>a.IsDestroyed==CatastrophycDamage.None))
					return true;
			}
			if (Game.CompletedTurnsCount >= _maxTurnCountForEachPlayer * Game.Players.Count)
				return true;
			return false;
		}
		public override IList<SpaceshipClass> GetAvailableSpaceshipClasses(string raceId)
			{
				var classRows = Game.GameData.GetSpaceshipClassesByRaceId(raceId);
				IEnumerable<SpaceshipClass> availableClasses = classRows.Select(a => new SpaceshipClass(a));

				return availableClasses.Where(a=>a.Points<=SingleShipMaxPoints && a.Category== SpaceshipCategory.CapitalShip).ToList();
			}
	}
}
