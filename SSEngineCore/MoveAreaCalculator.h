#pragma once
#include "MoveArea.h"
#include "MovementDescriptor.h"
#include "Position2D.h"

namespace SpaceStrategy
{
	class MoveAreaCalculator
	{
	public:
		MoveAreaCalculator();
		~MoveAreaCalculator();
		MoveArea GetMoveArea(double m_DistanceAfterLastTurn, const MovementDescriptor&, const Position2d&);
	};
}
