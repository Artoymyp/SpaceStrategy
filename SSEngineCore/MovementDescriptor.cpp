#include "stdafx.h"
#include "MovementDescriptor.h"

namespace SpaceStrategy{
	MovementDescriptor::MovementDescriptor()
	{
		m_Distance = 0;
		m_StraightBeforeTurn = 0;
		m_TurnAngle = 0;
		m_TurnCount = 0;
	}


	MovementDescriptor::~MovementDescriptor()
	{
	}
}