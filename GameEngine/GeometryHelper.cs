using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SpaceStrategy
{
	public static class GeometryHelper
	{
		public static double Sin(double radianAngle)
		{
			return Math.Sin(radianAngle);
		}
		public static double Cos(double radianAngle)
		{
			return Math.Cos(radianAngle);
		}

		internal static double Abs(double p)
		{
			return Math.Abs(p);
		}

		public static double DoublePi = 2 * Math.PI;
		public static double Pi = Math.PI;

		public static double HalfPi = Math.PI / 2;
		public static double ToRadian(this Vector vector)
		{
			return Math.Atan2(vector.Y, vector.X);
		}

        public static double PiDiv4 { get { return GeometryHelper.Pi / 4; } }
        public static double PiDiv3 { get { return GeometryHelper.Pi / 3; } }

		public static float Sqrt3Div2 { get { return 0.866F; } }

        internal static double Distance(double fromRad, double toRad, bool isClockwiseDistance)
        {
            double dist = (fromRad - toRad);
            while (dist < 0) { dist += DoublePi; }
            while (dist >= DoublePi) { dist -= DoublePi; }
            return isClockwiseDistance ? dist : -dist;
        }
        internal static bool IsBetween(double fromRad, double toRad, double testedRad)
        {
            fromRad = Modul(fromRad, DoublePi);
            toRad = Modul(toRad, DoublePi);
            testedRad = Modul(testedRad, DoublePi);
            return Distance(fromRad, toRad, true) > Distance(fromRad, testedRad, true) && Math.Abs(Distance(fromRad, testedRad, true))>0.0001;
        }

        internal static double PerpendicularDistance(Point2d Point2d, Point2d startPoint, Point2d endPoint)
        {
            double distance;
            if (TryGetPerpendicular(Point2d, startPoint, endPoint, out distance))
                return distance;
            else return double.PositiveInfinity;
        }

        private static bool TryGetPerpendicular(Point2d Point2d, Point2d startPoint, Point2d endPoint, out double distance)
        {
            Vector segmentVectorGCS = startPoint.VectorTo(endPoint);
            Position pos = new Position(startPoint, segmentVectorGCS);
            Point2d pLCS = Point2d.UnTransformBy(pos);
            Point2d sLCS = startPoint.UnTransformBy(pos);
            Point2d eLCS = endPoint.UnTransformBy(pos);
            if (pLCS.X < sLCS.X || pLCS.X > eLCS.X)
            {
                distance = double.NaN;
                return false;
            }
            else
            {
                distance = Abs(pLCS.Y);
                return true;
            }
        }

        internal static double Modul(double angTemp, double p)
        {
            double curVal = angTemp;
            while (curVal < 0)
            {
                curVal += p;
            }
            return curVal % p;
        }


        internal static int RoundUp(double p)
        {
            return (int)Math.Ceiling(p);
        }

        internal static List<T> GetRandomItems<T>(List<T> items, int count)
        {
            List<T> result = new List<T>();
            List<T> temp = items.ToList();
            for (int i = 0; i < items.Count; i++)
            {
                int index = Game.rand.Next(temp.Count);
                result.Add(items[index]);
                temp.RemoveAt(index);
            }
            return result;
        }
    }
}
