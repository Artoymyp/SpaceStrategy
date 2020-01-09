#include "stdafx.h"
#include "Vector2d.h"

namespace SpaceStrategy
{
	Vector2d::Vector2d()
	{
		m_X = 1;
		m_Y = 0;
	}

	Vector2d::Vector2d(float x, float y)
	{
		m_X = x;
		m_Y = y;
	}

	Vector2d::~Vector2d()
	{
	}

	Vector2d Vector2d::Normalize() const
	{
		const float length = Length();
		if (length == 0)
		{
			throw EXCEPTION_FLT_INVALID_OPERATION;
		}
		return Vector2d(m_X / length, m_Y / length);
	}

	float Vector2d::Length() const
	{
		return sqrt(m_X * m_X + m_Y * m_Y);
	}

	bool Vector2d::operator==(const Vector2d& right) const
	{
		const float precision = 0.0001f;
		if (abs(X() - right.X()) < precision &&
			abs(Y() - right.Y()) < precision)
			return true;
		return false;
	}

	bool Vector2d::operator!=(const Vector2d& right) const
	{
		return !(*this == right);
	}

	float Vector2d::ToRadian() const
	{
		return atan2(m_Y, m_X);
	}
}
