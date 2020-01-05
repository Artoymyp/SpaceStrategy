using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SpaceStrategy
{
	internal abstract class AnimationObject
	{
		TimeSpan _time;
		public TimeSpan AnimationDuration { get; protected set; }
		public bool Cyclic;
		protected float Phase;
		internal AnimationObject(Game game, TimeSpan animationDuration, bool cyclic = false)
		{
			Game = game;
			//Game.AddAnimation(this);
			_time = new TimeSpan();
			this.AnimationDuration = animationDuration;
			this.Cyclic = cyclic;
		}
		internal Game Game { get; private set; }
		internal abstract void Draw(Graphics dc);
		internal virtual void OnTime(TimeSpan dt) { 
			_time += dt;
			if (_time >= AnimationDuration) {
				if (Cyclic)
					_time -= AnimationDuration;
				else { 
					Drop(); 
					return; 
				}
			}
			Phase = 1 - (float)(AnimationDuration.TotalMilliseconds - _time.TotalMilliseconds) / (float)AnimationDuration.TotalMilliseconds;
		}
		internal virtual void Drop() { Game.DroppedAnimations.Add(this); }
	}
}
