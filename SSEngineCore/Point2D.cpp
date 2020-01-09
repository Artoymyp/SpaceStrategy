#include "stdafx.h"
#include "Point2d.h"
#include "Direction.h"

namespace SpaceStrategy
{
	Point2d::Point2d()
	{
		m_X = 0;
		m_Y = 0;
	}

	Point2d::Point2d(float x, float y)
	{
		m_X = x;
		m_Y = y;
	}

	Point2d::~Point2d()
	{
	}

	Vector2d Point2d::VectorTo(const Point2d& targetPoint) const
	{
		return Vector2d(targetPoint.m_X - m_X, targetPoint.m_Y - m_Y/*, targetPoint.Z - point.Z*/);
	}

	Vector2d Point2d::ToVector() const
	{
		return Vector2d(m_X, m_Y);
	}

	float Point2d::DistanceSqrTo(const Point2d& targetPoint) const
	{
		const float xDist = m_X - targetPoint.m_X;
		const float yDist = m_Y - targetPoint.m_Y;
		//double zDist = point.Z - targetPoint.Z;
		return (xDist * xDist + yDist * yDist/* + zDist * zDist*/);
	}

	float Point2d::DistanceTo(const Point2d& targetPoint) const
	{
		return sqrt(DistanceSqrTo(targetPoint));
	}

	Direction Point2d::DirectionTo(const Point2d& targetPoint) const
	{
		return Direction(VectorTo(targetPoint).ToRadian());
	}

	bool Point2d::operator==(const Point2d& r) const
	{
		return ToVector() == r.ToVector();
	}

	bool Point2d::operator!=(const Point2d& r) const
	{
		return !(*this == r);
	}
}
