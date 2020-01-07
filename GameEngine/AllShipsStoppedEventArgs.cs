using System;

namespace SpaceStrategy
{
	public class AllShipsStoppedEventArgs : EventArgs
	{
		public ShipsStopCause StopCause;

		public AllShipsStoppedEventArgs(ShipsStopCause p)
		{
			StopCause = p;
		}
	}
}