using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceStrategy
{
    public abstract class AimedWeapon:SpaceshipWeapon
    {
        public AimedWeapon(GothicSpaceship owner, GameDataSet.SpaceshipClassWeaponryRow data) : base(owner, data) { }
        protected abstract void AimingOnTarget(object sender, CursorEventArgs e);
        protected abstract void LockOnTarget(object sender, Point2dSelectEventArgs e);
        internal sealed override void Attack(GothicSpaceshipBase attackedSpaceship, IEnumerable<SpaceshipWeapon> allAttackingWeapons)
        {
            throw new NotSupportedException();
        }		
    }
}
