using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SpaceStrategy
{
	internal abstract class Trajectory : GraphicObject
	{
		protected double Length { get; set; }
		protected double DistanceFromStart { get; set; }

		internal Point2d StartPoint { get; set; }
		internal Point2d EndPoint { get; set; }
		protected TrajectoryCollection Owner;
		protected Trajectory(TrajectoryCollection owner, Point2d startPoint, Point2d endPoint)
		{
			StartPoint = startPoint;
			EndPoint = endPoint;
			this.Owner = owner;
		}
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
		public abstract Position GetPositionAt(double distance);
		
		internal Position GetEndPosition()
		{
			return GetPositionAt(Length);
		}

		internal virtual void OnMouseMove(Point2d point) { }
		internal virtual bool IsOnCourse(Point2d point, double maxDistFromCourse) { throw new NotImplementedException(); }
	}
}
