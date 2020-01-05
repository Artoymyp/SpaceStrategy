using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceStrategy
{
	public struct Point2d
	{
		double _x;
		double _y;
		public Point2d(double x, double y)
		{
			_x = x;
			_y = y;
		}
		public double X { get { return _x; } set { _x = value; } }
		public double Y { get { return _y; } set { _y = value; } }
		public double Ro { get { return _x; } set { _x = value; } }
		public double R { get { return _y; } set { _y = value; } }

		public Vector VectorTo(Point2d targetPoint)
		{
			return new Vector(targetPoint._x - _x, targetPoint._y - _y/*, targetPoint.Z - point.Z*/);
		}
		public Vector ToVector()
		{
			return new Vector(_x, _y);
		}
		public double DistanceSqrTo(Point2d targetPoint)
		{
			double xDist = _x - targetPoint._x;
			double yDist = _y - targetPoint._y;
			//double zDist = point.Z - targetPoint.Z;
			return (xDist * xDist + yDist * yDist/* + zDist * zDist*/);
		}
		public double DistanceTo(Point2d targetPoint)
		{
			return Math.Sqrt(DistanceSqrTo(targetPoint));
		}
		public static bool operator ==(Point2d l, Point2d r)
		{
			return l.ToVector() == r.ToVector();
		}
		public static bool operator !=(Point2d l, Point2d r)
		{
			return !(l == r);
		}
		public static Point2d operator *(Point2d l, double r)
		{
			return new Point2d(l._x * r, l._y * r);
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
			return new Point2d(p._x + v.X, p._y + v.Y);
		}
		public static Point2d operator -(Point2d p, Vector v)
		{
			return new Point2d(p._x - v.X, p._y - v.Y);
		}
		public static implicit operator PointF(Point2d p){
			return new PointF((float)p._x, (float)p._y);
		}
		public Point2d UnTransformBy(Position p)
		{
			Point2d result = new Point2d(_x - p.Location._x, _y - p.Location._y);
			result = new Point2d(
				GeometryHelper.Cos(p.Angle) * result._x + GeometryHelper.Sin(p.Angle) * result._y,
				-GeometryHelper.Sin(p.Angle) * result._x + GeometryHelper.Cos(p.Angle) * result._y
				);
			return result;
		}
		public Point2d TransformBy(Position p)
		{
			Point2d result = new Point2d(
				GeometryHelper.Cos(p.Angle) * _x - GeometryHelper.Sin(p.Angle) * _y,
				GeometryHelper.Sin(p.Angle) * _x + GeometryHelper.Cos(p.Angle) * _y
				);
			result = new Point2d(result._x + p.Location._x, result._y + p.Location._y);
			return result;
		}
		//public override string ToString()
		//{
		//	return string.Format("({0}; {1})", X.ToString(), Y.ToString());
		//}
		/// <summary>
		/// Point2d(Rad, Dist)
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
		public Point2d ToEuclidCs(Point2d origin)
		{
			return new Point2d(GeometryHelper.Cos(Ro) * R + origin.X, GeometryHelper.Sin(Ro) * R + origin.Y);
		}
		public Point2d ToEuclidCs()
		{
			return ToEuclidCs(new Point2d());
		}
		public override string ToString()
		{
			return string.Format("({0}; {1})", _x, _y);
		}
		
	}
}
