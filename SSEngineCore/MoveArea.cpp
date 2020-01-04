#include "stdafx.h"
#include "MoveArea.h"

namespace SpaceStrategy{
	MoveArea::MoveArea()
	{
	}


	MoveArea::~MoveArea()
	{
	}

	void MoveArea::Add(const MovePoint & p){
		m_anchorPoints.push_back(p);
	}
	
	void MoveArea::Add(const MoveSeg & s){
		m_lineSegs.push_back(s);
	}
	void MoveArea::Add(const MoveArc & a){
		m_circArcs.push_back(a);
	}
}