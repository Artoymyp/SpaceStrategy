﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace SpaceStrategy
{
	public class TorpedoSalvo : GothicSpaceshipBase
	{
		static readonly SpaceshipClass _TorpedoSalvoClass = new SpaceshipClass("Torpedo", "", SpaceshipCategory.Ordnance, "Weapon", 1, 0, 0, 6, 6, 6, 6, 0, 0, 0, 0, 0);
		readonly Color _aimZoneColor;
		readonly TorpedoWeapon _launcherWeapon;
		readonly List<Torpedo> _torpedoes = new List<Torpedo>();
		TimeSpan _maxTorpedoDestructionDelay;
		TorpedoSalvoState _torpedoState;

		public TorpedoSalvo(Game game, Player owner, TorpedoWeapon launcherWeapon, Position position, int power, double speed, Color zoneColor)
			: base(game, position, _TorpedoSalvoClass, owner)
		{
			Power = power;
			Speed = speed;
			MinRunBeforeTurn = 2 * speed;
			_torpedoState = TorpedoSalvoState.Aiming;
			_aimZoneColor = zoneColor;
			_launcherWeapon = launcherWeapon;

			double halfWidth = TorpedoWidth / 2;
			double maxTorpedoShift = Size / 2 - halfWidth;

			int fullRowsCount = power / SalvoWidthCount;
			int lastRowCount = power - fullRowsCount;
			for (int row = 0; row < fullRowsCount; row++)
			for (int column = 0; column < SalvoWidthCount; column++)
				_torpedoes.Add(new Torpedo(this, new Point2d(-(row - 1) * TorpedoLength, column * TorpedoWidth - maxTorpedoShift), (float)TorpedoLength, (float)TorpedoWidth));
			if (lastRowCount == 1) {
				_torpedoes.Add(new Torpedo(this, new Point2d(-(fullRowsCount - 1) * TorpedoLength, 0), (float)TorpedoLength, (float)TorpedoWidth));
			}
			else if (lastRowCount == 2) {
				_torpedoes.Add(new Torpedo(this, new Point2d(-(fullRowsCount - 1) * TorpedoLength, maxTorpedoShift), (float)TorpedoLength, (float)TorpedoWidth));
				_torpedoes.Add(new Torpedo(this, new Point2d(-(fullRowsCount - 1) * TorpedoLength, -maxTorpedoShift), (float)TorpedoLength, (float)TorpedoWidth));
			}

			IsCollisionObject = false;
		}

		public Spaceship LauncherSpaceship
		{
			get { return _launcherWeapon.OwnerSpaceship; }
		}

		public int Power { get; set; }

		public override double Speed { get; }

		internal double Size
		{
			get { return Game.Params.TorpedoSize; }
		}

		protected TimeSpan DefaultDestructionMaxDelay
		{
			get { return new TimeSpan(0, 0, 0, 0, 50); }
		}

		int SalvoWidthCount
		{
			get { return 3; }
		}

		double TorpedoLength
		{
			get { return Size / 2; }
		}

		double TorpedoWidth
		{
			get { return Size / SalvoWidthCount; }
		}

		public override void Draw(Graphics dc)
		{
			GraphicsState oldDc = dc.Save();
			dc.TranslateTransform((float)Position.Location.X, (float)Position.Location.Y);
			dc.RotateTransform((float)Position.Degree);

			switch (_torpedoState) {
				case TorpedoSalvoState.Aiming:
					int alphaChannelIntensity = 64;
					Brush areaBrush = new SolidBrush(Color.FromArgb(alphaChannelIntensity, _aimZoneColor));
					dc.FillRectangle(areaBrush, 0, -(float)Size / 2, Game.Size.Width * 2, (float)Size);
					var areaPen = new Pen(_aimZoneColor, Game.Params.TrajectoryLineThickness);
					dc.DrawLine(areaPen, 0, (float)Size / 2, Game.Size.Width * 2, (float)Size / 2);
					dc.DrawLine(areaPen, 0, -(float)Size / 2, Game.Size.Width * 2, -(float)Size / 2);
					break;

				case TorpedoSalvoState.Flying:
				case TorpedoSalvoState.Destroyed:
					foreach (Torpedo torpedo in _torpedoes) torpedo.Draw(dc, Player.Color);
					dc.DrawEllipse(new Pen(_aimZoneColor, Game.Params.TrajectoryLineThickness / 4), -(float)Size / 2, -(float)Size / 2, (float)Size, (float)Size);
					break;
			}

			dc.Restore(oldDc);
		}

		public override void InflictDamage(int damagePoints)
		{
			base.InflictDamage(damagePoints);
			if (HitPoints == 0) {
				DestroyTorpedoes(Power, _maxTorpedoDestructionDelay);
				IsCollisionObject = false;
				_torpedoState = TorpedoSalvoState.Destroyed;
			}

			_maxTorpedoDestructionDelay = new TimeSpan();
		}

		public override void OnTime(TimeSpan dt)
		{
			foreach (Torpedo torpedo in _torpedoes) torpedo.TryDestruction(dt);
			_torpedoes.RemoveAll(a => a.State == TorpedoState.Destroyed);
			if (_torpedoes.Count == 0) {
				State = SpaceshipState.Removed;

				//Game.RemoveSpaceship(this);
			}
		}

		/// <summary>
		///     Задает время, в течении котого в случайные моменты будут взрываться торпеды.
		///     Вызывать перед вызовом TorpedoSalvo.Attacked, после чего время автоматически установится равным 0.
		/// </summary>
		/// <param name="delay"></param>
		public void SetMaxTorpedoDestructionDelay(TimeSpan delay)
		{
			_maxTorpedoDestructionDelay = delay;
		}

		internal override void Attack(GothicSpaceshipBase attackedGothicSpaceship)
		{
			if (_torpedoState == TorpedoSalvoState.Flying) {
				if (attackedGothicSpaceship is GothicSpaceship) {
					if (attackedGothicSpaceship.BlastMarkersAtBase.Any()) {
						OnNewBlastMarkerContact(this, new NewBlastMarkersContactEventArgs(attackedGothicSpaceship.BlastMarkersAtBase));
					}

					Side attackedSide = GetAttackedSide(this, attackedGothicSpaceship);
					int armor = GetArmor(attackedSide);
					int hitTorpedoCount = Dice.RolledDicesCount(6, Power, armor, string.Format("Торпедная атака по борту {0}", attackedSide.ToString()));
					DestroyTorpedoes(hitTorpedoCount, new TimeSpan());
					attackedGothicSpaceship.Attacked(_launcherWeapon, hitTorpedoCount, new TimeSpan());
				}
				else if (attackedGothicSpaceship is TorpedoSalvo) {
					var attackedTorpedo = attackedGothicSpaceship as TorpedoSalvo;
					Attacked(attackedTorpedo._launcherWeapon, HitPoints, new TimeSpan());
					attackedTorpedo.Attacked(_launcherWeapon, attackedTorpedo.HitPoints, new TimeSpan());
				}
			}
		}

		internal override void Attacked(SpaceshipWeapon attacker, int damage, TimeSpan timeBeforeAttackImpact)
		{
			if (attacker is TurretWeapon) {
				if (damage > Power) {
					Power = 0;
					Game.ScriptManager.AddEvent(new InflictDamageToTorpedo(this, attacker as TurretWeapon, HitPoints, timeBeforeAttackImpact, _maxTorpedoDestructionDelay));

					//InflictDamage(HitPoints);
				}
				else {
					Power = Power - damage;
					DestroyTorpedoes(damage, _maxTorpedoDestructionDelay);
					_maxTorpedoDestructionDelay = new TimeSpan();
				}
			}
			else if (attacker is TorpedoWeapon) {
				SetMaxTorpedoDestructionDelay(DefaultDestructionMaxDelay);
				InflictDamage(HitPoints);
			}
			else if (damage >= HitPoints) {
				Game.ScriptManager.AddEvent(new InflictDamage(this, attacker, HitPoints, timeBeforeAttackImpact));

				//InflictDamage(HitPoints);
			}
		}

		internal void Launch(TorpedoWeapon w)
		{
			IsCollisionObject = true;
			BlastMarkersAtBase.SetCurBlastMarkers(_launcherWeapon.OwnerSpaceship.BlastMarkersAtBase);
			_torpedoState = TorpedoSalvoState.Flying;
		}

		protected override void PassingBlastMarkerEffect()
		{
			if (Dice.RollDices(6, 1, "Пролет торпедного залпа через маркеры взрыва.") >= 6) {
				SetMaxTorpedoDestructionDelay(DefaultDestructionMaxDelay);
				InflictDamage(HitPoints);
			}
		}

		void DestroyTorpedoes(int destroyedTorpedoCount, TimeSpan torpedoDestructionMaxDelay)
		{
			List<Torpedo> activeTorpedoes = _torpedoes.Where(a => a.State == TorpedoState.Normal).ToList();
			for (int i = 0; i < destroyedTorpedoCount; i++)
				if (activeTorpedoes.Count > 0) {
					int destroyedTorpedoIndex = Game.Rand.Next(activeTorpedoes.Count);
					activeTorpedoes[destroyedTorpedoIndex].Destroy(new TimeSpan(0, 0, 0, 0, Game.Rand.Next((int)torpedoDestructionMaxDelay.TotalMilliseconds)));
					activeTorpedoes.Remove(activeTorpedoes[destroyedTorpedoIndex]);
				}
		}


		enum TorpedoSalvoState
		{
			Aiming,
			Flying,
			Destroyed
		}


		enum TorpedoState
		{
			Normal,
			MarkedForDestruction,
			Destroyed
		}


		class Torpedo
		{
			readonly Spaceship _torpedoSalvo;
			readonly Point2d _position;
			TimeSpan _explodeAfter;
			public TorpedoState State;

			public Torpedo(TorpedoSalvo torpedoSalvo, Point2d position, float length, float width)
			{
				_position = position;
				State = TorpedoState.Normal;
				Length = length;
				Width = width;
				_torpedoSalvo = torpedoSalvo;
			}

			float Length { get; }

			float Width { get; }

			public void Destroy(TimeSpan explodeAfter)
			{
				State = TorpedoState.MarkedForDestruction;
				_explodeAfter = explodeAfter;
			}

			public bool TryDestruction(TimeSpan dt)
			{
				if (State == TorpedoState.MarkedForDestruction) {
					if (_explodeAfter > dt) {
						_explodeAfter -= dt;
						return false;
					}

					_explodeAfter = new TimeSpan();
					State = TorpedoState.Destroyed;
					AnimationHelper.CreateAnimation(new RoundExplosionAnimation(new Position(_position + _torpedoSalvo.Position.Location.ToVector() + new Vector(-Length / 2, 0)), new TimeSpan(0, 0, 0, 1), Length / 2, Length * 4));

					//AnimationHelper.CreateAnimation(new SpaceshipRoundExplosionAnimation(this.torpedoSalvo, new Position(this.position+new Vector(-Length/2,0)), new TimeSpan(0, 0, 0, 1), Length / 2, Length * 4));
					return true;
				}

				return false;
			}

			internal void Draw(Graphics dc, Color normalColor)
			{
				switch (State) {
					case TorpedoState.Normal:
					case TorpedoState.MarkedForDestruction:
						var path = new GraphicsPath(new PointF[]
							{
								_position,
								_position + new Vector(-Length, +Width / 2),
								_position + new Vector(-Length, -Width / 2)
							},
							new[]
							{
								(byte)PathPointType.Start,
								(byte)PathPointType.Line,
								(byte)PathPointType.Line
							});
						dc.FillPath(new SolidBrush(normalColor), path);
						break;
				}
			}
		}
	}
}