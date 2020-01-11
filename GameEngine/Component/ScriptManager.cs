using System;
using System.Collections.Generic;

namespace SpaceStrategy
{
	class InflictDamage : ScriptEvent
	{
		readonly int _damage;
		protected GothicSpaceshipBase Spaceship;

		public InflictDamage(GothicSpaceshipBase spaceship, SpaceshipWeapon attacker, int damage, TimeSpan startTime)
			: base(null, startTime)
		{
			Spaceship = spaceship;
			_damage = damage;
			Action = InflictSpaceshipDamage;
		}

		protected virtual void InflictSpaceshipDamage()
		{
			Spaceship.InflictDamage(_damage);
		}
	}


	class InflictDamageToTorpedo : InflictDamage
	{
		readonly TimeSpan _destructionDispersion;

		public InflictDamageToTorpedo(TorpedoSalvo torpedo, TurretWeapon attacker, int damage, TimeSpan startTime, TimeSpan destructionDispersion)
			: base(torpedo, attacker, damage, startTime)
		{
			_destructionDispersion = destructionDispersion;
		}

		TorpedoSalvo Torpedo
		{
			get { return (TorpedoSalvo)Spaceship; }
		}

		protected override void InflictSpaceshipDamage()
		{
			Torpedo.SetMaxTorpedoDestructionDelay(_destructionDispersion);

			base.InflictSpaceshipDamage();
		}
	}


	class ScriptEvent
	{
		public ScriptEvent(Action action, TimeSpan startTime)
		{
			Complete = false;
			CurTime = new TimeSpan();
			Action = action;
			StartTime = startTime;
		}

		public bool Complete { get; private set; }

		public TimeSpan CurTime { get; set; }

		public TimeSpan StartTime { get; }

		protected Action Action { get; set; }

		public void Run()
		{
			if (Action != null) {
				Action();
				Complete = true;
			}
		}
	}


	class ScriptManager
	{
		readonly List<ScriptEvent> _actions = new List<ScriptEvent>();

		public void AddEvent(ScriptEvent action)
		{
			_actions.Add(action);
		}

		public void OnTime(TimeSpan dt)
		{
			_actions.RemoveAll(a => a.Complete);
			foreach (ScriptEvent script in _actions) {
				script.CurTime += dt;
				if (script.CurTime >= script.StartTime) {
					script.Run();
				}
			}
		}
	}


	class ShowAnimationEvent : ScriptEvent
	{
		public ShowAnimationEvent(AnimationObject animation, TimeSpan startTime)
			: base(null, startTime)
		{
			Animation = animation;
			Action = ShowAnimation;
		}

		public AnimationObject Animation { get; set; }

		public void ShowAnimation()
		{
			AnimationHelper.CreateAnimation(Animation);
		}
	}
}