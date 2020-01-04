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

		private List<Trajectory> items = new List<Trajectory>();
		internal TrajectoryCollection(Spaceship owner) {
			this.Spaceship = owner;
			DistanceAfterLastTurn = 0;
			MinStraightBeforeTurn = 0;
			TotalDistance = 0;
		}
		internal Point2d StartPoint { get; set; }
		public IEnumerator<Trajectory> GetEnumerator() { return items.GetEnumerator(); }
		IEnumerator IEnumerable.GetEnumerator() { return items.GetEnumerator(); }
		internal void Add(Trajectory trajectory)
		{
			items.Add(trajectory);
		}
		internal void Add(Point2d targetPoint){
			Point2d prevPoint;
			if (items.Count > 0)
				prevPoint = items.Last().EndPoint;
			else
				prevPoint = StartPoint;
			LinearTrajectory newSegment = new LinearTrajectory(this, prevPoint, targetPoint);
			items.Add(newSegment);
			PrevDir = newSegment.Direction;
			Spaceship.State = SpaceshipState.Moving;
		}

		internal Spaceship Spaceship { get; private set; }
		internal void MoveAlong(double distance, out double unusedDistance){
			unusedDistance = distance;
			if (items.Count == 0) {
				return;
			}

			Position newPosition;
			do {
				Trajectory curTrajectory = items[0];
				double newUnusedDistance = 0;
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
					items.Remove(curTrajectory);
				}
				unusedDistance = newUnusedDistance;
			} while (unusedDistance > 0 && items.Count > 0 && Spaceship.State == SpaceshipState.Moving);
			if (items.Count == 0)
				Spaceship.State = SpaceshipState.Standing;
			StartPoint = newPosition;
			Spaceship.Position = newPosition;
		}
		private void Prolong()
		{
			Position lastPosition = items.Last().GetEndPosition();
			items.Add(new LinearTrajectory(this, lastPosition.Location, lastPosition.Location + lastPosition.Direction * 10));
		}

		internal void Draw(System.Drawing.Graphics dc)
		{
			foreach (var item in items) {
				item.Draw(dc);
			}
		}

		internal void OnMouseMove(Point2d coord)
		{
			foreach (var item in items) {
				item.OnMouseMove(coord);
			}
		}

		internal void Add(int index, Trajectory trajectory)
		{
			items.Insert(index, trajectory);
		}

		internal void Remove(Trajectory trajectory)
		{
			items.Remove(trajectory);
		}
	}
}
