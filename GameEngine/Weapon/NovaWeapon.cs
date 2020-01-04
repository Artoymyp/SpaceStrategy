using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceStrategy
{
    public class NovaWeapon : AimedWeapon
    {
        public NovaWeapon(GothicSpaceship owner, GameDataSet.SpaceshipClassWeaponryRow data) : base(owner, data) { }
        protected override void AimingOnTarget(object sender, CursorEventArgs e)
        {
            throw new NotImplementedException();
        }
        protected override void LockOnTarget(object sender, Point2dSelectEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
