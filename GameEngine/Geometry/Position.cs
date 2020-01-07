namespace SpaceStrategy
{
	public struct Position
	{
		/// <summary>
		///     Radian angle.
		/// </summary>
		public double Angle;

		public Point2d Location;
		Vector _direction;

		public Position(Point2d location)
		{
			Location = location;
			_direction = new Vector(1, 0);
			Angle = 0;
		}

		//public Position(Point location) : this(new Point(location.X, location.Y, 0)) { }
		public Position(Point2d location, Vector direction)
		{
			Location = location;
			_direction = direction;
			Angle = direction.ToRadian();
		}

		public Position(Point2d location, double angle)
		{
			Location = location;
			_direction = new Vector(GeometryHelper.Cos(angle), GeometryHelper.Sin(angle) /*,0*/);
			Angle = angle;
		}

		public double Degree
		{
			get
			{
				double result = Angle * 180 / GeometryHelper.Pi;
				if (double.IsNaN(result)) {
					return 0;
				}

				return result;
			}
		}

		public Vector Direction
		{
			get { return _direction; }
			set
			{
				_direction = value;
				Angle = _direction.ToRadian();
			}
		}

		public static implicit operator Point2d(Position p)
		{
			return new Point2d(p.Location.X, p.Location.Y);
		}

		internal double DistanceTo(Position position)
		{
			return Location.DistanceTo(position.Location);
		}
	}
}