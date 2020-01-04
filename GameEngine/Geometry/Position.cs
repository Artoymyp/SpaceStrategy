using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceStrategy
{
	public struct Position
	{
		public Point2d Location;
		private Vector direction;
		public Vector Direction
		{
			get { return direction;}
			set {
			direction = value;
			Angle = direction.ToRadian();
		} }
		/// <summary>
		/// Radian angle.
		/// </summary>
		public double Angle;
		public double Degree
		{
			get
			{
				double result = Angle * 180 / GeometryHelper.Pi;
				if (double.IsNaN(result))
					return 0;
				else
					return result;

			}
		}
		public Position(Point2d location){
			Location = location;
			direction = new Vector(1,0);
			Angle = 0;
		}
		public static implicit operator Point2d(Position p) { return new Point2d(p.Location.X, p.Location.Y); }
		//public Position(Point location) : this(new Point(location.X, location.Y, 0)) { }
		public Position(Point2d location, Vector direction)
		{
			Location = location;
			this.direction = direction;
			Angle = direction.ToRadian();
		}
		public Position(Point2d location, double angle)
		{
			Location = location;
			direction = new Vector(GeometryHelper.Cos(angle),GeometryHelper.Sin(angle)/*,0*/);
			Angle = angle;
		}

		internal double DistanceTo(Position position)
		{
			return Location.DistanceTo(position.Location);
		}
	}
}
