using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceStrategy
{
	internal class CoordConverter
	{
		private Point2d prevMouseCoord;
		private Vector moveVector;
		private Game game;
		internal void SetGameCoordToScreenCenter(Point2d gameCoord)
		{
			Translation = new Vector(game.Size.Width / 2, game.Size.Height / 2) / Scale - gameCoord.ToVector();
		}
		public CoordConverter(Game game, float scale)
		{
			// TODO: Complete member initialization
			this.game = game;
			Scale = scale;
			prevMouseCoord = new Point2d();
			SetGameCoordToScreenCenter(new Point2d(0, 0));
		}
		public Vector Translation { get; private set; }
		public float Scale { get; private set; }
		public void SetTranslation(Vector v)
		{
			Translation = v;
		}
		public void Translate(Vector v)
		{
			Translation = Translation + v/Scale;
		}
		public Point2d GetGameCoord(Point2d winCoord)
		{
			return winCoord / Scale - Translation;
		}
		public Point2d GetWinCoord(Point2d gameCoord)
		{
			return ((Translation + gameCoord.ToVector()) * Scale).ToPoint2d();
		}

		internal void StartFieldDrag(Point2d coord)
		{
			prevMouseCoord = coord;
			moveVector = new Vector();
		}

		internal void FinishFieldDrag(Point2d coord)
		{
			moveVector = prevMouseCoord.VectorTo(coord);
			prevMouseCoord = coord;
			Translate(moveVector);
		}

		public bool IsDraggingField { get { return moveVector == new Vector(); } }

		public Point2d CurWinCoord { get; set; }
		public Point2d CurGameCoord { get { return GetGameCoord(CurWinCoord); } }
		public Point2d LastSelectedShipGameCoord { get; set; }
	}
}
