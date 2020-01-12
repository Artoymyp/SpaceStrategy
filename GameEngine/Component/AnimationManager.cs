using System;
using System.Collections.Generic;
using System.Drawing;

namespace SpaceStrategy.Component
{
	class AnimationManager
	{
		readonly List<AnimationObject> _animations = new List<AnimationObject>();
		readonly List<AnimationObject> _animationsToAdd = new List<AnimationObject>();
		readonly List<AnimationObject> _animationsToRemove = new List<AnimationObject>();

		internal IEnumerable<AnimationObject> Animations
		{
			get { return _animations; }
		}

		public void AddAnimation(AnimationObject animation)
		{
			_animationsToAdd.Add(animation);
		}

		public void DrawAnimations(Graphics dc)
		{
			foreach (AnimationObject animation in _animations) {
				animation.Draw(dc);
			}
		}

		public void ProcessAnimations(TimeSpan dt)
		{
			foreach (AnimationObject animation in _animationsToRemove) {
				_animations.Remove(animation);
			}
			_animationsToRemove.Clear();

			foreach (AnimationObject animation in _animationsToAdd) {
				_animations.Add(animation);
			}
			_animationsToAdd.Clear();

			foreach (AnimationObject animation in _animations) {
				animation.OnTime(dt);
			}
		}

		public void RemoveAnimation(AnimationObject animation)
		{
			_animationsToRemove.Add(animation);
		}
	}
}
