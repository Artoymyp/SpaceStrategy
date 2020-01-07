using System;
using System.Drawing;

namespace SpaceStrategy
{
	abstract class AnimationObject
	{
		TimeSpan _time;
		protected float Phase;

		internal AnimationObject(Game game, TimeSpan animationDuration, bool cyclic = false)
		{
			Game = game;

			//Game.AddAnimation(this);
			_time = new TimeSpan();
			AnimationDuration = animationDuration;
			Cyclic = cyclic;
		}

		public TimeSpan AnimationDuration { get; protected set; }

		public bool Cyclic { get; set; }

		public Game Game { get; }

		public abstract void Draw(Graphics dc);

		public virtual void Drop()
		{
			Game.DroppedAnimations.Add(this);
		}

		public virtual void OnTime(TimeSpan dt)
		{
			_time += dt;
			if (_time >= AnimationDuration) {
				if (Cyclic) {
					_time -= AnimationDuration;
				}
				else {
					Drop();
					return;
				}
			}

			Phase = 1 - (float)(AnimationDuration.TotalMilliseconds - _time.TotalMilliseconds) / (float)AnimationDuration.TotalMilliseconds;
		}
	}
}