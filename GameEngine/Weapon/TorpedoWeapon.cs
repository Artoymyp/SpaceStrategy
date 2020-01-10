using System;

namespace SpaceStrategy
{
	public class TorpedoWeapon : AimedWeapon
	{
		TorpedoSalvo _launchedTorpedo;

		public TorpedoWeapon(GothicSpaceship owner, GameDataSet.SpaceshipClassWeaponryRow data) : base(owner, data)
		{
			LoadedTorpedoCount = Power;
		}

		public int LoadedTorpedoCount { get; private set; }

		public override int Power
		{
			get
			{
				if (OwnerSpaceship.SpecialOrder != GothicOrder.BraceForImpact) {
					return power;
				}

				return GeometryHelper.RoundUp(power / 2);
			}
			set { base.Power = value; }
		}

		public void Launch(int torpedoCount)
		{
			Game.CursorMove += AimingOnTarget;
			Game.PointSelected += LockOnTarget;

			int torpedoToLaunch = torpedoCount;
			if (torpedoToLaunch > LoadedTorpedoCount) {
				torpedoToLaunch = LoadedTorpedoCount;
			}

			LoadedTorpedoCount -= torpedoToLaunch;
			_launchedTorpedo = new TorpedoSalvo(OwnerSpaceship.Game, OwnerSpaceship.Player, this, OwnerSpaceship.Position, torpedoToLaunch, Range, LineColor);

			Game.AddTorpedoSalvo(_launchedTorpedo);
			Game.CommittedAttacks.Add(new Tuple<Spaceship, Spaceship>(_launchedTorpedo, OwnerSpaceship));
			Game.CommittedAttacks.Add(new Tuple<Spaceship, Spaceship>(OwnerSpaceship, _launchedTorpedo));
		}

		public void Reload()
		{
			LoadedTorpedoCount = Power;
			IsUsed = false;
		}

		protected override void AimingOnTarget(object sender, CursorEventArgs e)
		{
			Vector curDir = _launchedTorpedo.Position.Location.VectorTo(e.Position);
			if (curDir == new Vector()) {
				curDir = new Vector(1, 0);
			}
			else {
				curDir = curDir.GetNormalized();
			}

			double dist = GeometryHelper.Distance(curDir.ToRadian() + GeometryHelper.PiDiv4, curDir.ToRadian() - GeometryHelper.PiDiv4, true);
			var pos = new Position(_launchedTorpedo.Position.Location, curDir);

			//if (Math.Abs(pos.Degree - Owner.Position.Degree) <= 45)
			//	launchedTorpedo.Position = pos;
			//else
			//	launchedTorpedo.Position = new Position(launchedTorpedo.Position.Location, Owner.Position.Direction);
			if (GeometryHelper.IsBetween(pos.Angle + GeometryHelper.PiDiv4, pos.Angle - GeometryHelper.PiDiv4, OwnerSpaceship.Position.Angle)) {
				_launchedTorpedo.Position = pos;
			}
			else {
				_launchedTorpedo.Position = new Position(_launchedTorpedo.Position.Location, OwnerSpaceship.Position.Direction);
			}
		}

		protected override void LockOnTarget(object sender, Point2dSelectEventArgs e)
		{
			Game.CursorMove -= AimingOnTarget;
			Game.PointSelected -= LockOnTarget;
			_launchedTorpedo.Launch(this);
			IsUsed = true;
		}
	}
}