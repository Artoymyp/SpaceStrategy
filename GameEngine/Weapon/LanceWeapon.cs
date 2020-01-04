using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceStrategy
{
    public class LanceWeapon : DirectFireWeapon
    {
        protected LanceWeapon(GothicSpaceship owner, float range, int power, Side side) : base(owner, side, 0, range, power, WeaponType.Lance) { }
        public LanceWeapon(GothicSpaceship owner, SpaceStrategy.GameDataSet.SpaceshipClassWeaponryRow data) : base(owner, data) { }
        internal override void Attack(GothicSpaceshipBase attackedSpaceship, IEnumerable<SpaceshipWeapon> allAttackingWeapons)
        {
            if (IsUsed)
                return;
                   
     
            int damage = 0;
            if (attackedSpaceship is GothicSpaceship)
            {
                damage = Dice.RolledDicesCount(6, Power, 4, string.Format("Атака лазеров борта {0} корабля {1} по кораблю {2}.",
                    SpaceshipSide.ToString(),
                    OwnerSpaceship.ToString(),
                    attackedSpaceship.ToString()));
				if (OwnerSpaceship.SpecialOrder == GothicOrder.LockOn && damage < Power)
					damage += Dice.RolledDicesCount(6, Power - damage, 4, "Корректировка прицела.");
            }
            else if (attackedSpaceship is TorpedoSalvo)
            {
                damage = Dice.RolledDicesCount(6, Power, 6, string.Format("Атака лазеров борта {0} корабля {1} по торпедному залпу.",
                    SpaceshipSide.ToString(),
                    OwnerSpaceship.ToString()));
				if (OwnerSpaceship.SpecialOrder == GothicOrder.LockOn && damage < Power)
					damage += Dice.RolledDicesCount(6, Power - damage, 6, "Корректировка прицела.");
            }

            var attackAnimation = new Animation.LanceAttackAnimation(this, attackedSpaceship);
            AnimationHelper.CreateAnimation(attackAnimation);
            attackedSpaceship.Attacked(this, damage,attackAnimation.AnimationDuration);
            if (!Game.Params.Debug)
                IsUsed = true;
        }
    }
}
