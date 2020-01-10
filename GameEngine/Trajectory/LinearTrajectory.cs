using System.Drawing;

namespace SpaceStrategy
{
	class LinearTrajectory : Trajectory
	{
		internal LinearTrajectory(TrajectoryCollection owner, Point2d startPoint, Point2d endPoint)
			: base(owner, startPoint, endPoint)
		{
			Length = startPoint.DistanceTo(endPoint);
			Direction = startPoint.VectorTo(endPoint).GetNormalized();
			Position = new Position(startPoint, Direction);
		}

		public Vector Direction { get; set; }

		public override void Draw(Graphics dc)
		{
			//throw new NotImplementedException();
		}

		public override Position GetPositionAt(double distance)
		{
			double completedPart = distance / Length;
			var pointAtDistance = new Point2d(
				(EndPoint.X - StartPoint.X) * completedPart + StartPoint.X,
				(EndPoint.Y - StartPoint.Y) * completedPart + StartPoint.Y /*,
			0*/);
			return new Position(pointAtDistance, Direction);
		}
	}
}