using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceStrategy
{
	internal class GameCursor
	{
		private Point2d _location;
		private CursorForm _cursorForm;
		internal Game Game { get; private set; }
		internal GameCursor(Game game)
		{
			this.Game = game;
		}
		internal void SetLocation(Point2d location)
		{
			_location = location;
		}
		internal void SetForm(CursorForm cursorForm)
		{
			_cursorForm = cursorForm;
		}
		internal void Draw(Graphics dc)
		{
			float size = 10;
			switch (_cursorForm) {
				case CursorForm.TrajectoryOptional:
				case CursorForm.TrajectoryMandatory:
					var pointColor = Game.Params.SelectedMandatoryTrajectoryColor;
					if (_cursorForm == CursorForm.TrajectoryOptional)
						pointColor = Game.Params.SelectedTrajectoryColor;
					dc.DrawEllipse(new Pen(pointColor),
					(float)(_location.X - Game.Params.TrajectorycAnchorPointRadius),
					(float)(_location.Y - Game.Params.TrajectorycAnchorPointRadius),
					Game.Params.TrajectorycAnchorPointRadius * 2,
					Game.Params.TrajectorycAnchorPointRadius * 2);
					break;
				case CursorForm.Default:
					Pen pen = new Pen(Game.Params.SelectedTrajectoryColor, 3);
					float x = (float)_location.X;
					float y = (float)_location.Y;
					dc.DrawLine(pen, x + 0, y - size, x + 0, y + size);
					dc.DrawLine(pen, x - size, y + 0, x + size, y + 0);
					break;
				case CursorForm.Attack:
					Pen penA = new Pen(Game.Params.SelectedTrajectoryColor, Game.Params.AttackCompassThickness);
					float xA = (float)_location.X;
					float yA = (float)_location.Y;
					dc.DrawLine(penA, xA + 0, yA - size, xA + 0, yA + size);
					dc.DrawLine(penA, xA - size, yA + 0, xA + size, yA + 0);
					int circleMargin = 2;
					dc.DrawEllipse(penA, xA - size + circleMargin, yA - size + circleMargin, size * 2 - circleMargin * 2 - 1, size * 2 - circleMargin * 2 - 1);
					break;
				default:
					break;
			}
		}
	}
	internal enum CursorForm
	{
		TrajectoryOptional,
		TrajectoryMandatory,
		Default,
		Attack
	}

}
