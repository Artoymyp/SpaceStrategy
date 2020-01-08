using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SpaceStrategy
{
	class TrajectoryCollection : IEnumerable<Trajectory>
	{
		readonly List<Trajectory> _items = new List<Trajectory>();

		internal TrajectoryCollection(Spaceship owner)
		{
			Spaceship = owner;
			DistanceAfterLastTurn = 0;
			MinStraightBeforeTurn = 0;
			TotalDistance = 0;
		}

		internal double DistanceAfterLastTurn { get; private set; }

		internal double MinStraightBeforeTurn { get; set; }

		internal Spaceship Spaceship { get; }

		internal Point2d StartPoint { get; set; }

		internal double TotalDistance { get; private set; }

		Vector PrevDir { get; set; }

		public IEnumerator<Trajectory> GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		internal void Add(Trajectory trajectory)
		{
			_items.Add(trajectory);
		}

		internal void Add(Point2d targetPoint)
		{
			Point2d prevPoint;
			if (_items.Count > 0) {
				prevPoint = _items.Last().EndPoint;
			}
			else {
				prevPoint = StartPoint;
			}

			var newSegment = new LinearTrajectory(this, prevPoint, targetPoint);
			_items.Add(newSegment);
			PrevDir = newSegment.Direction;
			Spaceship.State = SpaceshipState.Moving;
		}

		internal void Add(int index, Trajectory trajectory)
		{
			_items.Insert(index, trajectory);
		}

		internal void Draw(Graphics dc)
		{
			foreach (Trajectory item in _items) item.Draw(dc);
		}

		internal void MoveAlong(double distance, out double unusedDistance)
		{
			unusedDistance = distance;
			if (_items.Count == 0) {
				return;
			}

			Position newPosition;
			do {
				Trajectory curTrajectory = _items[0];
				double newUnusedDistance;
				curTrajectory.AddToCurrentDistance(unusedDistance, out newUnusedDistance);
				double usedDistance = unusedDistance - newUnusedDistance;
				DistanceAfterLastTurn += usedDistance;

				TotalDistance += usedDistance;
				newPosition = curTrajectory.GetCurrentPosition();
				Vector newDir = newPosition.Direction;
				if (newDir != PrevDir) {
					DistanceAfterLastTurn = 0;
					PrevDir = newDir;
				}

				if (newUnusedDistance > 0 && Spaceship.State == SpaceshipState.Moving) {
					_items.Remove(curTrajectory);
				}

				unusedDistance = newUnusedDistance;
			} while (unusedDistance > 0 && _items.Count > 0 && Spaceship.State == SpaceshipState.Moving);

			if (_items.Count == 0) {
				Spaceship.State = SpaceshipState.Standing;
			}

			StartPoint = newPosition;
			Spaceship.Position = newPosition;
		}

		internal void OnMouseMove(Point2d point)
		{
			foreach (Trajectory item in _items) item.OnMouseMove(point);
		}

		internal void Remove(Trajectory trajectory)
		{
			_items.Remove(trajectory);
		}

		void Prolong()
		{
			Position lastPosition = _items.Last().GetEndPosition();
			_items.Add(new LinearTrajectory(this, lastPosition.Location, lastPosition.Location + lastPosition.Direction * 10));
		}
	}
}