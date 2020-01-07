using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SpaceStrategy
{
	class RoundExplosionAnimation
		: AnimationObject
	{
		readonly float _endDiameter;
		readonly float _startDiameter;
		Position _explosionPosition;

		internal RoundExplosionAnimation(
			Position explosionPosition,
			TimeSpan explosionDuration,
			float startDiameter,
			float endDiameter) :
			base(AnimationHelper.Game, explosionDuration)
		{
			_explosionPosition = explosionPosition;
			_startDiameter = startDiameter;
			_endDiameter = endDiameter;
		}

		public override void Draw(Graphics dc)
		{
			GraphicsState oldDc = dc.Save();
			dc.TranslateTransform((float)_explosionPosition.Location.X, (float)_explosionPosition.Location.Y);
			if (_explosionPosition.Degree != 0) {
				dc.RotateTransform((float)_explosionPosition.Degree);
			}

			float curExplosionRadius = AnimationHelper.AnimateFloat(Phase, _startDiameter, _endDiameter) / 2;
			Color curColor = AnimationHelper.AnimateColor(Phase, Color.FromArgb(0xFF, 0xFF, 0xD7, 0), Color.FromArgb(0, 0, 0, 0), AnimationMode.Pow8);
			dc.FillEllipse(new SolidBrush(curColor), -curExplosionRadius, -curExplosionRadius, curExplosionRadius * 2, curExplosionRadius * 2);
			dc.Restore(oldDc);
		}
	}


	class SpaceshipRoundExplosionAnimation
		: AnimationObject
	{
		readonly float _endDiameter;
		readonly Spaceship _explodedSpaceship;
		readonly float _startDiameter;
		Position _relativeExplosionPosition;

		internal SpaceshipRoundExplosionAnimation(
			Spaceship explodedSpaceship,
			Position relativeExplosionPosition,
			TimeSpan explosionDuration,
			float startDiameter,
			float endDiameter) :
			base(AnimationHelper.Game, explosionDuration)
		{
			_explodedSpaceship = explodedSpaceship;
			_relativeExplosionPosition = relativeExplosionPosition;
			_startDiameter = startDiameter;
			_endDiameter = endDiameter;
		}

		public override void Draw(Graphics dc)
		{
			GraphicsState oldDc = dc.Save();
			dc.TranslateTransform((float)_explodedSpaceship.Position.Location.X, (float)_explodedSpaceship.Position.Location.Y);
			dc.RotateTransform((float)_explodedSpaceship.Position.Degree);
			dc.TranslateTransform((float)_relativeExplosionPosition.Location.X, (float)_relativeExplosionPosition.Location.Y);
			dc.RotateTransform((float)_relativeExplosionPosition.Degree);

			float curExplosionRadius = AnimationHelper.AnimateFloat(Phase, _startDiameter, _endDiameter) / 2;
			Color curColor = AnimationHelper.AnimateColor(Phase, Color.FromArgb(0xFF, 0xFF, 0xD7, 0), Color.FromArgb(0, 0, 0, 0), AnimationMode.Pow8);
			dc.FillEllipse(new SolidBrush(curColor), -curExplosionRadius, -curExplosionRadius, curExplosionRadius * 2, curExplosionRadius * 2);
			dc.Restore(oldDc);
		}
	}
}