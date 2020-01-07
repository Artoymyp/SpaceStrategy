#include "stdafx.h"
#include "Spaceship.h"
#include "Vector2D.h"
#define _USE_MATH_DEFINES
#include <math.h>
#include "MovementDescriptor.h"
#include "MoveAreaCalculator.h"

namespace SpaceStrategy
{
	Spaceship::Spaceship(IMovementManager& movementManager) :
		m_trajectory(movementManager.CreateTrajectory())
	{
		int i = 1;
		int j = i + 1;
	}

	MoveArea Spaceship::GetMoveArea() const
	{
		MoveAreaCalculator navigator;
		return navigator.GetMoveArea(0, m_MoveType, m_Position);
	}

	void Spaceship::SetCourseTo(const Point2d& target)
	{
		m_trajectory->AddSegment(target);
	}

	void Spaceship::Move(float distance)
	{
		m_trajectory->MovePosition(m_Position, distance);
	}

	//float Spaceship::GetDistanceAfterLastTurn() const{
	//	return m_movementManager.GetDistanceAfterLastTurn(*m_trajectory);
	//}
}
