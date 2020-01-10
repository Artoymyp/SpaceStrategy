using System;

namespace SpaceStrategy
{
	public struct Vector
	{
		public readonly double X;
		public readonly double Y;

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

		public static bool operator !=(Vector left, Vector right)
		{
			return !(left == right);
		}

		public bool Equals(Vector other)
		{
			return X.Equals(other.X) && Y.Equals(other.Y);
		}

		public override bool Equals(object obj)
		{
			return obj is Point2d other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked {
				return (X.GetHashCode() * 397) ^ Y.GetHashCode();
			}
		}

		public Vector GetNormalized()
		{
			var v = new System.Windows.Vector(X, Y);
			v.Normalize();
			return new Vector(v.X, v.Y);
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