using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceStrategy
{
	internal class ScriptManager
	{
		public void AddEvent(ScriptEvent action)
		{
			_actions.Add(action);
		}
		List<ScriptEvent> _actions = new List<ScriptEvent>();

		public void OnTime(TimeSpan dt)
		{
			_actions.RemoveAll(a => a.Complete);
			for (int i = 0; i < _actions.Count; i++)
			{
				var script = _actions[i];
				script.CurTime += dt;
				if (script.CurTime >= script.StartTime)
				{
					script.Run();
				}
			}
		}
	}
	internal class ScriptEvent
	{
		protected Action Action { get; set; }
		public ScriptEvent(Action action, TimeSpan startTime)
		{
			Complete = false;
			CurTime = new TimeSpan();
			this.Action = action;
			StartTime = startTime;
		}
		public void Run()
		{
			if (Action != null)
			{
				Action();
				Complete = true;
			}
		}
		public bool Complete { get; private set; }
		public TimeSpan StartTime { get; private set; }
		public TimeSpan CurTime { get; set; }
	}
	internal class InflictDamage : ScriptEvent
	{
		public InflictDamage(GothicSpaceshipBase spaceship, SpaceshipWeapon attacker, int damage, TimeSpan startTime)
			: base(null, startTime)
		{
			this.Spaceship = spaceship;
			this._damage = damage;
			this._attacker = attacker;
			Action = InflictSpaceshipDamage;
		}
		protected GothicSpaceshipBase Spaceship;
		SpaceshipWeapon _attacker;
		int _damage;
		protected virtual void InflictSpaceshipDamage()
		{
			Spaceship.InflictDamage(_damage);
		}
	}

	internal class InflictDamageToTorpedo : InflictDamage
	{
		public InflictDamageToTorpedo(TorpedoSalvo torpedo, TurretWeapon attacker, int damage, TimeSpan startTime, TimeSpan destructionDispersion)
			: base(torpedo, attacker, damage, startTime)
		{
			this._destructionDispersion = destructionDispersion;
		}
		TimeSpan _destructionDispersion;
		protected override void InflictSpaceshipDamage()
		{
			(Spaceship as TorpedoSalvo).SetMaxTorpedoDestructionDelay(_destructionDispersion);

			base.InflictSpaceshipDamage();
		}
	}
	internal class ShowAnimationEvent : ScriptEvent
	{
		public ShowAnimationEvent(AnimationObject animation, TimeSpan startTime)
			: base(null, startTime)
		{
			Animation = animation;
			Action = ShowAnimation;
		}
		public void ShowAnimation()
		{
			AnimationHelper.CreateAnimation(Animation);
		}
		public AnimationObject Animation { get; set; }
		
	}
}
