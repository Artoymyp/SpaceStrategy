#pragma once
#include "Position2d.h"
#include "Common.h"

namespace SpaceStrategy
{
	class GraphicObject
	{
	public:
		GraphicObject();
		~GraphicObject();

	PROPERTY(Position2d, Position);
	PROPERTY_DEFAULT_VAL(bool, IsCollisionObject, true);
	PROPERTY_DEFAULT_VAL(bool, IsVisible, true);
	};
}
