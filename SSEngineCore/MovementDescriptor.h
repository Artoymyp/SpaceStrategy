#pragma once
#include "Common.h"
#include "Interfaces.h"

namespace SpaceStrategy{
	class MovementDescriptor 
	{
	public:
		MovementDescriptor();
		~MovementDescriptor();

		PROPERTY(float, Distance);
		PROPERTY(int, TurnCount);
		PROPERTY(float, StraightBeforeTurn);
		PROPERTY(float, TurnAngle);
	};
}
