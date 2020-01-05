using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceStrategy
{
	public class TorpedoWeapon : AimedWeapon
	{
		private TorpedoSalvo _launchedTorpedo;
		public TorpedoWeapon(GothicSpaceship owner, GameDataSet.SpaceshipClassWeaponryRow data) : base(owner, data) {
			LoadedTorpedoCount = this.Power;
		}
		public int LoadedTorpedoCount { get; private set; }
		public void Reload()
		{
			LoadedTorpedoCount = this.Power;
			IsUsed = false;
		}
		public void Launch(int torpedoCount)
		{
			Game.CursorMove+=AimingOnTarget;
			Game.PointSelected += LockOnTarget;
		
			int torpedoToLaunch = torpedoCount;
			if (torpedoToLaunch > LoadedTorpedoCount)
				torpedoToLaunch = LoadedTorpedoCount;
			LoadedTorpedoCount -= torpedoToLaunch;
			_launchedTorpedo = new TorpedoSalvo(OwnerSpaceship.Game, OwnerSpaceship.Player, this, OwnerSpaceship.Position, torpedoToLaunch, Range, LineColor);
			
			Game.AddTorpedoSalvo(_launchedTorpedo);
			Game.CommittedAttacks.Add(new Tuple<Spaceship, Spaceship>(_launchedTorpedo, this.OwnerSpaceship));
			Game.CommittedAttacks.Add(new Tuple<Spaceship, Spaceship>(this.OwnerSpaceship, _launchedTorpedo));
		}

		protected override void AimingOnTarget(object sender, CursorEventArgs e)
		{
			var curDir = _launchedTorpedo.Position.Location.VectorTo(e.Position);
			if (curDir == new Vector())
				curDir = new Vector(1, 0);
			else
				curDir.Normalize();
			double dist = GeometryHelper.Distance(curDir.ToRadian() + GeometryHelper.PiDiv4, curDir.ToRadian() - GeometryHelper.PiDiv4, true);
			Position pos = new Position(_launchedTorpedo.Position.Location, curDir);
			//if (Math.Abs(pos.Degree - Owner.Position.Degree) <= 45)
			//	launchedTorpedo.Position = pos;
			//else
			//	launchedTorpedo.Position = new Position(launchedTorpedo.Position.Location, Owner.Position.Direction);
			if (GeometryHelper.IsBetween(pos.Angle+GeometryHelper.PiDiv4, pos.Angle-GeometryHelper.PiDiv4, OwnerSpaceship.Position.Angle))
				_launchedTorpedo.Position = pos;
			else
				_launchedTorpedo.Position = new Position(_launchedTorpedo.Position.Location, OwnerSpaceship.Position.Direction);
		}

		protected override void LockOnTarget(object sender, Point2dSelectEventArgs e)
		{
			Game.CursorMove -= AimingOnTarget;
			Game.PointSelected -= LockOnTarget;
			_launchedTorpedo.Launch(this);
			IsUsed = true;
		}
		public override int Power
		{
			get
			{
				if (OwnerSpaceship.SpecialOrder != GothicOrder.BraceForImpact)
				{
					return power;
				}
				else
				{
					return GeometryHelper.RoundUp(power / 2);
				}
			}
			set
			{
				base.Power = value;
			}
		}
	}
}
