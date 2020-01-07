namespace SpaceStrategy
{
	class CoordinateConverter
	{
		readonly Game _game;
		Vector _moveVector;
		Point2d _prevMouseCoord;

		public CoordinateConverter(Game game, float scale)
		{
			// TODO: Complete member initialization
			_game = game;
			Scale = scale;
			_prevMouseCoord = new Point2d();
			SetGameCoordToScreenCenter(new Point2d(0, 0));
		}

		public Point2d CurGameCoord
		{
			get { return GetGameCoord(CurWinCoord); }
		}

		public Point2d CurWinCoord { get; set; }

		public bool IsDraggingField
		{
			get { return _moveVector == new Vector(); }
		}

		public Point2d LastSelectedShipGameCoord { get; set; }

		public float Scale { get; }

		public Vector Translation { get; private set; }

		public Point2d GetGameCoord(Point2d winCoord)
		{
			return winCoord / Scale - Translation;
		}

		public Point2d GetWinCoord(Point2d gameCoord)
		{
			return ((Translation + gameCoord.ToVector()) * Scale).ToPoint2d();
		}

		public void SetTranslation(Vector v)
		{
			Translation = v;
		}

		public void Translate(Vector v)
		{
			Translation = Translation + v / Scale;
		}

		internal void FinishFieldDrag(Point2d coord)
		{
			_moveVector = _prevMouseCoord.VectorTo(coord);
			_prevMouseCoord = coord;
			Translate(_moveVector);
		}

		internal void SetGameCoordToScreenCenter(Point2d gameCoord)
		{
			Translation = new Vector(_game.Size.Width / 2, _game.Size.Height / 2) / Scale - gameCoord.ToVector();
		}

		internal void StartFieldDrag(Point2d coord)
		{
			_prevMouseCoord = coord;
			_moveVector = new Vector();
		}
	}
}