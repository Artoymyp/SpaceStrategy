namespace SpaceStrategy
{
	public class Point2dSelectEventArgs
	{
		public Point2dSelectEventArgs(Point2d selectedPoint)
		{
			SelectedPoint = selectedPoint;
		}

		public Point2d SelectedPoint { get; }
	}
}