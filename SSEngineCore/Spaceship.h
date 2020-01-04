#pragma once
#include "Position2d.h"
#include "Common.h"
#include "Trajectory.h"
#include "MoveArea.h"
#include "MovementDescriptor.h"
#include "Interfaces.h"

namespace SpaceStrategy 
{
	enum class SpaceshipState
	{
		DeterminingPosition,
		DeterminingDirection,
		Moving,
		Standing,
		Removed
	};
	class Spaceship
	{
	public:
		Position2d& Position() { return m_Position; }
		void Position(Position2d value) { m_Position = value; }
		
		float GetDistanceAfterLastTurn() const;
		PROPERTY(MovementDescriptor, MoveType);

		Spaceship(IMovementManager& movementManager);
		MoveArea GetMoveArea() const;
		void Move(float distance);
		void SetCourseTo(const Point2d& target);
	private:
		std::unique_ptr<ITrajectory> m_trajectory;
		Position2d m_Position;
	//	Spaceship(Game game, const Position2d position)
	//	{
	//		m_State = SpaceshipState::DeterminingPosition;
	//		
	//		this->Position(position);
	//		//Game = game;
	//	}
	//	SpaceshipState m_State;
	//	double m_speed;
	//protected:
	//	Spaceship(Game game, Position2d position, double speed)
	//	{
	//		m_State = SpaceshipState::DeterminingPosition;
	//		
	//		this->Position(position);
	//		//Game = game;

	//		this->m_speed=speed;
	//		//Trajectory(new TrajectoryCollection(this));
	//	}
	//public:
	//	~Spaceship();

	//	
	//	void FinishSpaceshipCreation(){
	//		m_State = SpaceshipState::Moving;
	//		//Trajectory.StartPoint = Position;
	//	}

		//internal Game Game{ get; private set; }
		//internal TrajectoryCollection Trajectory{ get; set; }

		/*SpaceshipState State(){ return m_State; }
		void State(SpaceshipState value){ return m_State = value; }

		virtual double Speed{ get{ return speed; } }
		internal float Diameter{ get{ return Game.Params.SpaceshipDiameter; } }
		internal bool IsSelected{ get; set; }
		public virtual void OnTime(TimeSpan dt) { }
		protected virtual void OnMove(object sender, SpaceshipMovedEventArgs e) { }
		public bool TryMove(TimeSpan dt, out double usedDistance)
		{
			if (State == SpaceshipState.Moving && Speed >0) {
				double distance = Speed * dt.Milliseconds / 1000.0;
				double unusedDistance;
				Trajectory.MoveAlong(distance, out unusedDistance);
				usedDistance = distance - unusedDistance;
				OnMove(this, new SpaceshipMovedEventArgs(this.Position, dt));
				return true;
			}
			usedDistance = 0;
			return false;
		}*/
	};
}

