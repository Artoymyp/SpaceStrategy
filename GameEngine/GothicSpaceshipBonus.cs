using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceStrategy
{
	public class GothicSpaceshipBonus
	{
		public GothicSpaceshipBonus(string name, string description)
		{
			DisplayName = name;
			switch (DisplayName) {
				case "ImprovedThrusters": Name = GothicSpaceshipBonusName.ImprovedThrusters; break;
				case "ImprovedTargetingSystem": Name = GothicSpaceshipBonusName.ImprovedTargetingSystem; break;
				default:
					break;
			}
			Description = description;
		}
		public GothicSpaceshipBonus(GameDataSet.BonusesRow data) : this(data.Name, data.Description) { }
		public string Description { get; private set; }
		public string DisplayName { get; private set; }
		public GothicSpaceshipBonusName Name { get; private set; }
	}
	public enum GothicSpaceshipBonusName
	{
		ImprovedThrusters,
		ImprovedTargetingSystem
	}
}
