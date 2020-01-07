using System;
using System.Collections.Generic;
using System.Drawing;

namespace SpaceStrategy.Animation
{
	class LanceAttackAnimation : AnimationObject
	{
		readonly List<double> _randWeaponAttackDistComponent = new List<double>();

		readonly LanceWeapon _source;

		readonly Spaceship _target;

		readonly List<int> _workingWeapons = new List<int>();
		bool _explosionsPlaced;

		public LanceAttackAnimation(LanceWeapon source, Spaceship target)
			: base(target.Game, Duration)
		{
			_target = target;
			_source = source;
			var weaponIndexes = new List<int>();
			if (source.Power == 0) {
				return;
			}

			for (int i = 0; i < source.Power; i++) weaponIndexes.Add(i);
			for (int i = 0; i < source.Power; i++) {
				int index = Game.Rand.Next(weaponIndexes.Count);
				_workingWeapons.Add(weaponIndexes[index]);
				weaponIndexes.RemoveAt(index);
			}

			for (int i = 0; i < _workingWeapons.Count; i++) _randWeaponAttackDistComponent.Add((Game.Rand.NextDouble() - 0.5) * target.Diameter);
		}

		static Color Color
		{
			get { return Color.Red; }
		}

		static TimeSpan Duration
		{
			get { return new TimeSpan(0, 0, 0, 0, 400); }
		}

		public override void Draw(Graphics dc)
		{
			float startDiam = 0;
			float curDiam;
			float endDiam = 0.7F;
			float blobPhase = 0.7F;
			float explosionPhase = 0.8F;
			curDiam = AnimationHelper.AnimateFloat(Phase < blobPhase ? Phase * 2 : 0, startDiam, endDiam);
			var rectTopLeftPoint = new Point2d(-curDiam / 2, -curDiam / 2);
			var rectSize = new SizeF(curDiam, curDiam);

			Vector dir = _source.OwnerSpaceship.Position.Location.VectorTo(_target.Position);
			Vector dirNorm = dir;
			dirNorm.Normalize();
			for (int i = 0; i < _workingWeapons.Count; i++) {
				Position weaponPos;
				if (_source.OwnerSpaceship.WeaponPlacements.TryGetValue(_source, out List<Position> weaponPoses)) {
					weaponPos = weaponPoses[_workingWeapons[i]];
				}
				else {
					weaponPos = _source.OwnerSpaceship.Position;
				}

				Point2d globalWeaponPos = weaponPos.Location.TransformBy(_source.OwnerSpaceship.Position);
				if (curDiam > 0) {
					DrawLaserBlob(dc, new RectangleF(rectTopLeftPoint + globalWeaponPos.ToVector(), rectSize), globalWeaponPos);
				}

				Point2d lineEnd = globalWeaponPos + dir - 0.5 * dirNorm + dirNorm * _randWeaponAttackDistComponent[i];
				if (Phase >= blobPhase) {
					DrawLaserLine(dc, globalWeaponPos, lineEnd);
				}

				if (Phase > explosionPhase && !_explosionsPlaced) {
					AnimationHelper.CreateAnimation(new RoundExplosionAnimation(new Position(lineEnd), new TimeSpan(0, 0, 0, 0, 1000), 0, 1.5F));
				}
			}

			if (Phase > explosionPhase && !_explosionsPlaced) {
				_explosionsPlaced = true;
			}
		}

		void DrawLaserBlob(Graphics dc, RectangleF blobRect, Point2d point)
		{
			if (blobRect != null) {
				dc.FillEllipse(new SolidBrush(Color), blobRect);
			}
		}

		void DrawLaserLine(Graphics dc, Point2d source, Point2d target)
		{
			var p = new Pen(Color, 0.05F);
			dc.DrawLine(p, source, target);
		}
	}
}