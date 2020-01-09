using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceStrategy.Weapon
{
	abstract class DriveExplosionWeapon : LanceWeapon
	{
		protected DriveExplosionWeapon(GothicSpaceship owner, float range, int power)
			: base(owner, range, power, Side.All) { }

		public override int Power
		{
			get { return power; }
			set { power = value; }
		}

		TimeSpan ExplosionDuration
		{
			get { return new TimeSpan(0, 0, 3); }
		}

		internal override void Attack(GothicSpaceshipBase attackedSpaceship, IEnumerable<SpaceshipWeapon> allAttackingWeapons)
		{
			int damage = 0;
			if (attackedSpaceship is GothicSpaceship) {
				damage = Dice.RolledDicesCount(6, Power, 4, string.Format("Удар взрывной волной по кораблю {0}.",
					attackedSpaceship));
			}
			else if (attackedSpaceship is TorpedoSalvo) {
				damage = Dice.RolledDicesCount(6, Power, 6, "Удар взрывной волной по кораблю по торпедному залпу.");
			}

			//var attackAnimation = new Animation.LanceAttackAnimation(this, attackedSpaceship);
			//AnimationHelper.CreateAnimation(attackAnimation);
			double distance = OwnerSpaceship.Position.DistanceTo(attackedSpaceship.Position);
			double c = distance / Range;
			attackedSpaceship.Attacked(this, damage, new TimeSpan(0, 0, 0, 0, (int)(ExplosionDuration.TotalMilliseconds * c)));
		}

		internal void Explode()
		{
			var animation = new RoundExplosionAnimation(OwnerSpaceship.Position, ExplosionDuration, 0.0F, Range);
			AnimationHelper.CreateAnimation(animation);

			List<GothicSpaceshipBase> affectedSSs = Game.SpaceBodies.Where(a => a != OwnerSpaceship && a.Position.DistanceTo(OwnerSpaceship.Position) <= Range).ToList();
			foreach (GothicSpaceshipBase ss in affectedSSs) Attack(ss, new List<SpaceshipWeapon> {this});
			double bmsFieldRadius = Math.Sqrt(Power) * BlastMarker.CollisionRadius;
			for (int i = 0; i < Power; i++) {
				double r = Game.Rand.NextDouble() * bmsFieldRadius;
				double ro = Game.Rand.NextDouble() * GeometryHelper.DoublePi;
				Point2d bmsPos = new Point2d(r, ro).ToEuclidCs(OwnerSpaceship.Position);
				var bm = new BlastMarker(OwnerSpaceship.Game, new Position(bmsPos, ro), new TimeSpan());
				OwnerSpaceship.Game.AddGraphicObject(bm);
			}
		}
	}
}