namespace SpaceStrategy.Weapon
{
	class PlasmaDriveExplosion : DriveExplosionWeapon
	{
		public PlasmaDriveExplosion(GothicSpaceship owner, float range) : base(owner, range, GeometryHelper.RoundUp(owner.Class.HitPoints / 2.0)) { }
	}
}