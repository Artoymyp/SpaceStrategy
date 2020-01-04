#include "stdafx.h"
#define _USE_MATH_DEFINES
#include <math.h>
#include "Position2d.h"


namespace SpaceStrategy{
	Position2d::Position2d():
		m_Location(0,0),
		m_RadianDirection(0)
	{}

	Position2d::Position2d(const Point2d& location){
		m_Location = location;
	}
	Position2d::Position2d(const Point2d& location, const Direction& direction){
		m_Location = location;
		m_RadianDirection = direction;
	}
	Position2d Position2d::GetTranslated(float distance) const {
		return GetTranslated(distance, m_RadianDirection);
	}
	Position2d Position2d::GetTranslated(float distance, const Direction& direction) const {
		return Position2d(m_Location + direction.ToVector2d()*distance, m_RadianDirection);
	}
	void Position2d::MoveAlongDirection(float distance){
		m_Location = m_Location + m_RadianDirection.ToVector2d()*distance;
	}
	bool Position2d::operator==(const Position2d& r) const{
		return m_Location == r.m_Location && m_RadianDirection == r.m_RadianDirection;
	}
	bool Position2d::operator!=(const Position2d& r) const{
		return !(*this == r);
	}
}

