using System.Drawing;

namespace SpaceStrategy
{
	class GameCursor
	{
		CursorForm _cursorForm;
		Point2d _location;

		internal GameCursor(Game game)
		{
			Game = game;
		}

		internal Game Game { get; }

		internal void Draw(Graphics dc)
		{
			float size = 10;
			switch (_cursorForm) {
				case CursorForm.TrajectoryOptional:
				case CursorForm.TrajectoryMandatory:
					Color pointColor = Game.Params.SelectedMandatoryTrajectoryColor;
					if (_cursorForm == CursorForm.TrajectoryOptional) {
						pointColor = Game.Params.SelectedTrajectoryColor;
					}

					dc.DrawEllipse(new Pen(pointColor),
						(float)(_location.X - Game.Params.TrajectorycAnchorPointRadius),
						(float)(_location.Y - Game.Params.TrajectorycAnchorPointRadius),
						Game.Params.TrajectorycAnchorPointRadius * 2,
						Game.Params.TrajectorycAnchorPointRadius * 2);
					break;

				case CursorForm.Default:
					var pen = new Pen(Game.Params.SelectedTrajectoryColor, 3);
					float x = (float)_location.X;
					float y = (float)_location.Y;
					dc.DrawLine(pen, x + 0, y - size, x + 0, y + size);
					dc.DrawLine(pen, x - size, y + 0, x + size, y + 0);
					break;

				case CursorForm.Attack:
					var penA = new Pen(Game.Params.SelectedTrajectoryColor, Game.Params.AttackCompassThickness);
					float xA = (float)_location.X;
					float yA = (float)_location.Y;
					dc.DrawLine(penA, xA + 0, yA - size, xA + 0, yA + size);
					dc.DrawLine(penA, xA - size, yA + 0, xA + size, yA + 0);
					int circleMargin = 2;
					dc.DrawEllipse(penA, xA - size + circleMargin, yA - size + circleMargin, size * 2 - circleMargin * 2 - 1, size * 2 - circleMargin * 2 - 1);
					break;
			}
		}

		internal void SetForm(CursorForm cursorForm)
		{
			_cursorForm = cursorForm;
		}

		internal void SetLocation(Point2d location)
		{
			_location = location;
		}
	}
}