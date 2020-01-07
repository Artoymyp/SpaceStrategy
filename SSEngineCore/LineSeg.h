#pragma once
#include "Common.h"
#include "Point2D.h"

namespace SpaceStrategy
{
	class LineSeg
	{
	public:
		LineSeg();

		LineSeg(const Point2d& start, const Point2d& end)
		{
			m_Start = start;
			m_End = end;
		}

		bool operator==(const LineSeg& r) const
		{
			return m_Start == r.m_Start && m_End == r.m_End;
		}

		bool operator!=(const LineSeg& r) const
		{
			return !(*this == r);
		}

		~LineSeg();
	PROPERTY(Point2d, Start);
	PROPERTY(Point2d, End);
	};
}
