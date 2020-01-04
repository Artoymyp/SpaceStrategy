using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceStrategy
{
    public class TorpedoWeapon : AimedWeapon
	{
		private TorpedoSalvo launchedTorpedo;
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
			launchedTorpedo = new TorpedoSalvo(OwnerSpaceship.Game, OwnerSpaceship.Player, this, OwnerSpaceship.Position, torpedoToLaunch, Range, LineColor);
			
			Game.AddTorpedoSalvo(launchedTorpedo);
			Game.CommittedAttacks.Add(new Tuple<Spaceship, Spaceship>(launchedTorpedo, this.OwnerSpaceship));
			Game.CommittedAttacks.Add(new Tuple<Spaceship, Spaceship>(this.OwnerSpaceship, launchedTorpedo));
		}

		protected override void AimingOnTarget(object sender, CursorEventArgs e)
		{
			var curDir = launchedTorpedo.Position.Location.VectorTo(e.Position);
			if (curDir == new Vector())
				curDir = new Vector(1, 0);
			else
				curDir.Normalize();
            double dist = GeometryHelper.Distance(curDir.ToRadian() + GeometryHelper.PiDiv4, curDir.ToRadian() - GeometryHelper.PiDiv4, true);
			Position pos = new Position(launchedTorpedo.Position.Location, curDir);
			//if (Math.Abs(pos.Degree - Owner.Position.Degree) <= 45)
			//	launchedTorpedo.Position = pos;
			//else
			//	launchedTorpedo.Position = new Position(launchedTorpedo.Position.Location, Owner.Position.Direction);
			if (GeometryHelper.IsBetween(pos.Angle+GeometryHelper.PiDiv4, pos.Angle-GeometryHelper.PiDiv4, OwnerSpaceship.Position.Angle))
				launchedTorpedo.Position = pos;
			else
				launchedTorpedo.Position = new Position(launchedTorpedo.Position.Location, OwnerSpaceship.Position.Direction);
		}

        protected override void LockOnTarget(object sender, Point2dSelectEventArgs e)
		{
			Game.CursorMove -= AimingOnTarget;
			Game.PointSelected -= LockOnTarget;
			launchedTorpedo.Launch(this);
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
