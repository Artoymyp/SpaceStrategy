using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceStrategy
{
	public class Point2dSelectEventArgs
	{
		public Point2d SelectedPoint { get; private set; }
		public Point2dSelectEventArgs(Point2d selectedPoint)
		{
			SelectedPoint = selectedPoint;
		}
	}
}
