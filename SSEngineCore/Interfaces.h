#ifndef INTERFACES_H
#define INTERFACES_H

#include "Point2D.h"
#include "Position2D.h"
#include "MovementDescriptor.h"
#include "Common.h"
#include "MoveArea.h"

namespace SpaceStrategy
{
	class MovementDescriptor;

	class ITrajectory
	{
	public:
		virtual void MovePosition(Position2d& position, float distance) = 0;
		virtual void AddSegment(const Point2d& target) = 0;
	};

	class IMovementManager
	{
	public:
		virtual std::unique_ptr<ITrajectory> CreateTrajectory() = 0;
	};
}

#endif // !INTERFACES_H
