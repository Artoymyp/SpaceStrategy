using System;

namespace SpaceStrategy
{
	public class CursorEventArgs : EventArgs
	{
		public CursorEventArgs(Point2d cursorPosition)
		{
			Position = cursorPosition;
		}

		public Point2d Position { get; }
	}
}