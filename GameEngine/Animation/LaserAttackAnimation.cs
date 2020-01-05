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
			this._target = target;
			this._source = source;
			List<int> weaponIndexes = new List<int>();
			if (source.Power==0)
				return;
			for (int i = 0; i < source.Power; i++)
			{
				weaponIndexes.Add(i);
			}
			for (int i = 0; i < source.Power; i++)
			{
				int index = Game.Rand.Next(weaponIndexes.Count);
				_workingWeapons.Add(weaponIndexes[index]);
				weaponIndexes.RemoveAt(index);
			}
			for (int i = 0; i < _workingWeapons.Count; i++)
			{
				_randWeaponAttackDistComponent.Add((Game.Rand.NextDouble()-0.5) * target.Diameter);
			}
		}
		List<double> _randWeaponAttackDistComponent = new List<double>();
		List<int> _workingWeapons = new List<int>();
		static TimeSpan Duration { get { return new TimeSpan(0,0,0,0,400); } }
		static Color Color { get { return Color.Red; } }
		Spaceship _target;
		LanceWeapon _source;
		internal override void Draw(Graphics dc)
		{
			float startDiam = 0;
			float curDiam;
			float endDiam = 0.7F;
			float blobPhase = 0.7F;
			float explosionPhase = 0.8F;
			curDiam = AnimationHelper.AnimateFloat(Phase < blobPhase ? Phase * 2 : 0, startDiam, endDiam);
			Point2d rectTopLeftPoint = new Point2d(-curDiam / 2, -curDiam / 2);
			System.Drawing.SizeF rectSize = new System.Drawing.SizeF(curDiam,curDiam);

			Vector dir = _source.OwnerSpaceship.Position.Location.VectorTo(_target.Position);
			Vector dirNorm = dir;
			dirNorm.Normalize();
			for (int i = 0; i < _workingWeapons.Count; i++)
			{
				Position weaponPos;
				if (_source.OwnerSpaceship.WeaponPlacements.TryGetValue(_source, out var weaponPoses))
				{
					weaponPos = weaponPoses[_workingWeapons[i]];
				}
				else
					weaponPos = _source.OwnerSpaceship.Position;
				Point2d globalWeaponPos = weaponPos.Location.TransformBy(_source.OwnerSpaceship.Position);
				if (curDiam > 0)
				{
					DrawLaserBlob(dc, new RectangleF(rectTopLeftPoint + globalWeaponPos.ToVector(), rectSize), globalWeaponPos);
				}
				Point2d lineEnd = globalWeaponPos + dir - 0.5 * dirNorm + dirNorm * _randWeaponAttackDistComponent[i];
				if (Phase >= blobPhase)
				{
					DrawLaserLine(dc, globalWeaponPos, lineEnd);
				}
				if (Phase > explosionPhase && !_explosionsPlaced)
				{
					AnimationHelper.CreateAnimation(new RoundExplosionAnimation(new Position(lineEnd), new TimeSpan(0,0,0,0,1000),0,1.5F));
				}
			}
			if (Phase > explosionPhase && !_explosionsPlaced)
			{
				_explosionsPlaced = true;
			}
		}
		bool _explosionsPlaced = false;
		private void DrawLaserLine(Graphics dc, Point2d source, Point2d target)
		{
			Pen p = new Pen(Color, 0.05F);
			dc.DrawLine(p, source, target);			
		}
		private void DrawLaserBlob(Graphics dc, RectangleF blobRect, Point2d point)
		{
			if (blobRect != null)
				dc.FillEllipse(new SolidBrush(Color), blobRect);

		}
	}
}
