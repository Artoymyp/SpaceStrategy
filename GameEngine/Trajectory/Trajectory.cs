using System;

namespace SpaceStrategy
{
	abstract class Trajectory : GraphicObject
	{
		protected TrajectoryCollection Owner;

		protected Trajectory(TrajectoryCollection owner, Point2d startPoint, Point2d endPoint)
		{
			StartPoint = startPoint;
			EndPoint = endPoint;
			Owner = owner;
		}

		internal Point2d EndPoint { get; set; }

		internal Point2d StartPoint { get; set; }

		protected double DistanceFromStart { get; set; }

		protected double Length { get; set; }

		public abstract Position GetPositionAt(double distance);

		internal virtual void AddToCurrentDistance(double distance, out double unusedDistance)
		{
			unusedDistance = distance - (Length - DistanceFromStart);
			if (unusedDistance < 0) {
				unusedDistance = 0;
				DistanceFromStart += distance;
			}
			else {
				DistanceFromStart = Length;
			}
		}

		internal Position GetCurrentPosition()
		{
			return GetPositionAt(DistanceFromStart);
		}

		internal Position GetEndPosition()
		{
			return GetPositionAt(Length);
		}

		internal virtual bool IsOnCourse(Point2d point, double maxDistFromCourse)
		{
			throw new NotImplementedException();
		}

		internal virtual void OnMouseMove(Point2d point) { }
	}
}