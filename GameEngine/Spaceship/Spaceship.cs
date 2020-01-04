using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SpaceStrategy
{
	public abstract class Spaceship: GraphicObject
	{
		double speed;
		#region Constructors
		private Spaceship()
		{
			State = SpaceshipState.DeterminingPosition;
		}
		private Spaceship(Game game, Position position) :this()
		{
			this.Position = position;
			Game = game;
		}
		protected Spaceship(Game game, Position position, double speed)
			: this(game, position)
		{
			this.speed = speed;
			Trajectory = new TrajectoryCollection(this);
		} 
		internal void FinishSpaceshipCreation(){
			State = SpaceshipState.Moving;
			Trajectory.StartPoint = Position;
		}
		#endregion
		internal Game Game { get; private set; }
		internal TrajectoryCollection Trajectory { get; set; }
		internal SpaceshipState State { get; set; }
		public virtual double Speed { get { return speed; } }
		internal float Diameter { get { return Game.Params.SpaceshipDiameter; } }
		internal bool IsSelected { get; set; }
		public virtual void OnTime(TimeSpan dt) { }
		public bool TryMove(TimeSpan dt, out double usedDistance)
		{
			if (State == SpaceshipState.Moving && Speed >0) {
				double distance = Speed * dt.Milliseconds / 1000.0;
				Trajectory.MoveAlong(distance, out var unusedDistance);
				usedDistance = distance - unusedDistance;
				return true;
			}
			usedDistance = 0;
			return false;
		}
	}
	public enum SpaceshipState
	{
		DeterminingPosition,
		DeterminingDirection,
		Moving,
		Standing,
		Removed
	}
}
