#pragma once
#include "Common.h"
#include "Point2D.h"

namespace SpaceStrategy
{
	class CircArc
	{
	public:
		CircArc();
		CircArc(Point2d center, float radius, Direction startDir, Direction endDir);
		~CircArc();

		Point2d GetStartPoint() const;
		Point2d GetEndPoint() const;
	PROPERTY(Point2d, Center);
	PROPERTY(Direction, StartDir);
	PROPERTY(Direction, EndDir);
	PROPERTY(float, Radius);

		bool operator==(const CircArc& right) const;
		bool operator!=(const CircArc& right) const;
	};
}
