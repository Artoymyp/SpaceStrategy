using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SpaceStrategy.Animation;

namespace SpaceStrategy
{
	public enum Leaderships
	{
		Untried = 6,
		Experienced = 7,
		Veteran = 8,
		Crack = 9
	}


	public class BlastMarkersAtBase : IEnumerable<BlastMarker>
	{
		public EventHandler<NewBlastMarkersContactEventArgs> NewBlastMarkerContact;
		readonly List<BlastMarker> _items;
		readonly GothicSpaceshipBase _owner;

		ShieldActiveAnimation _shieldAnimation;

		internal BlastMarkersAtBase(GothicSpaceshipBase owner)
		{
			_owner = owner;
			_items = new List<BlastMarker>();
		}

		public int Count
		{
			get { return _items.Count; }
		}

		public IEnumerator<BlastMarker> GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		public void AddBlastMarker(BlastMarker newBlastMarker)
		{
			if (_items.Contains(newBlastMarker)) {
				return;
			}

			List<BlastMarker> bms = _items.ToList();
			bms.Add(newBlastMarker);
			SetCurBlastMarkers(bms);
		}

		public void DestroyShield()
		{
			if (_shieldAnimation != null) {
				_shieldAnimation.Cyclic = false;
				_shieldAnimation = null;
			}
		}

		public void SetCurBlastMarkers(IEnumerable<BlastMarker> curBlastMarkers)
		{
			var newBMs = new List<BlastMarker>();
			if (curBlastMarkers != null && curBlastMarkers.Any()) {
				newBMs = curBlastMarkers.Where(a => !_items.Contains(a)).ToList();
			}

			_items.Clear();
			_items.AddRange(curBlastMarkers);

			if (newBMs.Any()) {
				OnNewBlastMarkerContact(newBMs);
			}

			if (_owner.IsDestroyed != CatastrophicDamage.None) {
				return;
			}

			if (!_items.Any()) {
				//owner.ShieldPoints = owner.Class.Shield;
				if (_shieldAnimation != null) {
					_shieldAnimation.Cyclic = false;
					_shieldAnimation = null;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		internal void CreateBlastMarkerAtBase(Point2d preferredPosition, TimeSpan delay)
		{
			IEnumerable<Point2d> existingBmPolarPositions = _items.Select(a => a.Position.Location.ToPolarCs(_owner.Position.Location));

			double bmTemp = GeometryHelper.DoublePi / 3.5;
			double bmAngSize = bmTemp;

			IEnumerable<Tuple<double, double>> existingBmPolarRanges = existingBmPolarPositions.Select(a => new Tuple<double, double>(a.Ro - bmAngSize / 2, a.Ro + bmAngSize / 2));
			bool hasIntersections;
			do {
				hasIntersections = false;
				List<Tuple<double, double>> newRanges = null;
				foreach (Tuple<double, double> testedRange in existingBmPolarRanges) {
					foreach (Tuple<double, double> oldRange in existingBmPolarRanges) {
						Tuple<double, double> newRange = null;
						if (GeometryHelper.IsBetween(oldRange.Item2, oldRange.Item1, testedRange.Item1)) {
							hasIntersections = true;
							newRange = new Tuple<double, double>(oldRange.Item1, testedRange.Item2);
						}

						if (GeometryHelper.IsBetween(oldRange.Item2, oldRange.Item1, testedRange.Item2)) {
							hasIntersections = true;
							newRange = new Tuple<double, double>(testedRange.Item1, oldRange.Item2);
						}

						if (hasIntersections) {
							newRanges = existingBmPolarRanges.ToList();
							newRanges.Add(newRange);
							newRanges.Remove(testedRange);
							newRanges.Remove(oldRange);
							break;
						}
					}

					if (hasIntersections) {
						existingBmPolarRanges = newRanges;
						break;
					}
				}
			} while (hasIntersections);

			Point2d actualBmPosition;
			{
				Point2d preferredBmPositionPolar = preferredPosition.ToPolarCs(_owner.Position.Location);
				Tuple<double, double> blastedRangeAroundPreferredPosition = null;
				foreach (Tuple<double, double> range in existingBmPolarRanges)
					if (GeometryHelper.IsBetween(range.Item2, range.Item1, preferredBmPositionPolar.Ro)) {
						blastedRangeAroundPreferredPosition = range;
						break;
					}

				if (blastedRangeAroundPreferredPosition != null) {
					if (GeometryHelper.Distance(blastedRangeAroundPreferredPosition.Item1, preferredBmPositionPolar.Ro, true) > GeometryHelper.Distance(preferredBmPositionPolar.Ro, blastedRangeAroundPreferredPosition.Item2, true)) {
						actualBmPosition = new Point2d(blastedRangeAroundPreferredPosition.Item1, preferredBmPositionPolar.R * 0.9).ToEuclidCs(_owner.Position.Location);
					}
					else {
						actualBmPosition = new Point2d(blastedRangeAroundPreferredPosition.Item2, preferredBmPositionPolar.R * 0.9).ToEuclidCs(_owner.Position.Location);
					}
				}
				else {
					actualBmPosition = preferredPosition;
				}
			}
			double newBmPosDir = actualBmPosition.ToPolarCs(_owner.Position.Location).Ro - GeometryHelper.PiDiv3;
			var bm = new BlastMarker(_owner.Game, new Position(actualBmPosition, newBmPosDir), delay);
			_owner.Game.AddGraphicObject(bm);
			AddBlastMarker(bm);
		}

		protected virtual void OnNewBlastMarkerContact(IEnumerable<BlastMarker> newBlastMarkers)
		{
			ShieldActiveAnimation newAnimation;
			if (_owner.Class.Shield > 0 && _owner.IsDestroyed == CatastrophicDamage.None) {
				if (_shieldAnimation == null) {
					newAnimation = new ShieldActiveAnimation(_owner, true);
					_shieldAnimation = newAnimation;
				}
				else {
					newAnimation = new ShieldActiveAnimation(_owner, false);
				}

				AnimationHelper.CreateAnimation(newAnimation);
			}

			if (NewBlastMarkerContact != null) {
				NewBlastMarkerContact(this, new NewBlastMarkersContactEventArgs(newBlastMarkers));
			}
		}
	}


	public abstract class GothicSpaceshipBase : Spaceship
	{
		BlastMarkersAtBase _blastMarkersAtBase;

		public GothicSpaceshipBase(Game game, Position position, SpaceshipClass spaceshipClass, Player owner)
			: base(game, position, spaceshipClass.Speed)
		{
			Class = spaceshipClass;
			HitPoints = spaceshipClass.HitPoints;
			MinRunBeforeTurn = Class.MinRunBeforeTurn;
			MaxTurnAngle = Class.MaxTurnAngle;
			Trajectory.MinStraightBeforeTurn = Class.MinRunBeforeTurn;

			Player = owner;

			_blastMarkersAtBase = new BlastMarkersAtBase(this);
			_blastMarkersAtBase.NewBlastMarkerContact += OnNewBlastMarkerContact;

			IsCrippled = false;
			IsDestroyed = CatastrophicDamage.None;
		}

		internal event EventHandler<DamageEventArgs> Damaged;

		public SpaceshipClass Class { get; }

		//internal int HitPoints { get { return hitPoints; } private set { hitPoints = Math.Max(0, value); } }
		public int HitPoints { get; set; }

		public bool IsCrippled { get; protected set; }

		public CatastrophicDamage IsDestroyed { get; protected set; }

		public double MaxTurnAngle { get; internal set; }

		public double MinRunBeforeTurn { get; protected set; }

		public Player Player { get; }

		public int ShieldPoints
		{
			get
			{
				if (IsDestroyed != CatastrophicDamage.None) {
					return 0;
				}

				int totalSp = !IsCrippled ? Class.Shield : GeometryHelper.RoundUp(Class.Shield / 2.0);
				return Math.Max(0, totalSp - BlastMarkersAtBase.Count);
			}
		}

		internal BlastMarkersAtBase BlastMarkersAtBase
		{
			get
			{
				if (_blastMarkersAtBase == null) {
					_blastMarkersAtBase = new BlastMarkersAtBase(this);
				}

				return _blastMarkersAtBase;
			}
		}

		//protected set { hitPoints = Math.Max(0, value); } }
		//public double MinTurnRadius { get; private set; }
		public virtual void InflictDamage(int damagePoints)
		{
			HitPoints = Math.Max(0, HitPoints - damagePoints);
		}

		internal static Side GetAttackedSide(GothicSpaceshipBase attacker, GothicSpaceshipBase attacked)
		{
			var attackedSide = Side.Front;
			{
				//Vector attackDir = attacker.Position.Location.VectorTo(attacked.Position.Location);
				Vector attackDir = attacked.Position.Location.VectorTo(attacker.Position.Location);
				double globalAng = attackDir.ToRadian();
				double ang = globalAng - attacked.Position.Angle;
				ang = ang + GeometryHelper.PiDiv4;
				ang = GeometryHelper.GetNonNegativeRemainder(ang, GeometryHelper.DoublePi);
				if (0 <= ang && ang < GeometryHelper.HalfPi) {
					attackedSide = Side.Front;
				}
				else if (GeometryHelper.HalfPi <= ang && ang < GeometryHelper.Pi) {
					attackedSide = Side.Right;
				}
				else if (GeometryHelper.Pi <= ang && ang < GeometryHelper.HalfPi * 3) {
					attackedSide = Side.Back;
				}
				else if (GeometryHelper.HalfPi * 3 <= ang && ang < GeometryHelper.DoublePi) {
					attackedSide = Side.Left;
				}
			}
			return attackedSide;
		}

		internal static Side GetAttackingSide(GothicSpaceshipBase attacker, GothicSpaceshipBase attacked)
		{
			return GetAttackedSide(attacked, attacker);
		}

		internal abstract void Attack(GothicSpaceshipBase attackedGothicSpaceship);

		internal virtual void Attacked(SpaceshipWeapon attacker, int damage, TimeSpan timeBeforeAttackImpact)
		{
			if (attacker == null) {
				throw new ArgumentNullException("Не указано атакующее орудие.");
			}

			if (damage < 0) {
				throw new ArgumentOutOfRangeException("Попытка нанести отрицательный урон.");
			}

			if (attacker is DirectFireWeapon) {
				int shieldAbsorbedDamage = Math.Min(ShieldPoints, damage);

				//Урон щитам автоматически наносится в момент появления бласт маркера у коробля
				Game.ScriptManager.AddEvent(new InflictDamage(this, attacker, Math.Max(0, damage - shieldAbsorbedDamage), timeBeforeAttackImpact));

				//InflictDamage(Math.Max(0, damage - shieldAbsorbedDamage));

				//Place blast markers.
				Point2d attackerLocation = attacker.OwnerSpaceship.Position.Location;

				Vector attackDir = attackerLocation.VectorTo(Position.Location);
				Point2d preferredBmPosition;
				double bmRingRadius = (Diameter / 2 + BlastMarker.CollisionRadius) * 0.9;
				{
					attackDir.Normalize();
					preferredBmPosition = attackerLocation + attackDir * (attackerLocation.DistanceTo(Position) - bmRingRadius);
				}

				for (int i = 0; i < shieldAbsorbedDamage; i++) _blastMarkersAtBase.CreateBlastMarkerAtBase(preferredBmPosition, timeBeforeAttackImpact);
			}
			else if (attacker is TorpedoWeapon) {
				Game.ScriptManager.AddEvent(new InflictDamage(this, attacker, damage, timeBeforeAttackImpact));

				//InflictDamage(damage);
			}
		}

		internal int GetArmor(Side attackedSide)
		{
			int armor = 0;
			{
				switch (attackedSide) {
					case Side.Front:
						armor = Class.FrontArmor;
						break;

					case Side.Left:
						armor = Class.LeftArmor;
						break;

					case Side.Right:
						armor = Class.RightArmor;
						break;

					case Side.Back:
						armor = Class.BackArmor;
						break;
				}
			}
			return armor;
		}

		internal virtual bool MoveTo(Point2d point)
		{
			if (Trajectory.FirstOrDefault() is GothicTrajectory) {
				if ((Trajectory.First() as GothicTrajectory).MoveTo(point)) {
					State = SpaceshipState.Moving;
					return true;
				}
			}

			return false;
		}

		internal bool TryAutoMoveMandatoryDistance()
		{
			if (State == SpaceshipState.Moving || State == SpaceshipState.Standing) {
				if (Trajectory != null) {
					var curTrajectory = (GothicTrajectory)Trajectory.FirstOrDefault();
					if (curTrajectory != null) {
						return curTrajectory.Autocomplete();
					}
				}
			}

			return false;
		}

		protected void OnDamaged(int damage)
		{
			if (damage == 0) {
				return;
			}

			if (HitPoints <= GeometryHelper.RoundUp(Class.HitPoints / 2.0)) {
				IsCrippled = true;
			}

			EventHandler<DamageEventArgs> handler = Damaged;
			if (handler != null) {
				handler(this, new DamageEventArgs(this, Class.HitPoints, HitPoints + damage, HitPoints));
			}
		}

		protected virtual void OnDestroyed()
		{
		}

		protected void OnNewBlastMarkerContact(object sender, NewBlastMarkersContactEventArgs e)
		{
			//ShieldPoints = Math.Max(0, ShieldPoints - 1);
			if (Game.PassedBlastMarkerSpaceships.Contains(this)) { }
			else {
				PassingBlastMarkerEffect();
				Game.PassedBlastMarkerSpaceships.Add(this);
			}
		}

		protected abstract void PassingBlastMarkerEffect();


		public class DamageEventArgs : EventArgs
		{
			public DamageEventArgs(GothicSpaceshipBase attacked, int totalHitPoints, int oldHitPoints, int newHitPoints)
			{
				TotalHitPoints = totalHitPoints;
				OldHitPoints = oldHitPoints;
				NewHitPoints = newHitPoints;

				//Attacker = attacker;
				Attacked = attacked;
			}

			public GothicSpaceshipBase Attacked { get; set; }

			public int NewHitPoints { get; set; }

			public int OldHitPoints { get; set; }

			public int TotalHitPoints { get; set; }

			//public GothicSpaceshipBase Attacker { get; set; }
		}
	}


	public class NewBlastMarkersContactEventArgs : EventArgs
	{
		public NewBlastMarkersContactEventArgs(IEnumerable<BlastMarker> newBlastMarkers)
		{
			NewBlastMarkers = newBlastMarkers;
		}

		public IEnumerable<BlastMarker> NewBlastMarkers { get; set; }
	}
}