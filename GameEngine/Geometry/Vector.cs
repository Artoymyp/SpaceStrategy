using System;

namespace SpaceStrategy
{
	public struct Vector
	{
		public double X;
		public double Y;

		public Vector(double x, double y)
		{
			X = x;
			Y = y;
		}

		public double Length
		{
			get { return Math.Sqrt(X * X + Y * Y); }
		}

		public static Vector operator -(Vector l, Vector r)
		{
			return new Vector(l.X - r.X, l.Y - r.Y);
		}

		public static bool operator !=(Vector left, Vector right)
		{
			return !(left == right);
		}

		public static Vector operator *(Vector l, double r)
		{
			return new Vector(l.X * r, l.Y * r);
		}

		public static Vector operator *(double l, Vector r)
		{
			return r * l;
		}

		public static Vector operator /(Vector l, double r)
		{
			return l * (1 / r);
		}

		public static Vector operator +(Vector l, Vector r)
		{
			return new Vector(l.X + r.X, l.Y + r.Y);
		}

		public static bool operator ==(Vector left, Vector right)
		{
			double precision = 0.0001;
			if (Math.Abs(left.X - right.X) < precision && Math.Abs(left.Y - right.Y) < precision) {
				return true;
			}

			return false;
		}

		public void Normalize()
		{
			var v = new System.Windows.Vector(X, Y);
			v.Normalize();
			X = v.X;
			Y = v.Y;
		}

		public Point2d ToPoint2d()
		{
			return new Point2d(X, Y);
		}

		public override string ToString()
		{
			return string.Format("({0}; {1})", X, Y);
		}
	}
}