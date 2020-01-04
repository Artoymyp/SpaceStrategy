using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceStrategy
{
	internal class CircularTrajectory : Trajectory
	{
		internal CircularTrajectory(TrajectoryCollection owner, double startAng, double endAng, Point2d center, double radius, bool isClockwise)
			: base(owner, GetPointAtAngle(startAng, radius, center), GetPointAtAngle(endAng, radius, center))
		{
			//StartPoint = GetPointAtAngle(startAng);
			//EndPoint = GetPointAtAngle(endAng);

			if (endAng < startAng) endAng += GeometryHelper.DoublePi;
			double andDiff = endAng - startAng;
			if (!isClockwise) Length = andDiff;
			else Length = GeometryHelper.DoublePi - andDiff;

			this.center = center;
			this.radius = radius;
			this.isClockwise = isClockwise;
			this.startAng = startAng;
			this.endAng = endAng;
		}

		private static Point2d GetPointAtAngle(double angle, double radius, Point2d center)
		{
			return new Point2d(
				GeometryHelper.Cos(angle) * radius + center.X,
				GeometryHelper.Sin(angle) * radius + center.Y/*,
				0*/);
		}
		public override Position GetPositionAt(double distance)
		{
			double angleAtDistance = (startAng + distance / radius);
			Point2d pointAtDistance = GetPointAtAngle(angleAtDistance, radius, center);

			return new Position(
				pointAtDistance,
				angleAtDistance + (isClockwise ? -GeometryHelper.HalfPi : GeometryHelper.HalfPi)
				);
		}
		public override void Draw(Graphics dc)
		{
			throw new NotImplementedException();
		}
		private double startAng;
		private double endAng;
		private Point2d center;
		private double radius;
		private bool isClockwise;

	}
}
