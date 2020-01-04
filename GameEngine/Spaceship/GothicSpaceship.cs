using SpaceStrategy.Weapon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceStrategy
{
	public class GothicSpaceship : GothicSpaceshipBase
	{
		private List<SpaceshipWeapon> weapons;
		internal Dictionary<SpaceshipWeapon, List<Position>> weaponPlacements = new Dictionary<SpaceshipWeapon, List<Position>>();
		public GothicSpaceship(Game game, Position position, SpaceshipClass spaceshipClass, Player owner)
			: base(game, position, spaceshipClass, owner)
		{
			CriticalDamage = new CriticalDamageCollection(this);
			weapons = new List<SpaceshipWeapon>();
			weapons.Add(new TurretWeapon(this, Class.TurretsPower));
			var weaponsTA = new GameDataSetTableAdapters.SpaceshipClassWeaponryTableAdapter();
			var weaponsTable = weaponsTA.GetDataBySpaceshipClassId(Class.ClassName);
			foreach (var weaponDataRow in weaponsTable)
			{
				SpaceshipWeapon weapon;
				switch (SpaceshipWeapon.GetWeaponType(weaponDataRow.WeaponType))
				{
					case WeaponType.Torpedo: weapon = new TorpedoWeapon(this, weaponDataRow); break;
					case WeaponType.Battary: weapon = new CannonWeapon(this, weaponDataRow); break;
					case WeaponType.Lance: weapon = new LanceWeapon(this, weaponDataRow); break;
					case WeaponType.Nova: weapon = new NovaWeapon(this, weaponDataRow); break;
					default:
						throw new NotImplementedException("Неподдерживаемый вид оружия "+weaponDataRow.WeaponType);
				}
				

				weapons.Add(weapon);				
			}

			foreach (var weapon in weapons)
			{
				//Weapon placement on ship
				weaponPlacements[weapon] = new List<SpaceStrategy.Position>();
				double dx = Diameter / (weapon.Power - 1);
				switch (weapon.SpaceshipSide)
				{
					case Side.Front:
						if (weapon.Power > 1)
						{
							weaponPlacements[weapon].Add(new Position(new Point2d(Diameter / 2 * 1.2, -Diameter / 2)));
							for (int i = 0; i < weapon.Power - 1; i++)
							{
								weaponPlacements[weapon].Add(new Position(weaponPlacements[weapon][i].Location + new Vector(0, dx), 0));
							}
						}
						else if (weapon.Power == 1)
						{
							weaponPlacements[weapon].Add(new Position(new Point2d(Diameter / 2 * 1.2, 0)));
						}
						break;
					case Side.Left:
					case Side.Right:
						int sign = weapon.SpaceshipSide == Side.Left ? -1 : 1;
						weaponPlacements[weapon].Add(new Position(new Point2d(-Diameter / 2, sign * Diameter / 4), sign * GeometryHelper.HalfPi));
						for (int i = 0; i < weapon.Power - 1; i++)
						{
							weaponPlacements[weapon].Add(new Position(weaponPlacements[weapon][i].Location + new Vector(dx, 0), weaponPlacements[weapon][i].Direction));
						}
						break;
					case Side.LFR:
						weaponPlacements[weapon].Add(new Position(new Point2d(-Diameter / 2, 0)));
						for (int i = 0; i < weapon.Power - 1; i++)
						{
							weaponPlacements[weapon].Add(new Position(weaponPlacements[weapon][i].Location + new Vector(dx, 0), weaponPlacements[weapon][i].Direction));
						}
						break;
					case Side.All:
						double dr = GeometryHelper.DoublePi / weapon.Power;
						for (int i = 0; i < weapon.Power; i++)
						{
							Point2d dir = new Point2d(GeometryHelper.Cos(dr * i), GeometryHelper.Sin(dr * i));
							weaponPlacements[weapon].Add(new Position(dir, dir.ToVector()));
						}
						break;
					default:
						break;
				}
			}
			Leadership = 0;
		}
		public override double Speed
		{
			get
			{
				var speed = !IsCrippled ? Class.Speed : Math.Max(0, Class.Speed - 5);
				foreach (var thrustersDamage in CriticalDamage.OfType<ThrustersDamaged>())
				{
					speed = Math.Max(0, speed - 10);
				}
				return speed;
			}
		}
		public IEnumerable<SpaceshipWeapon> Weapons { get { return IsDestroyed==CatastrophycDamage.None ? weapons : new List<SpaceshipWeapon>(); } }
		GothicOrder specialOrder;
		public GothicOrder SpecialOrder
		{
			get { return specialOrder; }
			internal set
			{
				if (specialOrder != value)
				{
					{
						specialOrder = value;
						NotifyPropertyChanged("SpecialOrder");
						NotifyPropertyChanged("AvailableOrders");
					}
				}
			}
		}
		bool movementStarted;
		public bool MovementStarted { get { return movementStarted; } set {
			if (movementStarted != value) {
				movementStarted = value;
				NotifyPropertyChanged("AvailableOrders");
			}
		} 
		}
		public List<GothicOrder> AvailableOrders
		{
			get
			{
				var result = new List<GothicOrder>() { GothicOrder.None };
				if (Player != Game.CurrentPlayer)
				{
					return result;
				}
				if (Game.BattlePhase == GamePhase.Attack) {
					if (Weapons.OfType<TorpedoWeapon>().Any(a => a.LoadedTorpedoCount > 0)) {
						result.Add(GothicOrder.LaunchOrdnance);
					}
				}
				if (Player.SpecialOrderFail)
				{
					return result;
				}
				switch (Game.BattlePhase)
				{
					case GamePhase.Movement:
						if (SpecialOrder == GothicOrder.None)
						{
							if (Weapons.OfType<TorpedoWeapon>().Any(a => a.Power > a.LoadedTorpedoCount)) {
								result.Add(GothicOrder.ReloadOrdnance);
							}
							result.Add(GothicOrder.BraceForImpact);
							if (!MovementStarted) {
								result.Add(GothicOrder.LockOn);
								result.Add(GothicOrder.AllAheadFull);
								result.Add(GothicOrder.BurnRetros);
								result.Add(GothicOrder.ComeToNewDirection); 
							}
						}
						break;
					case GamePhase.Attack:
						if (SpecialOrder == GothicOrder.None) 
						{
							result.Add(GothicOrder.BraceForImpact);
						}
						break;
					case GamePhase.Ordnance:
						if (SpecialOrder == GothicOrder.None) {
							result.Add(GothicOrder.BraceForImpact);
						}
						break;
					case GamePhase.Ending:
						break;
					default:
						break;
				}
				return result;
			}
		}
		int leadership;
		public int Leadership
		{
			get { return leadership; }
			internal set
			{
				if (leadership != value) {
					leadership = value;
					NotifyPropertyChanged("Leadership");
				}
			}
		}
		protected override void OnDestroyed()
		{
			int catastrophycDamageType = Dice.RollDices(6, 2, "Определение типа катастрофических повреждений.");
			string result=string.Empty;
			CatastrophycDamage newState = CatastrophycDamage.None;
			switch (catastrophycDamageType)
			{
				case 2:
				case 3:
				case 4:
				case 5:
				case 6:
					GamePrinter.AddLine("Дрейфующий корпус");
					newState = CatastrophycDamage.DriftingHulk;
					break;
				case 7:
				case 8:
					GamePrinter.AddLine("Пылающий корпус");
					newState = CatastrophycDamage.BlazingHulk;
					break;
				case 9:
				case 10:
				case 11:
					GamePrinter.AddLine("Перегрузка плазменных двигателей");
					newState = CatastrophycDamage.PlasmaDriveOverload;
					break;
				case 12:
					GamePrinter.AddLine("Взрыв Warp-двигателя");
					newState = CatastrophycDamage.WarpDriveImplosion;
					break;
			}
			if (IsDestroyed == CatastrophycDamage.None || IsDestroyed == CatastrophycDamage.DriftingHulk)
				IsDestroyed = newState;
			else if (IsDestroyed == CatastrophycDamage.BlazingHulk) {
				if (newState == CatastrophycDamage.DriftingHulk)
					return;
				IsDestroyed = newState;
			}

			if (IsDestroyed == CatastrophycDamage.PlasmaDriveOverload ||
				IsDestroyed == CatastrophycDamage.WarpDriveImplosion)
			{
				float explosionRadius = Dice.RollDices(6, 3, "Определение радиуса поражения взрыва");
				DriveExplosionWeapon explosion;
				if (IsDestroyed == CatastrophycDamage.PlasmaDriveOverload)
					explosion = new PlasmaDriveExplosion(this, explosionRadius);
				else
					explosion = new WarpDriveImplosion(this, explosionRadius);
				explosion.Explode();
				Game.RemoveSpaceship(this);
			}
			this.BlastMarkersAtBase.DestroyShield();
		}

		public void SetSpecialOrder(GothicOrder specialOrder)
		{
			if (specialOrder != GothicOrder.LaunchOrdnance && specialOrder != GothicOrder.None) {
				if (Player.SpecialOrderFail) {
					return;
				}
				bool otherPlayersMadeSpecialOrders = false;
				foreach (var player in Game.Players) {
					if (player == Player)
						continue;
					if (Player.Spaceships.Any(a => a.SpecialOrder != GothicOrder.None)) {
						otherPlayersMadeSpecialOrders = true;
						break;
					}
				}

				var leadershipTestResult = Dice.RollDices(6, 2, "Проверка лидерства.");
				if (otherPlayersMadeSpecialOrders)
					leadershipTestResult++;
				if (BlastMarkersAtBase.Any())
					leadershipTestResult--;
				if (leadershipTestResult <= this.Leadership)
					GamePrinter.AddLine(string.Format("{0}<={1}. Успех!", leadershipTestResult, this.Leadership));
				else {
					GamePrinter.AddLine(string.Format("{0}>{1}. Провал! Далее невозможно отдавать приказы в этом ходу.", leadershipTestResult, this.Leadership));
					Player.SpecialOrderFail = true;
					specialOrder = GothicOrder.None;
				}
			}
			switch (specialOrder)
			{
				case GothicOrder.ReloadOrdnance:
					SpecialOrder = specialOrder;
					var torpedosReloaded = weapons.Where(a => a is TorpedoWeapon).Select(a=>a as TorpedoWeapon);
					foreach (var torpedoReloaded in torpedosReloaded) {
						torpedoReloaded.Reload();
					}
					break;
				case GothicOrder.AllAheadFull:
				case GothicOrder.ComeToNewDirection:
				case GothicOrder.BurnRetros:
				case GothicOrder.LockOn:
				case GothicOrder.BraceForImpact:
					SpecialOrder = specialOrder;
					if (Trajectory != null)
					{
						GothicTrajectory trajectory = Trajectory.FirstOrDefault() as GothicTrajectory;
						if (trajectory != null)
						{
							trajectory.SetSpecialOrder(specialOrder);
						}
					}
					break;
				case GothicOrder.LaunchOrdnance:
					TorpedoWeapon torpedoLauncher = (TorpedoWeapon)weapons.FirstOrDefault(a => a is TorpedoWeapon);
					if (torpedoLauncher != null && torpedoLauncher.LoadedTorpedoCount > 0)
					{
						Game.SelectPoint();
						torpedoLauncher.Launch(torpedoLauncher.LoadedTorpedoCount);
					}
					break;
				case GothicOrder.None:
				default:
					break;
			}
			//var gothicTrajectory = Trajectory.FirstOrDefault() as GothicTrajectory;
			//if (gothicTrajectory != null)
			//	gothicTrajectory.SpecialOrder = specialOrder;
		}
		internal override bool MoveTo(Point2d coord)
		{
			if (base.MoveTo(coord))
			{
				MovementStarted = true;
				return true;	
			}
			return false;
		}
		public override void Draw(Graphics dc)
		{
			var oldDc = dc.Save();
			dc.TranslateTransform((float)Position.Location.X, (float)Position.Location.Y);

			dc.RotateTransform((float)Position.Degree);
			if (IsSelected)
			{
				float selectionRadius = (float)(Diameter / 2.0 + Game.Params.SelectionThickness);
				dc.DrawEllipse(new Pen(Game.Params.SpaceshipSelectionColor, Game.Params.SelectionThickness), -selectionRadius, -selectionRadius, selectionRadius * 2, selectionRadius * 2);
			}
			Brush shipBrush = new SolidBrush(!IsCrippled ? Player.Color: Color.FromArgb(Player.Color.R/2,Player.Color.G/2,Player.Color.B/2));
			dc.FillEllipse(shipBrush, -(float)Diameter / 2, -(float)Diameter / 2, (float)Diameter, (float)Diameter);

			float dir45coord = (float)GeometryHelper.Cos(GeometryHelper.PiDiv4) * (float)Diameter / 2;
			dc.FillPolygon(shipBrush, new PointF[]{
				new PointF(dir45coord,dir45coord),
				new PointF(2*dir45coord,0),
				new PointF(dir45coord,-dir45coord)
			});
			//dc.DrawLine(new Pen(Brushes.Purple, 3), 0, 0, (float)Size, 0);
			foreach (var weapon in Weapons)
			{
				DrawWeapon(weapon, dc);
			}
			dc.Restore(oldDc);
			oldDc = dc.Save();
			dc.TranslateTransform((float)Position.Location.X, (float)Position.Location.Y - 1.3F * Diameter);
			Gauges.Draw(dc, Game.Params.HPGaugeColor, Class.HP, HitPoints, Diameter * 2);
			if (Class.Shield > 0)
			{
				dc.TranslateTransform(0, Gauges.Height);
				Gauges.Draw(dc, Game.Params.ShieldGaugeColor, Class.Shield, ShieldPoints, Diameter * 2);
			}
			dc.Restore(oldDc);
		}
		private void DrawWeapon(SpaceshipWeapon weapon, Graphics dc)
		{
			Pen fillB = new Pen(weapon.LineColor, Game.Params.AttackCompassThickness / 2);
			foreach (var weaponPos in weaponPlacements[weapon])
			{
				dc.DrawLine(fillB, weaponPos.Location, weaponPos.Location + weaponPos.Direction * 0.5);
			}
		}
		internal void GenerateLeadership()
		{
			int leadership = Dice.RollDices(6, 1, "Определение опытности коробля");
			switch (leadership)
			{
				case 1:
					Leadership = (int)Leaderships.Untried; break;
				case 2:
				case 3:
					Leadership = (int)Leaderships.Experienced; break;
				case 4:
				case 5:
					Leadership = (int)Leaderships.Veteran; break;
				case 6:
					Leadership = (int)Leaderships.Crack; break;
				default:
					break;
			}
		}
		internal override void Attack(GothicSpaceshipBase attackedSpaceship)
		{
			Side attackingSide = GothicSpaceshipBase.GetAttackingSide(this, attackedSpaceship);

			double distance = this.Position.DistanceTo(attackedSpaceship.Position);

			IEnumerable<SpaceshipWeapon> attackingWeapons = Weapons
				.Where(w =>
					w is DirectFireWeapon &&
					!w.IsUsed &&
					w.SpaceshipSide.HasFlag(attackingSide) &&
					w.Range >= distance);

			foreach (var weapon in attackingWeapons)
			{
				weapon.Attack(attackedSpaceship, attackingWeapons);
			}		 
		}
		public override void InflictDamage(int damagePoints)
		{
			int oldHP = HitPoints;
			int damageAfterBrace = BraceDamage(damagePoints);
			base.InflictDamage(damageAfterBrace);
			int actualDamage = oldHP - HitPoints;
			for (int i = 0; i < actualDamage; i++)
			{
				if (Dice.RollDices(6, 1, "Проверка шанса критического повреждения.") == 6)
				{
					CriticalDamage.Add();
				}
			}
			OnDamaged(actualDamage);
			if (hitPoints == 0 && damageAfterBrace > 0)
			{
				OnDestroyed();
			}			
		}
		internal void InflictAdditionalCriticalDamage(int damagePoints)
		{
			int oldHP = HitPoints;
			int damageAfterBrace = BraceDamage(damagePoints);
			base.InflictDamage(damageAfterBrace);
			int actualDamage = oldHP - HitPoints;
			OnDamaged(actualDamage);
			if (hitPoints == 0 && damageAfterBrace > 0)
			{
				OnDestroyed();
			}
		}
		private int BraceDamage(int damagePoints)
		{
			int result;
			if (SpecialOrder == GothicOrder.BraceForImpact)
			{
				result = Dice.RolledDicesCount(6, damagePoints, 4, "Попытка избежать повреждений.");
			}
			else
				result = damagePoints;
			return result;
		}
		protected override void PassingBlastMarkerEffect()
		{
			if (Class.Shield == 0 || CriticalDamage.Any(d=>d is ShieldsCollapse) || IsDestroyed!= CatastrophycDamage.None)
			{
				if (Dice.RollDices(6, 1, string.Format("Пролет {0} через маркеры взрыва.", this.ToString())) >= 6)
				{
					InflictDamage(1);
				}
			}
			if (Trajectory != null)
			{
				GothicTrajectory curTrajectory = Trajectory.FirstOrDefault() as GothicTrajectory;
				if (curTrajectory != null)
				{
					curTrajectory.CutDistanceToMove(Game.Params.BlastMarkerSpeedDamage);
				}
			}
		}
		public override string ToString()
		{
			return string.Format("{0} класса {1}", this.Class.Type, this.Class.ClassName);
		}
		public CriticalDamageCollection CriticalDamage { get; private set; }
		internal void RestoreCriticalDamage()
		{
			int restoreAttempts;
			restoreAttempts = !BlastMarkersAtBase.Any()? hitPoints: GeometryHelper.RoundUp((double)hitPoints / 2.0);
			CriticalDamage.FixDamage(restoreAttempts);
		}
		internal void ApplyCriticalDamageConsequences()
		{
			foreach (var criticalDamage in CriticalDamage)
			{
				criticalDamage.OnNextTurnAction(this);
			}
		}

	 
		public class CriticalDamageCollection : IEnumerable<CriticalDamageBase>
		{
			List<CriticalDamageBase> items;
			GothicSpaceship ownerSpaceship;
			internal CriticalDamageCollection(GothicSpaceship owner)
			{
				this.ownerSpaceship = owner; 
				items = new List<CriticalDamageBase>();
			}
			#region IEnumerable Implementation
			public IEnumerator<CriticalDamageBase> GetEnumerator() { return items.GetEnumerator(); }
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return this.GetEnumerator(); }
			public void Add()
			{
				int damageIndex = Dice.RollDices(6, 2, "Определение типа критического повреждения");
				CriticalDamageBase damageType=null;
				switch (damageIndex)
				{
					case 2: damageType = new DorsalArmamentDamaged(); break;
					case 3: damageType = new StarboardArmamentDamaged(); break;
					case 4: damageType = new PortArmamentDamaged(); break;
					case 5: damageType = new ProwArmamentDamaged(); break;
					case 6: damageType = new EngineRoomDamaged(); break;
					case 7: damageType = new FireDamage(); break;
					case 8: damageType = new ThrustersDamaged(); break;
					case 9: damageType = new BridgeSmashed(); break;
					case 10: damageType = new ShieldsCollapse(); break;
					case 11: damageType = new HullBreach(); break;
					case 12: damageType = new BulkheadCollapse(); break;
				}
				damageType = damageType.GetActualDamageType(ownerSpaceship);
				damageType.ApplyDamage(ownerSpaceship);
				GamePrinter.AddLine(string.Format("Получено повреждение типа {0}", damageType.GetType().Name));
				items.Add(damageType);
				ownerSpaceship.NotifyPropertyChanged("CriticalDamage");
			}
			#endregion

			internal void FixDamage(int restoreAttempts)
			{
				if (restoreAttempts == 0)
					return;
				List<CriticalDamageBase> fixableDamage = items.Where(d => d.IsRepairable).ToList();
				if (fixableDamage.Count == 0)
					return;
				int fixedDamageCount = Dice.RolledDicesCount(6, restoreAttempts, 6, string.Format("Определение количества восстановленных критических повреждений."));

				for (int i = 0; i < fixedDamageCount; i++)
				{
					int currentlyFixedDamageIndex = Game.rand.Next(fixableDamage.Count - 1);
					CriticalDamageBase fixedDamage = fixableDamage[currentlyFixedDamageIndex];
					fixedDamage.FixDamage(ownerSpaceship);
					items.Remove(fixedDamage);
					fixableDamage.Remove(fixedDamage);
					GamePrinter.AddLine(string.Format("Восстановлено критическое повреждение {0}.",fixedDamage.GetType().Name));
					if (fixableDamage.Count == 0)
						return;
				}
				if (fixedDamageCount > 0)
					ownerSpaceship.NotifyPropertyChanged("CriticalDamage");
			}
		}


		public event PropertyChangedEventHandler PropertyChanged;
		internal virtual void NotifyPropertyChanged(string propertyName)
		{
			var handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public void PreviewSpecialOrder(GothicOrder specialOrder)
		{
			if (AvailableOrders.Contains(specialOrder)) {
					if (Trajectory != null) {
						GothicTrajectory trajectory = Trajectory.FirstOrDefault() as GothicTrajectory;
						if (trajectory != null) {
							trajectory.PreviewSpecialOrder(specialOrder);
						}
					}
			}
		}
	}
	public abstract class CriticalDamageBase
	{
		public CriticalDamageBase(bool isRepairable)
		{
			IsRepairable = isRepairable;
		}
		public bool IsRepairable { get; private set; }
		public virtual CriticalDamageBase GetActualDamageType(GothicSpaceship spaceship) { return this; }
		protected virtual CriticalDamageBase DamageTypeIfNotApplicable { get { return this; } }
		public abstract void ApplyDamage(GothicSpaceship spaceship);
		public abstract void FixDamage(GothicSpaceship spaceship);
		public virtual void OnNextTurnAction(GothicSpaceship spaceship) { }
	}
	public class ArmamentDamage : CriticalDamageBase
	{
		private Side side;

		public ArmamentDamage(Side side)
			: base(true)
		{
			this.side = side;
		}
		public override void ApplyDamage(GothicSpaceship spaceship)
		{
			foreach (var weapon in spaceship.Weapons.Where(w => w.SpaceshipSide == side))
			{
				weapon.Power = 0;
			}
		}
		public override void FixDamage(GothicSpaceship spaceship)
		{
			foreach (var weapon in spaceship.Weapons.Where(w => w.SpaceshipSide == side))
			{
				weapon.Power = weapon.normalPower;
			}
		}
		public override CriticalDamageBase GetActualDamageType(GothicSpaceship spaceship)
		{
			if (spaceship.Weapons.Any(w => w.SpaceshipSide == side))
				return this;
			else
			{
				return DamageTypeIfNotApplicable.GetActualDamageType(spaceship);
			}
		}
	}
	public class DorsalArmamentDamaged : ArmamentDamage
	{
		public DorsalArmamentDamaged() : base(Side.LFR) { }
		protected override CriticalDamageBase DamageTypeIfNotApplicable { get { return new StarboardArmamentDamaged(); } }
	}
	public class StarboardArmamentDamaged : ArmamentDamage
	{
		public StarboardArmamentDamaged() : base(Side.Right) { }
		protected override CriticalDamageBase DamageTypeIfNotApplicable { get { return new PortArmamentDamaged(); } }
	}
	public class PortArmamentDamaged : ArmamentDamage
	{
		public PortArmamentDamaged() : base(Side.Left) { }
		protected override CriticalDamageBase DamageTypeIfNotApplicable { get { return new ProwArmamentDamaged(); } }
	}
	public class ProwArmamentDamaged : ArmamentDamage
	{
		public ProwArmamentDamaged() : base(Side.Front) { }
		protected override CriticalDamageBase DamageTypeIfNotApplicable { get { return new EngineRoomDamaged(); } }
	}
	public class EngineRoomDamaged : CriticalDamageBase
	{
		public EngineRoomDamaged() : base(true) { }
		public override void ApplyDamage(GothicSpaceship spaceship)
		{
			spaceship.MaxTurnAngle = 0;
			spaceship.InflictAdditionalCriticalDamage(1);
		}
		public override void FixDamage(GothicSpaceship spaceship)
		{
			if (!spaceship.CriticalDamage.Any(d => d is EngineRoomDamaged && d != this))
				spaceship.MaxTurnAngle = spaceship.Class.MaxTurnAngle;
		}
	}
	public class FireDamage : CriticalDamageBase
	{
		public FireDamage() : base(true) { }
		public override void ApplyDamage(GothicSpaceship spaceship) { return; }
		public override void FixDamage(GothicSpaceship spaceship) { return; }
		public override void OnNextTurnAction(GothicSpaceship spaceship)
		{
			spaceship.InflictAdditionalCriticalDamage(1);
			GamePrinter.AddLine(string.Format("Корабль {0} получил 1 единицу урона от пожара.", spaceship.ToString()));
		}
	}
	public class ThrustersDamaged : CriticalDamageBase
	{
		public ThrustersDamaged() : base(true) { }
		public override void ApplyDamage(GothicSpaceship spaceship)
		{
			//spaceship.Speed = Math.Max(0, spaceship.Speed - 10);
			spaceship.InflictAdditionalCriticalDamage(1);
		}
		public override void FixDamage(GothicSpaceship spaceship)
		{
			//spaceship.Speed = Math.Min(spaceship.Class.Speed, spaceship.Speed + 10);
		}
	}
	public class BridgeSmashed : CriticalDamageBase
	{
		public BridgeSmashed() : base(false) { }
		public override void ApplyDamage(GothicSpaceship spaceship) { spaceship.Leadership = Math.Max(0, spaceship.Leadership - 3); }
		public override void FixDamage(GothicSpaceship spaceship) { throw new NotSupportedException(); }
		protected override CriticalDamageBase DamageTypeIfNotApplicable { get { return new ShieldsCollapse(); } }
		public override CriticalDamageBase GetActualDamageType(GothicSpaceship spaceship)
		{
			if (spaceship.CriticalDamage.Any(d => d is BridgeSmashed))
				return DamageTypeIfNotApplicable;
			else
				return this;
		}
	}
	public class ShieldsCollapse : CriticalDamageBase
	{
		public ShieldsCollapse() : base(false) { }
		public override void ApplyDamage(GothicSpaceship spaceship) { spaceship.Leadership -= 3; }
		public override void FixDamage(GothicSpaceship spaceship) { throw new NotSupportedException(); }
		protected override CriticalDamageBase DamageTypeIfNotApplicable { get { return new HullBreach(); } }
		public override CriticalDamageBase GetActualDamageType(GothicSpaceship spaceship)
		{
			if (spaceship.CriticalDamage.Any(d => d is ShieldsCollapse))
				return DamageTypeIfNotApplicable;
			else
				return this;
		}
	}
	public class HullBreach : CriticalDamageBase
	{
		public HullBreach() : base(false) { }
		public override void ApplyDamage(GothicSpaceship spaceship)
		{
			int damage = Dice.RollDices(3, 1, "Дополнительный урон от повреждения корпуса.");
			spaceship.InflictAdditionalCriticalDamage(damage);
		}
		public override void FixDamage(GothicSpaceship spaceship) { throw new NotSupportedException(); }
	}
	public class BulkheadCollapse : CriticalDamageBase
	{
		public BulkheadCollapse() : base(false) { }
		public override void ApplyDamage(GothicSpaceship spaceship)
		{
			int damage = Dice.RollDices(6, 1, "Дополнительный урон от разрушения переборок.");
			spaceship.InflictAdditionalCriticalDamage(damage);
		}
		public override void FixDamage(GothicSpaceship spaceship) { throw new NotSupportedException(); }
	}

	public enum CatastrophycDamage
	{
		None,
		DriftingHulk,
		BlazingHulk,
		PlasmaDriveOverload,
		WarpDriveImplosion
	}
}
