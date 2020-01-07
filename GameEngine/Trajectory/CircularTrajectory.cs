using System;
using System.Drawing;

namespace SpaceStrategy
{
	class CircularTrajectory : Trajectory
	{
		readonly Point2d _center;

		readonly bool _isClockwise;

		readonly double _radius;

		readonly double _startAng;

		double _endAng;

		internal CircularTrajectory(TrajectoryCollection owner, double startAng, double endAng, Point2d center, double radius, bool isClockwise)
			: base(owner, GetPointAtAngle(startAng, radius, center), GetPointAtAngle(endAng, radius, center))
		{
			//StartPoint = GetPointAtAngle(startAng);
			//EndPoint = GetPointAtAngle(endAng);

			if (endAng < startAng) {
				endAng += GeometryHelper.DoublePi;
			}

			double andDiff = endAng - startAng;
			if (!isClockwise) {
				Length = andDiff;
			}
			else {
				Length = GeometryHelper.DoublePi - andDiff;
			}

			_center = center;
			_radius = radius;
			_isClockwise = isClockwise;
			_startAng = startAng;
			_endAng = endAng;
		}

		public override void Draw(Graphics dc)
		{
			throw new NotImplementedException();
		}

		public override Position GetPositionAt(double distance)
		{
			double angleAtDistance = _startAng + distance / _radius;
			Point2d pointAtDistance = GetPointAtAngle(angleAtDistance, _radius, _center);

			return new Position(
				pointAtDistance,
				angleAtDistance + (_isClockwise ? -GeometryHelper.HalfPi : GeometryHelper.HalfPi)
			);
		}

		static Point2d GetPointAtAngle(double angle, double radius, Point2d center)
		{
			return new Point2d(
				GeometryHelper.Cos(angle) * radius + center.X,
				GeometryHelper.Sin(angle) * radius + center.Y /*,
				0*/);
		}
	}
}