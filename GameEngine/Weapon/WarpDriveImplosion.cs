using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceStrategy.Weapon
{
	internal class WarpDriveImplosion: DriveExplosionWeapon
	{
		public WarpDriveImplosion(GothicSpaceship owner, float range) : base(owner, range, owner.Class.HP) { }
	}
}
