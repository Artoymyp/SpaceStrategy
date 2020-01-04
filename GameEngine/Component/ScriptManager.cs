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
			actions.Add(action);
		}
		List<ScriptEvent> actions = new List<ScriptEvent>();

		public void OnTime(TimeSpan dt)
		{
			actions.RemoveAll(a => a.Complete);
			for (int i = 0; i < actions.Count; i++)
			{
				var script = actions[i];
				script.CurTime += dt;
				if (script.CurTime >= script.NeedTime)
				{
					script.Run();
				}
			}
		}
	}
	internal class ScriptEvent
	{
		protected Action Action { get; set; }
		public ScriptEvent(Action action, TimeSpan needTime)
		{
			Complete = false;
			CurTime = new TimeSpan();
			this.Action = action;
			NeedTime = needTime;
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
		public TimeSpan NeedTime { get; private set; }
		public TimeSpan CurTime { get; set; }
	}
	internal class InflictDamage : ScriptEvent
	{
		public InflictDamage(GothicSpaceshipBase spaceship, SpaceshipWeapon attakcer, int damage, TimeSpan needTime)
			: base(null, needTime)
		{
			this.spaceship = spaceship;
			this.damage = damage;
			this.attakcer = attakcer;
			Action = InflictSpaceshipDamage;
		}
		protected GothicSpaceshipBase spaceship;
		SpaceshipWeapon attakcer;
		int damage;
		protected virtual void InflictSpaceshipDamage()
		{
			spaceship.InflictDamage(damage);
		}
	}

	internal class InflictDamageToTorpedo : InflictDamage
	{
		public InflictDamageToTorpedo(TorpedoSalvo torpedo, TurretWeapon attakcer, int damage, TimeSpan needTime, TimeSpan destructionDispersion)
			: base(torpedo, attakcer, damage, needTime)
		{
			this.destructionDispersion = destructionDispersion;
		}
		TimeSpan destructionDispersion;
		protected override void InflictSpaceshipDamage()
		{
			(spaceship as TorpedoSalvo).SetMaxTorpedoDestructionDelay(destructionDispersion);

			base.InflictSpaceshipDamage();
		}
	}
	internal class ShowAnimationEvent : ScriptEvent
	{
		public ShowAnimationEvent(AnimationObject animation, TimeSpan needTime)
			: base(null, needTime)
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
