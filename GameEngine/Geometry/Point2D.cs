using System;
using System.Drawing;

namespace SpaceStrategy
{
	public struct Point2d
	{
		public Point2d(double x, double y)
		{
			Ro = x;
			R = y;
		}

		public double R { get; }

		public double Ro { get; }

		public double X
		{
			get { return Ro; }
		}

		public double Y
		{
			get { return R; }
		}

		public static implicit operator PointF(Point2d p)
		{
			return new PointF((float)p.Ro, (float)p.R);
		}

		public static Point2d operator -(Point2d p, Vector v)
		{
			return new Point2d(p.Ro - v.X, p.R - v.Y);
		}

		public static Point2d operator *(Point2d l, double r)
		{
			return new Point2d(l.Ro * r, l.R * r);
		}

		public static Point2d operator *(double l, Point2d r)
		{
			return r * l;
		}

		public static Point2d operator /(Point2d l, double r)
		{
			return l * (1 / r);
		}

		public static Point2d operator +(Point2d p, Vector v)
		{
			return new Point2d(p.Ro + v.X, p.R + v.Y);
		}

		public static bool operator ==(Point2d l, Point2d r)
		{
			return l.ToVector() == r.ToVector();
		}

		public static bool operator !=(Point2d l, Point2d r)
		{
			return !(l == r);
		}

		public bool Equals(Point2d other)
		{
			return R.Equals(other.R) && Ro.Equals(other.Ro);
		}

		public override bool Equals(object obj)
		{
			return obj is Point2d other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked {
				return (R.GetHashCode() * 397) ^ Ro.GetHashCode();
			}
		}

		public double DistanceSqrTo(Point2d targetPoint)
		{
			double xDist = Ro - targetPoint.Ro;
			double yDist = R - targetPoint.R;

			//double zDist = point.Z - targetPoint.Z;
			return xDist * xDist + yDist * yDist;
		}

		public double DistanceTo(Point2d targetPoint)
		{
			return Math.Sqrt(DistanceSqrTo(targetPoint));
		}

		public Point2d ToEuclidCs(Point2d origin)
		{
			return new Point2d(GeometryHelper.Cos(Ro) * R + origin.X, GeometryHelper.Sin(Ro) * R + origin.Y);
		}

		public Point2d ToEuclidCs()
		{
			return ToEuclidCs(new Point2d());
		}

		//public override string ToString()
		//{
		//	return string.Format("({0}; {1})", X.ToString(), Y.ToString());
		//}
		/// <summary>
		///     Point2d(Rad, Dist)
		/// </summary>
		/// <param name="origin"></param>
		/// <returns></returns>
		public Point2d ToPolarCs(Point2d origin)
		{
			double distance = DistanceTo(origin);
			double ang = origin.VectorTo(this).ToRadian();
			return new Point2d(ang, distance);
		}

		public Point2d ToPolarCs()
		{
			return ToPolarCs(new Point2d());
		}

		public override string ToString()
		{
			return string.Format("({0}; {1})", Ro, R);
		}

		public Vector ToVector()
		{
			return new Vector(Ro, R);
		}

		public Point2d TransformBy(Position p)
		{
			var result = new Point2d(
				GeometryHelper.Cos(p.Angle) * Ro - GeometryHelper.Sin(p.Angle) * R,
				GeometryHelper.Sin(p.Angle) * Ro + GeometryHelper.Cos(p.Angle) * R
			);
			result = new Point2d(result.Ro + p.Location.Ro, result.R + p.Location.R);
			return result;
		}

		public Point2d UnTransformBy(Position p)
		{
			var result = new Point2d(Ro - p.Location.Ro, R - p.Location.R);
			result = new Point2d(
				GeometryHelper.Cos(p.Angle) * result.Ro + GeometryHelper.Sin(p.Angle) * result.R,
				-GeometryHelper.Sin(p.Angle) * result.Ro + GeometryHelper.Cos(p.Angle) * result.R
			);
			return result;
		}

		public Vector VectorTo(Point2d targetPoint)
		{
			return new Vector(targetPoint.Ro - Ro, targetPoint.R - R /*, targetPoint.Z - point.Z*/);
		}
	}
}