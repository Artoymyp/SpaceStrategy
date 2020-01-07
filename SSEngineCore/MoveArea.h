#pragma once
#include "Point2D.h"
#include "LineSeg.h"
#include "CircArc.h"
#include <vector>

namespace SpaceStrategy
{
	typedef Point2d MovePoint;
	typedef LineSeg MoveSeg;
	typedef CircArc MoveArc;
	typedef std::vector<MovePoint> AnchorPointCollection;
	typedef std::vector<MoveSeg> LineSegCollection;
	typedef std::vector<MoveArc> CircArcCollection;

	class MoveArea
	{
	public:
		MoveArea();
		~MoveArea();

		void Add(const MovePoint& p);
		void Add(const MoveSeg& p);
		void Add(const MoveArc& p);
		AnchorPointCollection AnchorPoints() const { return m_anchorPoints; }
		LineSegCollection LineSegments() const { return m_lineSegs; }
		CircArcCollection CircleArcs() const { return m_circArcs; }
	private:
		AnchorPointCollection m_anchorPoints;
		LineSegCollection m_lineSegs;
		CircArcCollection m_circArcs;
	};
}
