namespace SpaceStrategy
{
	class CoordinateConverter
	{
		readonly Game _game;
		Vector _moveVector;
		Point2d _prevMousePoint;

		public CoordinateConverter(Game game, float scale)
		{
			// TODO: Complete member initialization
			_game = game;
			Scale = scale;
			_prevMousePoint = new Point2d();
			SetGameCsPointToScreenCenter(new Point2d(0, 0));
		}

		public Point2d CurGamePoint
		{
			get { return GetGameCsPoint(CurWinPoint); }
		}

		public Point2d CurWinPoint { get; set; }

		public bool IsDraggingField
		{
			get { return _moveVector == new Vector(); }
		}

		public Point2d LastSelectedShipGameCsPoint { get; set; }

		public float Scale { get; }

		public Vector Translation { get; private set; }

		public Point2d GetGameCsPoint(Point2d winCsPoint)
		{
			return winCsPoint / Scale - Translation;
		}

		public Point2d GetWinCsPoint(Point2d gameCsPoint)
		{
			return ((Translation + gameCsPoint.ToVector()) * Scale).ToPoint2d();
		}

		public void SetTranslation(Vector v)
		{
			Translation = v;
		}

		public void Translate(Vector v)
		{
			Translation = Translation + v / Scale;
		}

		internal void FinishFieldDrag(Point2d point)
		{
			_moveVector = _prevMousePoint.VectorTo(point);
			_prevMousePoint = point;
			Translate(_moveVector);
		}

		internal void SetGameCsPointToScreenCenter(Point2d gameCsPoint)
		{
			Translation = new Vector(_game.Size.Width / 2, _game.Size.Height / 2) / Scale - gameCsPoint.ToVector();
		}

		internal void StartFieldDrag(Point2d point)
		{
			_prevMousePoint = point;
			_moveVector = new Vector();
		}
	}
}