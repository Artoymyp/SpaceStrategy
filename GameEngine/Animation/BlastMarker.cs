using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceStrategy
{
	public class BlastMarker : GraphicObject
	{
		BlastMarkerAnimation _animation;
		internal BlastMarker(Game game, Position position, TimeSpan delay)
			: base()
		{
			Position = position;
			_animation = new BlastMarkerAnimation(position);
			game.ScriptManager.AddEvent(new ShowAnimationEvent(_animation, delay));
			//AnimationHelper.CreateAnimation(animation);
		}
		public override void Draw(System.Drawing.Graphics dc)
		{}
		public void Dispose()
		{
			if (_animation!=null)
				_animation.Drop();
		}
		public static double CollisionRadius { get { return 1.35; } }
	}
}
