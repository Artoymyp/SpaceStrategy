namespace SpaceStrategy
{
	public enum GothicSpaceshipBonusName
	{
		ImprovedThrusters,
		ImprovedTargetingSystem
	}


	public class GothicSpaceshipBonus
	{
		public GothicSpaceshipBonus(string name, string description)
		{
			DisplayName = name;
			switch (DisplayName) {
				case "ImprovedThrusters":
					Name = GothicSpaceshipBonusName.ImprovedThrusters;
					break;
				case "ImprovedTargetingSystem":
					Name = GothicSpaceshipBonusName.ImprovedTargetingSystem;
					break;
			}

			Description = description;
		}

		public GothicSpaceshipBonus(GameDataSet.BonusesRow data) : this(data.Name, data.Description) { }

		public string Description { get; }

		public string DisplayName { get; }

		public GothicSpaceshipBonusName Name { get; }
	}
}