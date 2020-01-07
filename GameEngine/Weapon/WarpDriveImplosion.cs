namespace SpaceStrategy.Weapon
{
	class WarpDriveImplosion : DriveExplosionWeapon
	{
		public WarpDriveImplosion(GothicSpaceship owner, float range) : base(owner, range, owner.Class.HitPoints) { }
	}
}