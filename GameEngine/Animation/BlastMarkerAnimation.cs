using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SpaceStrategy
{
	class BlastMarkerAnimation : AnimationObject
	{
		Position _position;

		internal BlastMarkerAnimation(Position position) : base(AnimationHelper.Game, new TimeSpan(), true)
		{
			_position = position;
		}

		public override void Draw(Graphics dc)
		{
			GraphicsState oldDc = dc.Save();
			dc.TranslateTransform((float)_position.Location.X, (float)_position.Location.Y);
			dc.RotateTransform((float)_position.Degree);

			Color epicenterColor = Color.Gold;
			Color perimeterColor = Color.FromArgb(128, Color.Orange);
			float elementDiameter = Game.Params.BlastMarkerDiameter / 2;
			var path = new GraphicsPath();
			path.AddEllipse(-elementDiameter / 2, -elementDiameter / 2, elementDiameter, elementDiameter);
			var epicenterBrush = new PathGradientBrush(path)
			{
				CenterColor = epicenterColor,
				SurroundColors = new[] {perimeterColor},
				FocusScales = new PointF(0.2F, 0.2F)
			};
			Brush perimeterBrush = new SolidBrush(perimeterColor);

			float y = elementDiameter / 2 * (GeometryHelper.Sqrt3Div2 + 1);
			dc.FillEllipse(perimeterBrush, -elementDiameter, -elementDiameter / 2, elementDiameter, elementDiameter);
			dc.FillEllipse(perimeterBrush, 0, -y, elementDiameter, elementDiameter);
			dc.FillEllipse(perimeterBrush, 0, y - elementDiameter, elementDiameter, elementDiameter);
			dc.FillEllipse(epicenterBrush, -elementDiameter / 2, -elementDiameter / 2, elementDiameter, elementDiameter);

			dc.Restore(oldDc);
		}
	}
}