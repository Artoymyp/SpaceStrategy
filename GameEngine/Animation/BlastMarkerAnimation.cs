using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceStrategy
{
	internal class BlastMarkerAnimation: AnimationObject
	{
		Position _position;
		internal BlastMarkerAnimation(Position position):base(AnimationHelper.Game, new TimeSpan(), true)
		{
			_position = position;
		}
			
		internal override void Draw(System.Drawing.Graphics dc)
		{
			var oldDc = dc.Save();
			dc.TranslateTransform((float)_position.Location.X, (float)_position.Location.Y);
			dc.RotateTransform((float)_position.Degree);

			Color epicenterColor = Color.Gold;
			Color perimeterColor = Color.FromArgb(128, Color.Orange);
			float elementDiameter = Game.Params.BlastMarkerDiameter/2;
			var path = new GraphicsPath();
			path.AddEllipse(-elementDiameter/2,-elementDiameter/2,elementDiameter,elementDiameter);
			PathGradientBrush epicenterBrush = new PathGradientBrush(path);
			epicenterBrush.CenterColor = epicenterColor;
			epicenterBrush.SurroundColors =new Color[]{ perimeterColor };
			epicenterBrush.FocusScales= new PointF(0.2F,0.2F);
			Brush perimeterBrush = new SolidBrush(perimeterColor);

			float yCoord = elementDiameter / 2 * (GeometryHelper.Sqrt3Div2 + 1);
			dc.FillEllipse(perimeterBrush, -elementDiameter, -elementDiameter/2, elementDiameter, elementDiameter);
			dc.FillEllipse(perimeterBrush, 0, -yCoord, elementDiameter, elementDiameter);
			dc.FillEllipse(perimeterBrush, 0, yCoord - elementDiameter, elementDiameter, elementDiameter);
			dc.FillEllipse(epicenterBrush, -elementDiameter / 2, -elementDiameter / 2, elementDiameter, elementDiameter);

			dc.Restore(oldDc);
		}
	}
}
