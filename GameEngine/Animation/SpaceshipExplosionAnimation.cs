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
        private Position explosionPosition;
        private float StartDiameter;
        private float EndDiameter;
        internal RoundExplosionAnimation(
            Position explosionPosition,
            TimeSpan explosionDuration,
            float startDiameter,
            float endDiameter) :
            base(AnimationHelper.Game, explosionDuration)
        {
            this.explosionPosition = explosionPosition;
            StartDiameter = startDiameter;
            EndDiameter = endDiameter;
        }
        internal override void Draw(Graphics dc)
        {
            var oldDc = dc.Save();
            dc.TranslateTransform((float)explosionPosition.Location.X, (float)explosionPosition.Location.Y);
            if (explosionPosition.Degree!=0)
                dc.RotateTransform((float)explosionPosition.Degree);

            float curExplosionRadius = AnimationHelper.AnimateFloat(Phase, StartDiameter, EndDiameter) / 2;
            Color curColor = AnimationHelper.AnimateColor(Phase, Color.FromArgb(0xFF, 0xFF, 0xD7, 0), Color.FromArgb(0, 0, 0, 0), AnimationMode.x8);
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
		private Spaceship ExplodedSpaceship;
		private Position RelativeExplosionPosition;
		private float StartDiameter;
		private float EndDiameter;
		internal SpaceshipRoundExplosionAnimation(
			Spaceship explodedSpaceship, 
			Position relativeExplosionPosition, 
			TimeSpan explosionDuration,
			float startDiameter, 
			float endDiameter):
			base(AnimationHelper.Game,explosionDuration)
		{
			ExplodedSpaceship = explodedSpaceship;
			RelativeExplosionPosition = relativeExplosionPosition;
			StartDiameter = startDiameter;
			EndDiameter = endDiameter;
		}
		internal override void Draw(Graphics dc)
		{
			var oldDc = dc.Save();
			dc.TranslateTransform((float)ExplodedSpaceship.Position.Location.X, (float)ExplodedSpaceship.Position.Location.Y);
			dc.RotateTransform((float)ExplodedSpaceship.Position.Degree);
			dc.TranslateTransform((float)RelativeExplosionPosition.Location.X, (float)RelativeExplosionPosition.Location.Y);
			dc.RotateTransform((float)RelativeExplosionPosition.Degree);
			
			float curExplosionRadius = AnimationHelper.AnimateFloat(Phase, StartDiameter, EndDiameter)/2;
			Color curColor = AnimationHelper.AnimateColor(Phase, Color.FromArgb(0xFF, 0xFF, 0xD7, 0), Color.FromArgb(0, 0, 0, 0), AnimationMode.x8);
			dc.FillEllipse(new SolidBrush(curColor), -curExplosionRadius, -curExplosionRadius, curExplosionRadius * 2, curExplosionRadius*2);
			dc.Restore(oldDc);
		}
		internal override void OnTime(TimeSpan dt)
		{
			base.OnTime(dt);
		}
	}
}
