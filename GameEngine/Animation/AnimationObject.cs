using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SpaceStrategy
{
	internal abstract class AnimationObject
	{
		TimeSpan Time;
        public TimeSpan AnimationDuration { get; protected set; }
		public bool Cyclic;
		protected float Phase;
		internal AnimationObject(Game game, TimeSpan animationDuration, bool cyclic = false)
		{
			Game = game;
			//Game.AddAnimation(this);
			Time = new TimeSpan();
			this.AnimationDuration = animationDuration;
			this.Cyclic = cyclic;
		}
		internal Game Game { get; private set; }
		internal abstract void Draw(Graphics dc);
		internal virtual void OnTime(TimeSpan dt) { 
			Time += dt;
			if (Time >= AnimationDuration) {
				if (Cyclic)
					Time -= AnimationDuration;
				else { 
					Drop(); 
					return; 
				}
			}
			Phase = 1 - (float)(AnimationDuration.TotalMilliseconds - Time.TotalMilliseconds) / (float)AnimationDuration.TotalMilliseconds;
		}
		internal virtual void Drop() { Game.DroppedAnimations.Add(this); }
	}
}
