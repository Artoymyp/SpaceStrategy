#pragma once
#include "Common.h"
#include "Direction.h"
#include "Point2D.h"
#include "Position2D.h"
#include "Interfaces.h"
#include "MovementDescriptor.h"
#include <vector>

namespace SpaceStrategy
{
	class Trajectory : public ITrajectory
	{
	private:
		std::vector<Point2d> m_segments;
		IMovementManager& m_movementManager;
	public:
	PROPERTY(float, DistanceAfterLastTurn);
		Trajectory(IMovementManager& movementManager);
		~Trajectory();

		void AddSegment(const Point2d& target) override;
		bool IsEmpty() const { return m_segments.empty(); }
		void MovePosition(Position2d& position, float distance) override;
	};
}
