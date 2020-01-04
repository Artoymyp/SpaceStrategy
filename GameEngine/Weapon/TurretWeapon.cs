using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceStrategy
{
	public class TurretWeapon : SpaceshipWeapon
	{
		public TurretWeapon(GothicSpaceship owner, int power)
			: base(owner, Side.All, 0, 10, power, WeaponType.Turret)
		{

		}
		internal override void Attack(GothicSpaceshipBase attackedSpaceship, IEnumerable<SpaceshipWeapon> allAttackingWeapons)
		{
			if (attackedSpaceship is TorpedoSalvo)
			{
				TorpedoSalvo torpedo = attackedSpaceship as TorpedoSalvo;
				int destroyedTorpedoCount = Dice.RolledDicesCount(6, Power, 4, "Anti-torpedo turrets attack");
				double antiTorpedoAttackDistance = torpedo.Size + OwnerSpaceship.Diameter * 3;
				TimeSpan timeBeforeTorpedoImpact = new TimeSpan(0, 0, 0, 0, (int)(antiTorpedoAttackDistance / OwnerSpaceship.Speed * 1000));
				torpedo.SetMaxTorpedoDestructionDelay(timeBeforeTorpedoImpact);
				torpedo.Attacked(this, destroyedTorpedoCount, new TimeSpan());
			}
		}
	}
}
