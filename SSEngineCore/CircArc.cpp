#include "stdafx.h"
#include "CircArc.h"
#include <math.h>

namespace SpaceStrategy{
	CircArc::CircArc()
	{
	}
	CircArc::CircArc(Point2d center, float radius, Direction startDir, Direction endDir){
		m_Center = center;
		m_Radius = radius;
		m_EndDir = endDir;
		m_StartDir = startDir;
	}

	CircArc::~CircArc()
	{
	}
	Point2d CircArc::GetStartPoint() const{
		auto v = m_StartDir.ToVector2d();
		return m_Center + v*m_Radius;
	}
	Point2d CircArc::GetEndPoint() const{
		auto v = m_EndDir.ToVector2d();
		return m_Center + v*m_Radius;
	}
	bool CircArc::operator==(const CircArc& right) const{
		return
			m_Center == right.m_Center &&
			m_EndDir == right.m_EndDir &&
			m_StartDir == right.m_StartDir &&
			m_Radius == right.m_Radius;
	}
	bool CircArc::operator!=(const CircArc& right) const{
		return !(*this == right);
	}
}