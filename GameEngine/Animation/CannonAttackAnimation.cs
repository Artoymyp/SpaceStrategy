using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SpaceStrategy.Animation
{
	class CannonAttackAnimation : AnimationObject
	{
		readonly List<FireBlob> _allFireBlobs = new List<FireBlob>();

		readonly List<FireBlob> _fireBlobs = new List<FireBlob>();

		TimeSpan _timeFromLastFire;

		public CannonAttackAnimation(CannonWeapon source, Spaceship target) //, int damage)
			: base(target.Game, new TimeSpan())
		{
			var weaponIndexes = new List<int>();

			int totalPower = source.Power;
			if (totalPower == 0) {
				return;
			}

			for (int i = 0; i < totalPower; i++) weaponIndexes.Add(i);
			List<int> workingWeapons = weaponIndexes.GetRandomItems(totalPower);
			List<int> hitWeapons = workingWeapons.GetRandomItems(workingWeapons.Count);

			var blobStartTime = new TimeSpan();
			var maxBlobLifeTime = new TimeSpan();
			for (int i = 0; i < totalPower; i++) {
				if (!workingWeapons.Contains(i)) {
					_allFireBlobs.Add(null);
					continue;
				}

				Position weaponPos = source.OwnerSpaceship.WeaponPlacements[source][i];
				Point2d globalWeaponPos = weaponPos.Location.TransformBy(source.OwnerSpaceship.Position);
				double randX = (Game.Rand.NextDouble() - 0.5) * target.Diameter;
				double randY = (Game.Rand.NextDouble() - 0.5) * target.Diameter;
				var targetShift = new Vector(randX, randY);
				Vector attackDir = globalWeaponPos.VectorTo(target.Position.Location + targetShift);
				double attackDist = attackDir.Length;
				attackDir = attackDir.GetNormalized();
				_allFireBlobs.Add(new FireBlob(new Position(globalWeaponPos, attackDir), attackDist, hitWeapons.Contains(i)));

				//Calculating animation duration;
				double curBlobLifeTimeMs = attackDist / FireBlob.UnitsPerSecSpeed * 1000;
				TimeSpan curBlobLifeTime = blobStartTime + new TimeSpan(0, 0, 0, 0, (int)curBlobLifeTimeMs);
				if (curBlobLifeTime > maxBlobLifeTime) {
					maxBlobLifeTime = curBlobLifeTime;
				}

				blobStartTime += FireSpan;
			}

			_timeFromLastFire = FireSpan;
			AnimationDuration = maxBlobLifeTime;
		}

		static Color Color
		{
			get { return Color.DeepSkyBlue; }
		}

		static TimeSpan FireSpan
		{
			get { return new TimeSpan(0, 0, 0, 0, 30); }
		}

		public override void Draw(Graphics dc)
		{
			foreach (FireBlob blob in _fireBlobs) blob.Draw(dc);
		}

		public override void OnTime(TimeSpan dt)
		{
			_fireBlobs.RemoveAll(a => a.Complete);
			foreach (FireBlob blob in _fireBlobs) blob.Move(dt);
			if (_timeFromLastFire >= FireSpan) {
				if (_allFireBlobs.Count > 0) {
					FireBlob curBlob = _allFireBlobs.First();
					_allFireBlobs.RemoveAt(0);
					if (curBlob != null) {
						_fireBlobs.Add(curBlob);
					}
				}

				_timeFromLastFire = new TimeSpan();
			}

			_timeFromLastFire += dt;
			if (_allFireBlobs.Count == 0 && _fireBlobs.Count == 0) {
				Drop();
			}
		}


		class FireBlob
		{
			readonly double _maxDistance;
			double _distance;

			Position _position;
			public bool Complete;

			public FireBlob(Position startPos, double maxDistance, bool placeExplosion)
			{
				_position = startPos;
				_maxDistance = maxDistance;
			}

			public static double UnitsPerSecSpeed
			{
				get { return 40; }
			}

			static float Radius
			{
				get { return 0.4F; }
			}

			public void Draw(Graphics dc)
			{
				var rect = new RectangleF((float)_position.Location.X - Radius, (float)_position.Location.Y - Radius, Radius * 2, Radius * 2);
				dc.FillEllipse(new SolidBrush(Color), rect);
			}

			public void Move(TimeSpan dt)
			{
				if (Complete) {
					return;
				}

				double distIncrement = dt.Milliseconds / (float)1000 * UnitsPerSecSpeed;
				_position.Location = _position.Location + _position.Direction * distIncrement;
				_distance += distIncrement;
				if (_distance >= _maxDistance) {
					Complete = true;
					AnimationHelper.CreateAnimation(new RoundExplosionAnimation(_position, new TimeSpan(0, 0, 0, 0, 1000), 0, 1.5F));
				}
			}
		}
	}
}