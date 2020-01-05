using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceStrategy
{
	internal class CoordinateConverter
	{
		private Point2d _prevMouseCoord;
		private Vector _moveVector;
		private Game _game;
		internal void SetGameCoordToScreenCenter(Point2d gameCoord)
		{
			Translation = new Vector(_game.Size.Width / 2, _game.Size.Height / 2) / Scale - gameCoord.ToVector();
		}
		public CoordinateConverter(Game game, float scale)
		{
			// TODO: Complete member initialization
			this._game = game;
			Scale = scale;
			_prevMouseCoord = new Point2d();
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
			_prevMouseCoord = coord;
			_moveVector = new Vector();
		}

		internal void FinishFieldDrag(Point2d coord)
		{
			_moveVector = _prevMouseCoord.VectorTo(coord);
			_prevMouseCoord = coord;
			Translate(_moveVector);
		}

		public bool IsDraggingField { get { return _moveVector == new Vector(); } }

		public Point2d CurWinCoord { get; set; }
		public Point2d CurGameCoord { get { return GetGameCoord(CurWinCoord); } }
		public Point2d LastSelectedShipGameCoord { get; set; }
	}
}
