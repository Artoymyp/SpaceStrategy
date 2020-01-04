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
        protected double distanceFromStart { get; set; }

		internal Point2d StartPoint { get; set; }
		internal Point2d EndPoint { get; set; }
		protected TrajectoryCollection owner;
		protected Trajectory(TrajectoryCollection owner, Point2d startPoint, Point2d endPoint)
		{
			StartPoint = startPoint;
			EndPoint = endPoint;
			this.owner = owner;
		}
		internal virtual void AddToCurrentDistance(double distance, out double unusedDistance)
		{
			unusedDistance = distance - (Length - distanceFromStart);
			if (unusedDistance < 0) {
				unusedDistance = 0;
				distanceFromStart += distance;
			} 
			else {
				distanceFromStart = Length;
			}
		}
		internal Position GetCurrentPosition()
		{
			return GetPositionAt(distanceFromStart);
		}
		public abstract Position GetPositionAt(double distance);
		
		internal Position GetEndPosition()
		{
			return GetPositionAt(Length);
		}

		internal virtual void OnMouseMove(Point2d coord) { }
		internal virtual bool IsOnCourse(Point2d coord, double maxDistFromCourse) { throw new NotImplementedException(); }
	}
}
