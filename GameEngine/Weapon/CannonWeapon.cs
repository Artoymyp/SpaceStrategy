using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceStrategy
{
	public class CannonWeapon : DirectFireWeapon
	{
		public CannonWeapon(GothicSpaceship owner, GameDataSet.SpaceshipClassWeaponryRow data) : base(owner, data) { }
		internal override void Attack(GothicSpaceshipBase attackedSpaceship, IEnumerable<SpaceshipWeapon> allAttackingWeapons)
		{
			IEnumerable<CannonWeapon> attackingCannons = allAttackingWeapons
				.OfType<CannonWeapon>()
				.Where(a=>!a.IsUsed);
			if (!attackingCannons.Any())
				return;
			bool blastMarkersOnLineOfFire=GunneryTable.GetBlastMarkersOnLine(Game.BlastMarkers,OwnerSpaceship.Position.Location, attackedSpaceship.Position.Location)||OwnerSpaceship.BlastMarkersAtBase.Any()||attackedSpaceship.BlastMarkersAtBase.Any();
			double distance = OwnerSpaceship.Position.DistanceTo(attackedSpaceship.Position);

			TargetOrientation targetOrientation;
			Side attackingSide = GothicSpaceshipBase.GetAttackingSide(this.OwnerSpaceship, attackedSpaceship);
			Side attackedSide = GothicSpaceshipBase.GetAttackedSide(this.OwnerSpaceship, attackedSpaceship);
			switch (attackedSide)
			{
				case Side.Left:
				case Side.Right:
					targetOrientation = TargetOrientation.Abeam; break;
				case Side.Front:
					targetOrientation = TargetOrientation.Closing; break;
				case Side.Back:
					targetOrientation = TargetOrientation.MovingAway; break;
				default:
					throw new InvalidOperationException("Не удалось определить относительную ориентацию цели.");
			}
			int diceCount = GunneryTable.GetDiceCount(
				attackingCannons.Aggregate(0, (totalPower,cannon)=>totalPower+=cannon.Power),
				attackedSpaceship.Class.Category,
				targetOrientation,
				distance, 
				Game.GetSpaceshipMoveDistanceInLastTurn(attackedSpaceship),
				blastMarkersOnLineOfFire);
			
			
			int damage = 0;
			if (attackedSpaceship is GothicSpaceship)
			{
				damage = Dice.RolledDicesCount(6, diceCount, attackedSpaceship.GetArmor(attackedSide), string.Format("Атака батарей борта {0} корабля {1} по борту {2} корабля {3} на расстоянии {4:0.00} на курсе {5}.",
					attackingSide.ToString(),
					OwnerSpaceship.ToString(),
					attackedSide.ToString(),
					attackedSpaceship.ToString(),
					distance,
					targetOrientation.ToString()));
				if (OwnerSpaceship.SpecialOrder == GothicOrder.LockOn && damage < diceCount)
					damage += Dice.RolledDicesCount(6, diceCount - damage, attackedSpaceship.GetArmor(attackedSide), "Корректировка прицела.");
			}
			else if (attackedSpaceship is TorpedoSalvo)
			{
				damage = Dice.RolledDicesCount(6, diceCount, 6, string.Format("Атака батарей борта {0} корабля {1} по торпедному залпу на расстоянии {4:0.00} на курсе {5}.",
					attackingSide.ToString(),
					OwnerSpaceship.ToString(),
					attackedSide.ToString(),
					attackedSpaceship.ToString(),
					distance.ToString(),
					targetOrientation.ToString()));
				if (OwnerSpaceship.SpecialOrder == GothicOrder.LockOn && damage < diceCount)
					damage += Dice.RolledDicesCount(6, diceCount - damage, 6, "Корректировка прицела.");
			}

			var attackAnimation = new Animation.CannonAttackAnimation(this, attackedSpaceship);
			AnimationHelper.CreateAnimation(attackAnimation);

			attackedSpaceship.Attacked(this, damage, attackAnimation.AnimationDuration);

			if (!Game.Params.Debug)
			{
				foreach (var cannon in attackingCannons)
				{
					cannon.IsUsed = true;
				}
			}
		}
	}
}
