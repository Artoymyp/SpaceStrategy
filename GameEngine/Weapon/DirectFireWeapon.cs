namespace SpaceStrategy
{
	public abstract class DirectFireWeapon : SpaceshipWeapon
	{
		protected DirectFireWeapon(GothicSpaceship owner, Side side, float minRange, float range, int power, WeaponType type) : base(owner, side, minRange, range, power, type) { }

		protected DirectFireWeapon(GothicSpaceship ownerSpaceship, GameDataSet.SpaceshipClassWeaponryRow data) : base(ownerSpaceship, data) { }

		public override int Power
		{
			get
			{
				if (OwnerSpaceship.SpecialOrder != GothicOrder.AllAheadFull ||
				    OwnerSpaceship.SpecialOrder != GothicOrder.ComeToNewDirection ||
				    OwnerSpaceship.SpecialOrder != GothicOrder.BurnRetros ||
				    OwnerSpaceship.SpecialOrder != GothicOrder.BraceForImpact) {
					return power;
				}

				return GeometryHelper.RoundUp(power / 2);
			}
			set { base.Power = value; }
		}
	}
}