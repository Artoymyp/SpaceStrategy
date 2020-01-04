using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceStrategy
{
	public class SpaceshipClass
	{
		public SpaceshipClass(string className, string typeName, SpaceshipCategory category, string raceName, 
			int hp, int turretsCount, int shieldPower, 
			int frontArmor, int leftArmor, int rightArmor, int backArmor,
			double speed, double minTurnRadius, double minRunBeforeTurn, int maxTurnAngle, int points)
		{
			ClassName = className;
			Type = typeName;
            Category = category;
			HP = hp;
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
		internal SpaceshipClass(GameDataSet.SpaceshipClassRow data):
			this(data.Class, data.TypeName, GetCategory(data.Type), data.Race,data.HP,data.Turrets,data.Shield,
			data.FrontArmor,data.LeftArmor,data.RightArmor,data.BackArmor,
			data.Speed,data.MinTurnRadius, data.MinRunBeforeTurn,data.MaxTurnAngle,data.Points){
			Id = data.Id;
			
			IEnumerable<GothicSpaceshipBonus> Bonuses = GameDataAdapter.GetBonusesByClassName(ClassName);

			if (Bonuses.Any(a => a.Name == GothicSpaceshipBonusName.ImprovedThrusters))
				AllAheadFullCoef = 5;
			else
				AllAheadFullCoef = 4;
		}
		public int Id { get; private set; }
		public string ClassName { get; private set; }
		public string Type { get; private set; }
		public SpaceshipCategory Category { get; private set; }
		public int Shield { get; private set; }
		public int FrontArmor { get; private set; }
		public int LeftArmor { get; private set; }
		public int RightArmor { get; private set; }
		public int BackArmor { get; private set; }
		public double Speed { get; private set; }
		public double MinTurnRadius { get; private set; }
		public string Race { get; private set; }
		public int MaxTurnAngle { get; private set; }
		public double MinRunBeforeTurn { get; private set; }
		public int Points { get; private set; }
		public int AllAheadFullCoef { get; private set; }
		public int TurretsPower { get; private set; }
		public int HP { get; set; }
		public IEnumerable<GothicSpaceshipBonus> Bonuses { get; private set; }
		private static SpaceshipCategory GetCategory(string categoryName)
		{
			switch (categoryName) {
				case "Capital ship": return SpaceshipCategory.CapitalShip;
				case "Escort": return SpaceshipCategory.Escort;
				case "Ordnance": return SpaceshipCategory.Ordnance;
				case "Defense": return SpaceshipCategory.Defense;
				default:
					throw new ArgumentOutOfRangeException("categoryName","Invalid spaceship category name");
			}
		}
	}
	public enum SpaceshipCategory
	{
		CapitalShip,
		Escort,
		Ordnance,
		Defense
	}
}
