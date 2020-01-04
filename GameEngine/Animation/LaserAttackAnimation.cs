using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SpaceStrategy.Animation
{
	internal class LanceAttackAnimation : AnimationObject
	{
		public LanceAttackAnimation(LanceWeapon source, Spaceship target)
			: base(target.Game, Duration, false)
		{
			this.target = target;
			this.source = source;
			List<int> weaponIndexes = new List<int>();
			if (source.Power==0)
				return;
			for (int i = 0; i < source.Power; i++)
			{
				weaponIndexes.Add(i);
			}
			for (int i = 0; i < source.Power; i++)
			{
				int index = Game.rand.Next(weaponIndexes.Count);
				workingWeapons.Add(weaponIndexes[index]);
				weaponIndexes.RemoveAt(index);
			}
			for (int i = 0; i < workingWeapons.Count; i++)
			{
				randWeaponAttackDistComponent.Add((Game.rand.NextDouble()-0.5) * target.Diameter);
			}
		}
		List<double> randWeaponAttackDistComponent = new List<double>();
		List<int> workingWeapons = new List<int>();
		static TimeSpan Duration { get { return new TimeSpan(0,0,0,0,400); } }
		static Color color { get { return Color.Red; } }
		Spaceship target;
		LanceWeapon source;
		internal override void Draw(Graphics dc)
		{
			float startDiam = 0;
			float curDiam;
			float endDiam = 0.7F;
			float blobPhase = 0.7F;
			float explosionPhase = 0.8F;
			curDiam = AnimationHelper.AnimateFloat(Phase < blobPhase ? Phase * 2 : 0, startDiam, endDiam);
			Point2d rectPointTL = new Point2d(-curDiam / 2, -curDiam / 2);
			System.Drawing.SizeF rectSize = new System.Drawing.SizeF(curDiam,curDiam);

			Vector dir = source.OwnerSpaceship.Position.Location.VectorTo(target.Position);
			Vector dirNorm = dir;
			dirNorm.Normalize();
			for (int i = 0; i < workingWeapons.Count; i++)
			{
				Position weaponPos;
				if (source.OwnerSpaceship.weaponPlacements.TryGetValue(source, out var weaponPoses))
				{
					weaponPos = weaponPoses[workingWeapons[i]];
				}
				else
					weaponPos = source.OwnerSpaceship.Position;
				Point2d globalWeaponPos = weaponPos.Location.TransformBy(source.OwnerSpaceship.Position);
				if (curDiam > 0)
				{
					DrawLaserBlob(dc, new RectangleF(rectPointTL + globalWeaponPos.ToVector(), rectSize), globalWeaponPos);
				}
				Point2d lineEnd = globalWeaponPos + dir - 0.5 * dirNorm + dirNorm * randWeaponAttackDistComponent[i];
				if (Phase >= blobPhase)
				{
					DrawLaserLine(dc, globalWeaponPos, lineEnd);
				}
				if (Phase > explosionPhase && !explosionsPlaced)
				{
					AnimationHelper.CreateAnimation(new RoundExplosionAnimation(new Position(lineEnd), new TimeSpan(0,0,0,0,1000),0,1.5F));
				}
			}
			if (Phase > explosionPhase && !explosionsPlaced)
			{
				explosionsPlaced = true;
			}
		}
		bool explosionsPlaced = false;
		private void DrawLaserLine(Graphics dc, Point2d source, Point2d target)
		{
			Pen p = new Pen(color, 0.05F);
			dc.DrawLine(p, source, target);			
		}
		private void DrawLaserBlob(Graphics dc, RectangleF blobRect, Point2d point)
		{
			if (blobRect != null)
				dc.FillEllipse(new SolidBrush(color), blobRect);

		}
	}
}
