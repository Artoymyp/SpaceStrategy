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
		double m_X;
		double m_Y;
		public Point2d(double x, double y)
		{
			m_X = x;
			m_Y = y;
		}
		public double X { get { return m_X; } set { m_X = value; } }
		public double Y { get { return m_Y; } set { m_Y = value; } }
		public double Ro { get { return m_X; } set { m_X = value; } }
		public double R { get { return m_Y; } set { m_Y = value; } }

		public Vector VectorTo(Point2d targetPoint)
		{
			return new Vector(targetPoint.m_X - m_X, targetPoint.m_Y - m_Y/*, targetPoint.Z - point.Z*/);
		}
		public Vector ToVector()
		{
			return new Vector(m_X, m_Y);
		}
		public double DistanceSqrTo(Point2d targetPoint)
		{
			double xDist = m_X - targetPoint.m_X;
			double yDist = m_Y - targetPoint.m_Y;
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
			return new Point2d(l.m_X * r, l.m_Y * r);
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
			return new Point2d(p.m_X + v.X, p.m_Y + v.Y);
		}
		public static Point2d operator -(Point2d p, Vector v)
		{
			return new Point2d(p.m_X - v.X, p.m_Y - v.Y);
		}
		public static implicit operator PointF(Point2d p){
			return new PointF((float)p.m_X, (float)p.m_Y);
		}
		public Point2d UnTransformBy(Position p)
		{
			Point2d result = new Point2d(m_X - p.Location.m_X, m_Y - p.Location.m_Y);
			result = new Point2d(
				GeometryHelper.Cos(p.Angle) * result.m_X + GeometryHelper.Sin(p.Angle) * result.m_Y,
				-GeometryHelper.Sin(p.Angle) * result.m_X + GeometryHelper.Cos(p.Angle) * result.m_Y
				);
			return result;
		}
		public Point2d TransformBy(Position p)
		{
			Point2d result = new Point2d(
				GeometryHelper.Cos(p.Angle) * m_X - GeometryHelper.Sin(p.Angle) * m_Y,
				GeometryHelper.Sin(p.Angle) * m_X + GeometryHelper.Cos(p.Angle) * m_Y
				);
			result = new Point2d(result.m_X + p.Location.m_X, result.m_Y + p.Location.m_Y);
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
		public Point2d ToPolarCS(Point2d origin)
		{
			double distance = DistanceTo(origin);
			double ang = origin.VectorTo(this).ToRadian();
			return new Point2d(ang, distance);
		}
		public Point2d ToPolarCS()
		{
			return ToPolarCS(new Point2d());
		}
		public Point2d ToEuclidCS(Point2d origin)
		{
			return new Point2d(GeometryHelper.Cos(Ro) * R + origin.X, GeometryHelper.Sin(Ro) * R + origin.Y);
		}
		public Point2d ToEuclidCS()
		{
			return ToEuclidCS(new Point2d());
		}
		public override string ToString()
		{
			return string.Format("({0}; {1})", m_X, m_Y);
		}
		
	}
}
