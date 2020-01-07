using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceStrategy
{
	public enum SpaceshipCategory
	{
		CapitalShip,
		Escort,
		Ordnance,
		Defense
	}


	public class SpaceshipClass
	{
		public SpaceshipClass(string className, string typeName, SpaceshipCategory category, string raceName,
			int hitPoints, int turretsCount, int shieldPower,
			int frontArmor, int leftArmor, int rightArmor, int backArmor,
			double speed, double minTurnRadius, double minRunBeforeTurn, int maxTurnAngle, int points)
		{
			ClassName = className;
			Type = typeName;
			Category = category;
			HitPoints = hitPoints;
			TurretsPower = turretsCount;
			Shield = shieldPower;
			FrontArmor = frontArmor;
			LeftArmor = leftArmor;
			RightArmor = rightArmor;
			BackArmor = backArmor;
			Speed = speed;
			MinTurnRadius = minTurnRadius;
			Race = raceName;
			MinRunBeforeTurn = minRunBeforeTurn;
			MaxTurnAngle = maxTurnAngle;
			Points = points;
		}

		internal SpaceshipClass(GameDataSet.SpaceshipClassRow data) :
			this(data.Class, data.TypeName, GetCategory(data.Type), data.Race, data.HP, data.Turrets, data.Shield,
				data.FrontArmor, data.LeftArmor, data.RightArmor, data.BackArmor,
				data.Speed, data.MinTurnRadius, data.MinRunBeforeTurn, data.MaxTurnAngle, data.Points)
		{
			Id = data.Id;

			IEnumerable<GothicSpaceshipBonus> bonuses = GameDataAdapter.GetBonusesByClassName(ClassName);

			if (bonuses.Any(a => a.Name == GothicSpaceshipBonusName.ImprovedThrusters)) {
				AllAheadFullCoef = 5;
			}
			else {
				AllAheadFullCoef = 4;
			}
		}

		public int AllAheadFullCoef { get; }

		public int BackArmor { get; }

		public IEnumerable<GothicSpaceshipBonus> Bonuses { get; private set; }

		public SpaceshipCategory Category { get; }

		public string ClassName { get; }

		public int FrontArmor { get; }

		public int HitPoints { get; set; }

		public int Id { get; }

		public int LeftArmor { get; }

		public int MaxTurnAngle { get; }

		public double MinRunBeforeTurn { get; }

		public double MinTurnRadius { get; }

		public int Points { get; }

		public string Race { get; }

		public int RightArmor { get; }

		public int Shield { get; }

		public double Speed { get; }

		public int TurretsPower { get; }

		public string Type { get; }

		static SpaceshipCategory GetCategory(string categoryName)
		{
			switch (categoryName) {
				case "Capital ship": return SpaceshipCategory.CapitalShip;
				case "Escort": return SpaceshipCategory.Escort;
				case "Ordnance": return SpaceshipCategory.Ordnance;
				case "Defense": return SpaceshipCategory.Defense;
				default:
					throw new ArgumentOutOfRangeException("categoryName", "Invalid spaceship category name");
			}
		}
	}
}