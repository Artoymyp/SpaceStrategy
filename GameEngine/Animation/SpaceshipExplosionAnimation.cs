using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceStrategy
{
	internal class RoundExplosionAnimation
	: AnimationObject
	{
		private Position _explosionPosition;
		private float _startDiameter;
		private float _endDiameter;
		internal RoundExplosionAnimation(
			Position explosionPosition,
			TimeSpan explosionDuration,
			float startDiameter,
			float endDiameter) :
			base(AnimationHelper.Game, explosionDuration)
		{
			this._explosionPosition = explosionPosition;
			_startDiameter = startDiameter;
			_endDiameter = endDiameter;
		}
		internal override void Draw(Graphics dc)
		{
			var oldDc = dc.Save();
			dc.TranslateTransform((float)_explosionPosition.Location.X, (float)_explosionPosition.Location.Y);
			if (_explosionPosition.Degree!=0)
				dc.RotateTransform((float)_explosionPosition.Degree);

			float curExplosionRadius = AnimationHelper.AnimateFloat(Phase, _startDiameter, _endDiameter) / 2;
			Color curColor = AnimationHelper.AnimateColor(Phase, Color.FromArgb(0xFF, 0xFF, 0xD7, 0), Color.FromArgb(0, 0, 0, 0), AnimationMode.Pow8);
			dc.FillEllipse(new SolidBrush(curColor), -curExplosionRadius, -curExplosionRadius, curExplosionRadius * 2, curExplosionRadius * 2);
			dc.Restore(oldDc);
		}
		internal override void OnTime(TimeSpan dt)
		{
			base.OnTime(dt);
		}
	}
	internal class SpaceshipRoundExplosionAnimation
		: AnimationObject
	{
		private Spaceship _explodedSpaceship;
		private Position _relativeExplosionPosition;
		private float _startDiameter;
		private float _endDiameter;
		internal SpaceshipRoundExplosionAnimation(
			Spaceship explodedSpaceship, 
			Position relativeExplosionPosition, 
			TimeSpan explosionDuration,
			float startDiameter, 
			float endDiameter):
			base(AnimationHelper.Game,explosionDuration)
		{
			_explodedSpaceship = explodedSpaceship;
			_relativeExplosionPosition = relativeExplosionPosition;
			_startDiameter = startDiameter;
			_endDiameter = endDiameter;
		}
		internal override void Draw(Graphics dc)
		{
			var oldDc = dc.Save();
			dc.TranslateTransform((float)_explodedSpaceship.Position.Location.X, (float)_explodedSpaceship.Position.Location.Y);
			dc.RotateTransform((float)_explodedSpaceship.Position.Degree);
			dc.TranslateTransform((float)_relativeExplosionPosition.Location.X, (float)_relativeExplosionPosition.Location.Y);
			dc.RotateTransform((float)_relativeExplosionPosition.Degree);
			
			float curExplosionRadius = AnimationHelper.AnimateFloat(Phase, _startDiameter, _endDiameter)/2;
			Color curColor = AnimationHelper.AnimateColor(Phase, Color.FromArgb(0xFF, 0xFF, 0xD7, 0), Color.FromArgb(0, 0, 0, 0), AnimationMode.Pow8);
			dc.FillEllipse(new SolidBrush(curColor), -curExplosionRadius, -curExplosionRadius, curExplosionRadius * 2, curExplosionRadius*2);
			dc.Restore(oldDc);
		}
		internal override void OnTime(TimeSpan dt)
		{
			base.OnTime(dt);
		}
	}
}
