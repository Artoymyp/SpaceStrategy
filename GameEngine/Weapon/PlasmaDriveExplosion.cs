using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceStrategy.Weapon
{
	internal class PlasmaDriveExplosion : DriveExplosionWeapon
	{
		public PlasmaDriveExplosion(GothicSpaceship owner, float range) : base(owner, range, GeometryHelper.RoundUp((float)owner.Class.HP / 2.0)) { }
	}
}
