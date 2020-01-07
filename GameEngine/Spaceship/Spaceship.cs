using System;

namespace SpaceStrategy
{
	public enum SpaceshipState
	{
		DeterminingPosition,
		DeterminingDirection,
		Moving,
		Standing,
		Removed
	}


	public abstract class Spaceship : GraphicObject
	{
		public virtual double Speed { get; }

		internal float Diameter
		{
			get { return Game.Params.SpaceshipDiameter; }
		}

		internal Game Game { get; }

		internal bool IsSelected { get; set; }

		internal SpaceshipState State { get; set; }

		internal TrajectoryCollection Trajectory { get; set; }

		public virtual void OnTime(TimeSpan dt) { }

		public bool TryMove(TimeSpan dt, out double usedDistance)
		{
			if (State == SpaceshipState.Moving && Speed > 0) {
				double distance = Speed * dt.Milliseconds / 1000.0;
				Trajectory.MoveAlong(distance, out double unusedDistance);
				usedDistance = distance - unusedDistance;
				return true;
			}

			usedDistance = 0;
			return false;
		}

		#region Constructors

		protected Spaceship(Game game, Position position, double speed)
			: this(game, position)
		{
			Speed = speed;
			Trajectory = new TrajectoryCollection(this);
		}

		Spaceship()
		{
			State = SpaceshipState.DeterminingPosition;
		}

		Spaceship(Game game, Position position) : this()
		{
			Position = position;
			Game = game;
		}

		internal void FinishSpaceshipCreation()
		{
			State = SpaceshipState.Moving;
			Trajectory.StartPoint = Position;
		}

		#endregion Constructors
	}
}