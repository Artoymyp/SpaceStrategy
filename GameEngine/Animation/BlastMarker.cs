using System;
using System.Drawing;

namespace SpaceStrategy
{
	public class BlastMarker : GraphicObject
	{
		readonly BlastMarkerAnimation _animation;

		internal BlastMarker(Game game, Position position, TimeSpan delay)
		{
			Position = position;
			_animation = new BlastMarkerAnimation(position);
			game.ScriptManager.AddEvent(new ShowAnimationEvent(_animation, delay));

			//AnimationHelper.CreateAnimation(animation);
		}

		public static double CollisionRadius
		{
			get { return 1.35; }
		}

		public void Dispose()
		{
			if (_animation != null) {
				_animation.Drop();
			}
		}

		public override void Draw(Graphics dc) { }
	}
}