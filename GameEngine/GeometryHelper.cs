using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceStrategy
{
	public static class GeometryHelper
	{
		public static double DoublePi = 2 * Math.PI;

		public static double HalfPi = Math.PI / 2;

		public static double Pi = Math.PI;

		public static double PiDiv3
		{
			get { return Pi / 3; }
		}

		public static double PiDiv4
		{
			get { return Pi / 4; }
		}

		public static float Sqrt3Div2
		{
			get { return 0.866F; }
		}

		public static double Cos(double radianAngle)
		{
			return Math.Cos(radianAngle);
		}

		public static double Sin(double radianAngle)
		{
			return Math.Sin(radianAngle);
		}

		public static double ToRadian(this Vector vector)
		{
			return Math.Atan2(vector.Y, vector.X);
		}

		internal static double Abs(double p)
		{
			return Math.Abs(p);
		}

		internal static double Distance(double fromRad, double toRad, bool isClockwiseDistance)
		{
			double dist = fromRad - toRad;
			while (dist < 0) dist += DoublePi;
			while (dist >= DoublePi) dist -= DoublePi;
			return isClockwiseDistance ? dist : -dist;
		}

		internal static List<T> GetRandomItems<T>(this List<T> items, int count)
		{
			var result = new List<T>();
			List<T> temp = items.ToList();
			for (int i = 0; i < items.Count; i++) {
				int index = Game.Rand.Next(temp.Count);
				result.Add(items[index]);
				temp.RemoveAt(index);
			}

			return result;
		}

		internal static bool IsBetween(double fromRad, double toRad, double testedRad)
		{
			fromRad = Modul(fromRad, DoublePi);
			toRad = Modul(toRad, DoublePi);
			testedRad = Modul(testedRad, DoublePi);
			return Distance(fromRad, toRad, true) > Distance(fromRad, testedRad, true) && Math.Abs(Distance(fromRad, testedRad, true)) > 0.0001;
		}

		internal static double Modul(double angTemp, double p)
		{
			double curVal = angTemp;
			while (curVal < 0) curVal += p;
			return curVal % p;
		}

		internal static double PerpendicularDistance(Point2d point, Point2d startPoint, Point2d endPoint)
		{
			if (TryGetPerpendicular(point, startPoint, endPoint, out double distance)) {
				return distance;
			}

			return double.PositiveInfinity;
		}

		internal static int RoundUp(double p)
		{
			return (int)Math.Ceiling(p);
		}

		static bool TryGetPerpendicular(Point2d point, Point2d startPoint, Point2d endPoint, out double distance)
		{
			Vector segmentVectorGcs = startPoint.VectorTo(endPoint);
			var pos = new Position(startPoint, segmentVectorGcs);
			Point2d pLcs = point.UnTransformBy(pos);
			Point2d sLcs = startPoint.UnTransformBy(pos);
			Point2d eLcs = endPoint.UnTransformBy(pos);
			if (pLcs.X < sLcs.X || pLcs.X > eLcs.X) {
				distance = double.NaN;
				return false;
			}

			distance = Abs(pLcs.Y);
			return true;
		}
	}
}