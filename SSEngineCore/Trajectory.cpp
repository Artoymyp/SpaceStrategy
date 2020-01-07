#include "stdafx.h"
#include "Trajectory.h"
#include "Position2D.h"
#include "Interfaces.h"
#include "MoveArea.h"

using namespace std;

namespace SpaceStrategy
{
	Trajectory::Trajectory(IMovementManager& movementManager) :
		m_movementManager(movementManager),
		m_DistanceAfterLastTurn(0)
	{
	}

	Trajectory::~Trajectory()
	{
	}

	void Trajectory::AddSegment(const Point2d& target)
	{
		m_segments.push_back(target);
	}

	void Trajectory::MovePosition(Position2d& position, float distance)
	{
		while (distance > 0 && m_segments.size() > 0)
		{
			Point2d curPoint = *m_segments.begin();
			if (position != curPoint)
			{
				Direction oldDirection = position.RadianDirection();
				Direction newDirection = position.Location().DirectionTo(curPoint);
				if (newDirection != oldDirection)
				{
					position.RadianDirection(newDirection);
					m_DistanceAfterLastTurn = 0;
				}
				float usedDistance = 0;
				float segmentLength = position.Location().DistanceTo(curPoint);
				if (distance >= segmentLength)
				{
					position.Location(curPoint);
					m_segments.erase(m_segments.begin());
					usedDistance = segmentLength;
				}
				else
				{
					position.Location(position.Location() + position.RadianDirection().ToVector2d() * distance);
					usedDistance = distance;
				}
				distance -= usedDistance;
				m_DistanceAfterLastTurn += usedDistance;
			}
			else
			{
				m_segments.erase(m_segments.begin());
			}
		}
	}
}
