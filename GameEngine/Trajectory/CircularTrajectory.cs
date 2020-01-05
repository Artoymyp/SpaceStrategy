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

			this._center = center;
			this._radius = radius;
			this._isClockwise = isClockwise;
			this._startAng = startAng;
			this._endAng = endAng;
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
			double angleAtDistance = (_startAng + distance / _radius);
			Point2d pointAtDistance = GetPointAtAngle(angleAtDistance, _radius, _center);

			return new Position(
				pointAtDistance,
				angleAtDistance + (_isClockwise ? -GeometryHelper.HalfPi : GeometryHelper.HalfPi)
				);
		}
		public override void Draw(Graphics dc)
		{
			throw new NotImplementedException();
		}
		private double _startAng;
		private double _endAng;
		private Point2d _center;
		private double _radius;
		private bool _isClockwise;

	}
}
