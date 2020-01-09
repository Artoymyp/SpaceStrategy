using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SpaceStrategy.Animation;

namespace SpaceStrategy
{
	public class CannonWeapon : DirectFireWeapon
	{
		public CannonWeapon(GothicSpaceship owner, GameDataSet.SpaceshipClassWeaponryRow data) : base(owner, data) { }

		internal override void Attack(GothicSpaceshipBase attackedSpaceship, IEnumerable<SpaceshipWeapon> allAttackingWeapons)
		{
			IEnumerable<CannonWeapon> attackingCannons = allAttackingWeapons
				.OfType<CannonWeapon>()
				.Where(a => !a.IsUsed);
			if (!attackingCannons.Any()) {
				return;
			}

			bool blastMarkersOnLineOfFire = GunneryTable.GetBlastMarkersOnLine(Game.BlastMarkers, OwnerSpaceship.Position.Location, attackedSpaceship.Position.Location) || OwnerSpaceship.BlastMarkersAtBase.Any() || attackedSpaceship.BlastMarkersAtBase.Any();
			double distance = OwnerSpaceship.Position.DistanceTo(attackedSpaceship.Position);

			TargetOrientation targetOrientation;
			Side attackingSide = GothicSpaceshipBase.GetAttackingSide(OwnerSpaceship, attackedSpaceship);
			Side attackedSide = GothicSpaceshipBase.GetAttackedSide(OwnerSpaceship, attackedSpaceship);
			switch (attackedSide) {
				case Side.Left:
				case Side.Right:
					targetOrientation = TargetOrientation.Abeam;
					break;
				case Side.Front:
					targetOrientation = TargetOrientation.Closing;
					break;
				case Side.Back:
					targetOrientation = TargetOrientation.MovingAway;
					break;
				default:
					throw new InvalidOperationException("Не удалось определить относительную ориентацию цели.");
			}

			int diceCount = GunneryTable.GetDiceCount(
				attackingCannons.Aggregate(0, (totalPower, cannon) => totalPower += cannon.Power),
				attackedSpaceship.Class.Category,
				targetOrientation,
				distance,
				Game.GetSpaceshipMoveDistanceInLastTurn(attackedSpaceship),
				blastMarkersOnLineOfFire);

			int damage = 0;
			if (attackedSpaceship is GothicSpaceship) {
				damage = Dice.RolledDicesCount(6, diceCount, attackedSpaceship.GetArmor(attackedSide), string.Format("Атака батарей борта {0} корабля {1} по борту {2} корабля {3} на расстоянии {4:0.00} на курсе {5}.",
					attackingSide.ToString(),
					OwnerSpaceship,
					attackedSide.ToString(),
					attackedSpaceship,
					distance,
					targetOrientation.ToString()));
				if (OwnerSpaceship.SpecialOrder == GothicOrder.LockOn && damage < diceCount) {
					damage += Dice.RolledDicesCount(6, diceCount - damage, attackedSpaceship.GetArmor(attackedSide), "Корректировка прицела.");
				}
			}
			else if (attackedSpaceship is TorpedoSalvo) {
				damage = Dice.RolledDicesCount(6, diceCount, 6, string.Format("Атака батарей борта {0} корабля {1} по торпедному залпу на расстоянии {4:0.00} на курсе {5}.",
					attackingSide.ToString(),
					OwnerSpaceship,
					attackedSide.ToString(),
					attackedSpaceship,
					distance.ToString(CultureInfo.InvariantCulture),
					targetOrientation.ToString()));
				if (OwnerSpaceship.SpecialOrder == GothicOrder.LockOn && damage < diceCount) {
					damage += Dice.RolledDicesCount(6, diceCount - damage, 6, "Корректировка прицела.");
				}
			}

			var attackAnimation = new CannonAttackAnimation(this, attackedSpaceship);
			AnimationHelper.CreateAnimation(attackAnimation);

			attackedSpaceship.Attacked(this, damage, attackAnimation.AnimationDuration);

			if (!Game.Params.Debug) {
				foreach (CannonWeapon cannon in attackingCannons) cannon.IsUsed = true;
			}
		}
	}
}