using System.Drawing;
using System.Drawing.Drawing2D;

namespace SpaceStrategy
{
	class AttackCompass
	{
		internal AttackCompass(Game game)
		{
			Game = game;
		}

		internal Game Game { get; }

		internal void Draw(Graphics dc, Color color, Position pos, Side side, float radius)
		{
			GraphicsState dcState = dc.Save();
			dc.TranslateTransform((float)pos.Location.X, (float)pos.Location.Y);
			dc.RotateTransform((float)pos.Degree);

			var pen = new Pen(color, Game.Params.AttackCompassThickness);
			pen.DashStyle = DashStyle.Dash;
			double x45degree = GeometryHelper.Cos(GeometryHelper.Pi / 4) * radius;

			var centerPoint = new Point2d(0, 0);
			var bottomRightPoint = new Point2d(x45degree, x45degree);
			var topRightPoint = new Point2d(x45degree, -x45degree);
			var bottomLeftPoint = new Point2d(-x45degree, x45degree);
			var topLeftPoint = new Point2d(-x45degree, -x45degree);
			switch (side) {
				case Side.Left:
					dc.DrawLine(pen, topLeftPoint, centerPoint);
					dc.DrawLine(pen, topRightPoint, centerPoint);
					dc.DrawArc(pen, -radius, -radius, 2 * radius, 2 * radius, -135, 90);
					break;

				case Side.Front:
					dc.DrawLine(pen, topRightPoint, centerPoint);
					dc.DrawLine(pen, bottomRightPoint, centerPoint);
					dc.DrawArc(pen, -radius, -radius, 2 * radius, 2 * radius, -45, 90);
					break;

				case Side.Right:
					dc.DrawLine(pen, bottomRightPoint, centerPoint);
					dc.DrawLine(pen, bottomLeftPoint, centerPoint);
					dc.DrawArc(pen, -radius, -radius, 2 * radius, 2 * radius, 45, 90);
					break;

				case Side.LeftFrontRight:
					dc.Restore(dcState);
					Draw(dc, color, pos, Side.Left, radius);
					Draw(dc, color, pos, Side.Front, radius);
					Draw(dc, color, pos, Side.Right, radius);
					break;
			}

			dc.Restore(dcState);
		}
	}
}