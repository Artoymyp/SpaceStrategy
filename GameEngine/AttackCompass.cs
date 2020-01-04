using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceStrategy
{
	internal class AttackCompass
	{
		internal AttackCompass(Game game)
		{
			Game = game;
		}
		internal Game Game { get; private set; }
		internal void Draw(Graphics dc, Color color, Position pos, Side side, float radius)
		{
			var dcState = dc.Save();
			dc.TranslateTransform((float)pos.Location.X, (float)pos.Location.Y);
			dc.RotateTransform((float)pos.Degree);

			Pen pen = new Pen(color, Game.Params.AttackCompassThickness);
			pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
			var x45degree = GeometryHelper.Cos(GeometryHelper.Pi / 4) * radius;

			var centerPoint = new Point2d(0, 0);
			var cornerPointDR = new Point2d(x45degree, x45degree);
			var cornerPointTR = new Point2d(x45degree, -x45degree);
			var cornerPointDL = new Point2d(-x45degree, x45degree);
			var cornerPointTL = new Point2d(-x45degree, -x45degree);
			switch (side) {
				case Side.Left:
					dc.DrawLine(pen, cornerPointTL, centerPoint);
					dc.DrawLine(pen, cornerPointTR, centerPoint);
					dc.DrawArc(pen, -radius, -radius, 2 * radius, 2 * radius, -135, 90);
					break;
				case Side.Front:
					dc.DrawLine(pen, cornerPointTR, centerPoint);
					dc.DrawLine(pen, cornerPointDR, centerPoint);
					dc.DrawArc(pen, -radius, -radius, 2 * radius, 2 * radius, -45, 90);
					break;
				case Side.Right:
					dc.DrawLine(pen, cornerPointDR, centerPoint);
					dc.DrawLine(pen, cornerPointDL, centerPoint);
					dc.DrawArc(pen, -radius, -radius, 2 * radius, 2 * radius, 45, 90);
					break;
				case Side.LFR:
					dc.Restore(dcState);
					Draw(dc, color, pos, Side.Left, radius);
					Draw(dc, color, pos, Side.Front, radius);
					Draw(dc, color, pos, Side.Right, radius);
					break;
				default:
					break;
			}
			dc.Restore(dcState);
		}
	}
	
}
