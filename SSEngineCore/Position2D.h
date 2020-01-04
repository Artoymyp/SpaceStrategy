#pragma once
#include "Common.h"
#include "Point2d.h"
#include "Vector2d.h"
#include "Direction.h"

namespace SpaceStrategy{
	struct Position2d
	{
	public:
		Position2d();
		Position2d(const Point2d& location);
		Position2d(const Point2d& location, const Direction& direction);

		PROPERTY(Point2d, Location);
		PROPERTY(Direction, RadianDirection);

		void MoveAlongDirection(float distance);
		Position2d GetTranslated(float distance) const;
		Position2d GetTranslated(float distance, const Direction& dir) const;
		bool operator==(const Position2d& r)const;
		bool operator!=(const Position2d& r)const;
	};
}
