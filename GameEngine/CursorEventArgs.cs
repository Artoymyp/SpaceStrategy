using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceStrategy
{
	public class CursorEventArgs:EventArgs
	{
		public Point2d Position { get; private set; }
		public CursorEventArgs(Point2d cursorPosition)
		{
			Position = cursorPosition;
		}
	}
}
