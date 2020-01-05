using SpaceStrategy.Animation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceStrategy
{
	public abstract class GothicSpaceshipBase:Spaceship
	{
		BlastMarkersAtBase _blastMarkersAtBase;
		int _hitPoints;
		double _maxTurnAngle;
		public GothicSpaceshipBase(Game game, Position position, SpaceshipClass spaceshipClass, Player owner)
			: base(game, position, spaceshipClass.Speed)
		{
			Class = spaceshipClass;
			_hitPoints = spaceshipClass.HitPoints;
			MinRunBeforeTurn = Class.MinRunBeforeTurn;
			_maxTurnAngle = Class.MaxTurnAngle;
			Trajectory.MinStraightBeforeTurn = Class.MinRunBeforeTurn;

			Player = owner;

			_blastMarkersAtBase = new BlastMarkersAtBase(this);
			_blastMarkersAtBase.NewBlastMarkerContact += OnNewBlastMarkerContact;

			IsCrippled = false;
			IsDestroyed = CatastrophycDamage.None;
		}

		protected void OnNewBlastMarkerContact(object sender, NewBlastMarkersContactEventArgs e)
		{
			//ShieldPoints = Math.Max(0, ShieldPoints - 1);				
			if (Game.PassedBlastMarkerSpaceships.Contains(this))
			{
				return;
			}
			else
			{
				PassingBlastMarkerEffect();					
				Game.PassedBlastMarkerSpaceships.Add(this);				
			}
		}
		public bool IsCrippled { get; protected set; }		
		protected abstract void PassingBlastMarkerEffect();
		public Player Player { get; private set; }
		public SpaceshipClass Class { get; private set; }
		public CatastrophycDamage IsDestroyed { get; protected set; }
		//internal int HitPoints { get { return hitPoints; } private set { hitPoints = Math.Max(0, value); } }
		public int HitPoints { get { return _hitPoints; } }//protected set { hitPoints = Math.Max(0, value); } }
		public int ShieldPoints
		{
			get {
				if (IsDestroyed != CatastrophycDamage.None)
					return 0;
				var totalSp = !IsCrippled ? Class.Shield : GeometryHelper.RoundUp((double)Class.Shield / 2.0);
				return Math.Max(0, totalSp - BlastMarkersAtBase.Count);
			}
		}
		public double MaxTurnAngle { get { return _maxTurnAngle; } internal set{ _maxTurnAngle = value;} }
		public double MinRunBeforeTurn { get; protected set; }
		//public double MinTurnRadius { get; private set; }
		public virtual void InflictDamage(int damagePoints)
		{
			_hitPoints = Math.Max(0, _hitPoints - damagePoints);
		}
		internal bool TryAutoMoveMandatoryDistance()
		{
			if (State == SpaceshipState.Moving || State == SpaceshipState.Standing)
			{
				if (Trajectory != null)
				{
					GothicTrajectory curTrajectory = (GothicTrajectory)Trajectory.FirstOrDefault();
					if (curTrajectory != null)
					{
						return curTrajectory.Autocomplete();
					}
				}
			}
			return false;
		}
	 
		internal BlastMarkersAtBase BlastMarkersAtBase
		{
			get
			{
				if (_blastMarkersAtBase == null)
					_blastMarkersAtBase = new BlastMarkersAtBase(this);
				return _blastMarkersAtBase;
			}
		}
		internal abstract void Attack(GothicSpaceshipBase attackedGothicSpaceship);
		public class DamageEventArgs:EventArgs{
			public DamageEventArgs(GothicSpaceshipBase attacked, int totalHitPoints, int oldHitPoints, int newHitPoints)
			{
				TotalHitPoints = totalHitPoints;
				OldHitPoints = oldHitPoints;
				NewHitPoints = newHitPoints;
				//Attacker = attacker;
				Attacked = attacked;
			}		
			public int TotalHitPoints { get; set; }

			public int OldHitPoints { get; set; }

			public int NewHitPoints { get; set; }

			//public GothicSpaceshipBase Attacker { get; set; }

			public GothicSpaceshipBase Attacked { get; set; }
		}
		internal event EventHandler<DamageEventArgs> Damaged;
		protected virtual void OnDestroyed() { }
		protected void OnDamaged(int damage)
		{
			if (damage == 0)
				return;
			if (_hitPoints <= GeometryHelper.RoundUp((double)Class.HitPoints / 2.0))
			{
				IsCrippled = true;
			}
			var handler = Damaged;
			if (handler != null)
			{
				handler(this, new DamageEventArgs(this, Class.HitPoints, HitPoints + damage, HitPoints));
			}
		}
		internal virtual void Attacked(SpaceshipWeapon attacker, int damage, TimeSpan timeBeforeAttackImpact)
		{
			if (attacker == null)
				throw new ArgumentNullException("Не указано атакующее орудие.");
			if (damage < 0)
				throw new ArgumentOutOfRangeException("Попытка нанести отрицательный урон.");
			if (attacker is DirectFireWeapon)
			{
				int shieldAbsorbedDamage = Math.Min(ShieldPoints, damage);
				//Урон щитам автоматически наносится в момент появления бласт маркера у коробля
				Game.ScriptManager.AddEvent(new InflictDamage(this, attacker, Math.Max(0, damage - shieldAbsorbedDamage), timeBeforeAttackImpact)); 
				//InflictDamage(Math.Max(0, damage - shieldAbsorbedDamage));

				//Place blast markers.
				var attackerLocation = attacker.OwnerSpaceship.Position.Location;
				
				Vector attackDir = attackerLocation.VectorTo(Position.Location);
				Point2d preferedBmPosition;
				double bmRingRadius = (Diameter / 2 + BlastMarker.CollisionRadius)*0.9;
				{
					attackDir.Normalize();
					preferedBmPosition = attackerLocation + attackDir * (attackerLocation.DistanceTo(Position) - bmRingRadius);
				}

				for (int i = 0; i < shieldAbsorbedDamage; i++)
				{
					_blastMarkersAtBase.CreateBlastMarkerAtBase(preferedBmPosition, timeBeforeAttackImpact);					
				}
			}
			else if (attacker is TorpedoWeapon)
			{
				Game.ScriptManager.AddEvent(new InflictDamage(this, attacker, damage, timeBeforeAttackImpact)); 
				//InflictDamage(damage);
			}
		}
		internal int GetArmor(Side attackedSide)
		{
			int armor = 0;
			{
				switch (attackedSide)
				{
					case Side.Front: armor = this.Class.FrontArmor; break;
					case Side.Left: armor = this.Class.LeftArmor; break;
					case Side.Right: armor = this.Class.RightArmor; break;
					case Side.Back: armor = this.Class.BackArmor; break;
				}
			}
			return armor;
		}
		internal virtual bool MoveTo(Point2d coord)
		{
			if (Trajectory.FirstOrDefault() is GothicTrajectory)
			{
				if ((Trajectory.First() as GothicTrajectory).MoveTo(coord))
				{
					State = SpaceshipState.Moving;
					return true;
				}
			}
			return false;
		}
		internal static Side GetAttackedSide(GothicSpaceshipBase attacker, GothicSpaceshipBase attacked)
		{
			Side attackedSide = Side.Front;
			{
				//Vector attackDir = attacker.Position.Location.VectorTo(attacked.Position.Location);
				Vector attackDir = attacked.Position.Location.VectorTo(attacker.Position.Location);
				double globalAng = attackDir.ToRadian();
				double ang = globalAng - attacked.Position.Angle;
				ang = ang + GeometryHelper.PiDiv4;
				ang = GeometryHelper.Modul(ang, GeometryHelper.DoublePi);
				if (0 <= ang && ang < GeometryHelper.HalfPi)
					attackedSide = Side.Front;
				else if (GeometryHelper.HalfPi <= ang && ang < GeometryHelper.Pi)
					attackedSide = Side.Right;
				else if (GeometryHelper.Pi <= ang && ang < GeometryHelper.HalfPi * 3)
					attackedSide = Side.Back;
				else if (GeometryHelper.HalfPi * 3 <= ang && ang < GeometryHelper.DoublePi)
					attackedSide = Side.Left;
			}
			return attackedSide;
		}
		internal static Side GetAttackingSide(GothicSpaceshipBase attacker, GothicSpaceshipBase attacked)
		{
			return GetAttackedSide(attacked, attacker);
		}

	}
	public class BlastMarkersAtBase : IEnumerable<BlastMarker>
	{
		List<BlastMarker> _items;
		GothicSpaceshipBase _owner;
		internal BlastMarkersAtBase(GothicSpaceshipBase owner)
		{
			this._owner = owner;
			_items = new List<BlastMarker>();
		}
		public IEnumerator<BlastMarker> GetEnumerator() { return _items.GetEnumerator(); }
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return this.GetEnumerator(); }
		public void DestroyShield()
		{
			if (_shieldAnimation != null)
			{
				_shieldAnimation.Cyclic = false;
				_shieldAnimation = null;
			}
		}
		public void SetCurBlastMarkers(IEnumerable<BlastMarker> curBlastMarkers)
		{
			List<BlastMarker> newBMs = new List<BlastMarker>();
			if (curBlastMarkers != null && curBlastMarkers.Any())
			{
				newBMs = curBlastMarkers.Where(a => !_items.Contains(a)).ToList();
			}
			_items.Clear();
			_items.AddRange(curBlastMarkers);

			if (newBMs.Any())
				OnNewBlastMarkerContact(newBMs);

			if (_owner.IsDestroyed != CatastrophycDamage.None)
				return;

			if (!_items.Any())
			{
				//owner.ShieldPoints = owner.Class.Shield;
				if (_shieldAnimation != null)
				{
					_shieldAnimation.Cyclic = false;
					_shieldAnimation = null;
				}
			}
		}
		public void AddBlastMarker(BlastMarker newBlastMarker)
		{
			if (_items.Contains(newBlastMarker))
				return;
			List<BlastMarker> bms = _items.ToList();
			bms.Add(newBlastMarker);
			SetCurBlastMarkers(bms);
		}

		private ShieldActiveAnimation _shieldAnimation;
		public EventHandler<NewBlastMarkersContactEventArgs> NewBlastMarkerContact;
		protected virtual void OnNewBlastMarkerContact(IEnumerable<BlastMarker> newBlastMarkers)
		{
			ShieldActiveAnimation newAnimation;
			if (_owner.Class.Shield > 0 && _owner.IsDestroyed== CatastrophycDamage.None)
			{
				if (_shieldAnimation == null)
				{
					newAnimation = new ShieldActiveAnimation(_owner, true);
					_shieldAnimation = newAnimation;
				}
				else
				{
					newAnimation = new ShieldActiveAnimation(_owner, false);
				}
				AnimationHelper.CreateAnimation(newAnimation);
			}
			if (NewBlastMarkerContact != null)
				NewBlastMarkerContact(this, new NewBlastMarkersContactEventArgs(newBlastMarkers));
		}

		public int Count { get { return _items.Count; } }

		internal void CreateBlastMarkerAtBase(Point2d preferredPosition, TimeSpan delay)
		{
			IEnumerable<Point2d> existingBmPolarPositions = _items.Select(a => a.Position.Location.ToPolarCs(_owner.Position.Location));

			//double bmAngSizeCoef = 2;
			//double bmAngSize = 2 * Math.Asin(BlastMarker.CollisionRadius / bmRingRadius) * bmAngSizeCoef;				
			double bmTemp = GeometryHelper.DoublePi / 3.5;
			double bmAngSize = bmTemp;
				
			IEnumerable<Tuple<double, double>> existingBmPolarRanges = existingBmPolarPositions.Select(a => new Tuple<double, double>(a.Ro - bmAngSize / 2, a.Ro + bmAngSize / 2));
			bool hasIntersections;
			do
			{
				hasIntersections = false;
				List<Tuple<double, double>> newRanges = null;
				foreach (var testedRange in existingBmPolarRanges)
				{
					foreach (var oldRange in existingBmPolarRanges)
					{
						Tuple<double, double> newRange = null;
						if (GeometryHelper.IsBetween(oldRange.Item2, oldRange.Item1, testedRange.Item1))
						{
							hasIntersections = true;
							newRange = new Tuple<double, double>(oldRange.Item1, testedRange.Item2);
						}
						if (GeometryHelper.IsBetween(oldRange.Item2, oldRange.Item1, testedRange.Item2))
						{
							hasIntersections = true;
							newRange = new Tuple<double, double>(testedRange.Item1, oldRange.Item2);
						}
						if (hasIntersections)
						{
							newRanges = existingBmPolarRanges.ToList();
							newRanges.Add(newRange);
							newRanges.Remove(testedRange);
							newRanges.Remove(oldRange);
							break;
						}
					}
					if (hasIntersections)
					{
						existingBmPolarRanges = newRanges;
						break;
					}
				}
			}
			while (hasIntersections);

			Point2d actualBmPosition;
			{
				Point2d preferedBmPositionPolar = preferredPosition.ToPolarCs(_owner.Position.Location);
				Tuple<double, double> blastedRangeAroundPreferredPosition = null;
				foreach (var range in existingBmPolarRanges)
				{
					if (GeometryHelper.IsBetween(range.Item2, range.Item1, preferedBmPositionPolar.Ro))
					{
						blastedRangeAroundPreferredPosition = range;
						break;
					}
				}

				if (blastedRangeAroundPreferredPosition != null)
				{
					if (GeometryHelper.Distance(blastedRangeAroundPreferredPosition.Item1, preferedBmPositionPolar.Ro, true) > GeometryHelper.Distance(preferedBmPositionPolar.Ro, blastedRangeAroundPreferredPosition.Item2, true))
					{
						actualBmPosition = new Point2d(blastedRangeAroundPreferredPosition.Item1, preferedBmPositionPolar.R * 0.9).ToEuclidCs(_owner.Position.Location);
					}
					else
					{
						actualBmPosition = new Point2d(blastedRangeAroundPreferredPosition.Item2, preferedBmPositionPolar.R * 0.9).ToEuclidCs(_owner.Position.Location);
					}
				}
				else
				{
					actualBmPosition = preferredPosition;
				}
			}
			double newBmPosDir = actualBmPosition.ToPolarCs(_owner.Position.Location).Ro - GeometryHelper.PiDiv3;
			BlastMarker bm = new BlastMarker(_owner.Game, new Position(actualBmPosition, newBmPosDir), delay);
			_owner.Game.AddGraphicObject(bm);
			AddBlastMarker(bm);
		}
	}
	public class NewBlastMarkersContactEventArgs:EventArgs{
		public NewBlastMarkersContactEventArgs(IEnumerable<BlastMarker> newBlastMarkers){
			this.NewBlastMarkers=newBlastMarkers;
		}	
		public IEnumerable<BlastMarker> NewBlastMarkers { get; set; }
	}

	public enum Leaderships
	{
		Untried=6,
		Experienced=7,
		Veteran=8,
		Crack=9
	}
}
