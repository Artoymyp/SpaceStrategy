using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceStrategy.Animation
{
	internal class ShiledActiveAnimation: AnimationObject
	{
		static TimeSpan dur { get { return new TimeSpan(0,0,0,0,1000);} }
		GothicSpaceshipBase spaceship;
		static Color ShieldColorMax { get { return Color.FromArgb(0x50, 0xFF, 0xFF, 0xA0); } }
		static Color ShieldColorMin { get { return Color.FromArgb(0x20, 0xFF, 0xFF, 0xA0); } }
		static float SizeFactor { get { return 1.2F; } }
		public ShiledActiveAnimation(GothicSpaceshipBase spaceship, bool cyclic):base(spaceship.Game, dur, cyclic)
		{
			this.spaceship = spaceship;
		}
		internal override void Draw(System.Drawing.Graphics dc)
		{
			var oldDc = dc.Save();
			dc.TranslateTransform((float)spaceship.Position.Location.X, (float)spaceship.Position.Location.Y);
			Color curColor;
			if (Phase < 0.5)
				curColor = AnimationHelper.AnimateColor(Phase * 2, ShieldColorMin, ShieldColorMax);
			else
				curColor = AnimationHelper.AnimateColor((Phase - 0.5) * 2, ShieldColorMax, ShieldColorMin);

			float radius = spaceship.Diameter * 0.5F * SizeFactor;
			RectangleF rect = new RectangleF(-radius, -radius, radius * 2, radius * 2);

			Brush pthGrBrush = new SolidBrush(curColor);

			dc.FillEllipse(pthGrBrush, rect);

			//float bubbleRadius = radius / 3;
			//int bubbleCount = 8;
			//float bubbleAngleStep = (float)GeometryHelper.DoublePi / bubbleCount;
			//Point2d curBubblePos = new Point2d(radius, 0);
			//for (int i = 0; i < bubbleCount; i++)
			//{
			//	RectangleF bubbleRect = new RectangleF((float)curBubblePos.X-bubbleRadius, (float)curBubblePos.Y-bubbleRadius, bubbleRadius*2, bubbleRadius*2);
			//	dc.FillEllipse(pthGrBrush, bubbleRect);
			//	Point2d prevBubblePolarPos = curBubblePos.ToPolarCS();
			//	curBubblePos = new Point2d(prevBubblePolarPos.Ro + bubbleAngleStep, prevBubblePolarPos.R).ToEuclidCS();
			//}
			dc.Restore(oldDc);
		}
	}
}
