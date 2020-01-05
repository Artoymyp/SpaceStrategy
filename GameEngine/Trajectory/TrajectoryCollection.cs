using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SpaceStrategy
{
	class TrajectoryCollection : IEnumerable<Trajectory>
	{
		internal double DistanceAfterLastTurn { get; private set; }
		private Vector PrevDir { get; set; }
		internal double MinStraightBeforeTurn { get; set; }
		internal double TotalDistance { get; private set; }

		private List<Trajectory> _items = new List<Trajectory>();
		internal TrajectoryCollection(Spaceship owner) {
			this.Spaceship = owner;
			DistanceAfterLastTurn = 0;
			MinStraightBeforeTurn = 0;
			TotalDistance = 0;
		}
		internal Point2d StartPoint { get; set; }
		public IEnumerator<Trajectory> GetEnumerator() { return _items.GetEnumerator(); }
		IEnumerator IEnumerable.GetEnumerator() { return _items.GetEnumerator(); }
		internal void Add(Trajectory trajectory)
		{
			_items.Add(trajectory);
		}
		internal void Add(Point2d targetPoint){
			Point2d prevPoint;
			if (_items.Count > 0)
				prevPoint = _items.Last().EndPoint;
			else
				prevPoint = StartPoint;
			LinearTrajectory newSegment = new LinearTrajectory(this, prevPoint, targetPoint);
			_items.Add(newSegment);
			PrevDir = newSegment.Direction;
			Spaceship.State = SpaceshipState.Moving;
		}

		internal Spaceship Spaceship { get; private set; }
		internal void MoveAlong(double distance, out double unusedDistance){
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
			if (_items.Count == 0)
				Spaceship.State = SpaceshipState.Standing;
			StartPoint = newPosition;
			Spaceship.Position = newPosition;
		}
		private void Prolong()
		{
			Position lastPosition = _items.Last().GetEndPosition();
			_items.Add(new LinearTrajectory(this, lastPosition.Location, lastPosition.Location + lastPosition.Direction * 10));
		}

		internal void Draw(System.Drawing.Graphics dc)
		{
			foreach (var item in _items) {
				item.Draw(dc);
			}
		}

		internal void OnMouseMove(Point2d coord)
		{
			foreach (var item in _items) {
				item.OnMouseMove(coord);
			}
		}

		internal void Add(int index, Trajectory trajectory)
		{
			_items.Insert(index, trajectory);
		}

		internal void Remove(Trajectory trajectory)
		{
			_items.Remove(trajectory);
		}
	}
}
