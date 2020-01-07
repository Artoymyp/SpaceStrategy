#include "stdafx.h"
#include "Direction.h"

#define _USE_MATH_DEFINES
#include "Math.h"

namespace SpaceStrategy
{
	Direction::Direction(const float& direction)
	{
		m_direction = ForceInRadianRange(direction);
	}

	Direction::~Direction()
	{
	}

	Vector2d Direction::ToVector2d() const
	{
		return Vector2d(cos(m_direction), sin(m_direction));
	}

	Direction Direction::operator+(const float& r) const
	{
		return Direction(m_direction + r);
	}

	Direction Direction::operator-(const float& r) const
	{
		return Direction(m_direction - r);
	}

	bool Direction::operator==(const float& r) const
	{
		return abs((m_direction - ForceInRadianRange(r))) < m_precision;
	}

	bool Direction::operator==(const Direction& r) const
	{
		return abs((m_direction - r.m_direction)) < m_precision;
	}

	bool Direction::operator!=(const float& r) const
	{
		return !(*this == r);
	}

	bool Direction::operator!=(const Direction& r) const
	{
		return !(*this == r);
	}

	float Direction::ForceInRadianRange(float f) const
	{
		float convertedF = f;
		while (convertedF >= 2 * M_PI)
		{
			convertedF -= 2 * M_PI;
		}
		while (convertedF < 0)
		{
			convertedF += 2 * M_PI;
		}
		return convertedF;
	}
}
