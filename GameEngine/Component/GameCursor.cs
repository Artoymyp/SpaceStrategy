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
		private Point2d Location;
		private CursorForm Form;
		internal Game Game { get; private set; }
		internal GameCursor(Game game)
		{
			this.Game = game;
		}
		internal void SetLocation(Point2d location)
		{
			Location = location;
		}
		internal void SetForm(CursorForm form)
		{
			Form = form;
		}
		internal void Draw(Graphics dc)
		{
			float size = 10;
			switch (Form) {
				case CursorForm.TrajectoryOptional:
				case CursorForm.TrajectoryMandatory:
					var pointColor = Game.Params.SelectedMandatoryTrajectoryColor;
					if (Form == CursorForm.TrajectoryOptional)
						pointColor = Game.Params.SelectedTrajectoryColor;
					dc.DrawEllipse(new Pen(pointColor),
					(float)(Location.X - Game.Params.TrajectorycAnchorPointRadius),
					(float)(Location.Y - Game.Params.TrajectorycAnchorPointRadius),
					Game.Params.TrajectorycAnchorPointRadius * 2,
					Game.Params.TrajectorycAnchorPointRadius * 2);
					break;
				case CursorForm.Default:
					Pen pen = new Pen(Game.Params.SelectedTrajectoryColor, 3);
					float x = (float)Location.X;
					float y = (float)Location.Y;
					dc.DrawLine(pen, x + 0, y - size, x + 0, y + size);
					dc.DrawLine(pen, x - size, y + 0, x + size, y + 0);
					break;
				case CursorForm.Attack:
					Pen penA = new Pen(Game.Params.SelectedTrajectoryColor, Game.Params.AttackCompassThickness);
					float xA = (float)Location.X;
					float yA = (float)Location.Y;
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
