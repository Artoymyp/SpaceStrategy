#include "stdafx.h"
#include "MoveAreaCalculator.h"
#include "Point2D.h"

namespace SpaceStrategy
{
	MoveAreaCalculator::MoveAreaCalculator()
	{
	}

	MoveAreaCalculator::~MoveAreaCalculator()
	{
	}

	MoveArea MoveAreaCalculator::GetMoveArea(double m_DistanceAfterLastTurn, const MovementDescriptor& m_MoveType,
	                                         const Position2d& m_Position)
	{
		auto area = MoveArea();
		if (m_MoveType.Distance() <= 0)
		{
			return area;
		}
		Point2d endOfStraightSegment;
		{
			const Vector2d straightVector = m_Position.RadianDirection().ToVector2d() * min(
				m_MoveType.Distance(), m_MoveType.StraightBeforeTurn() - m_DistanceAfterLastTurn);
			endOfStraightSegment = m_Position.Location() + straightVector;

			if (straightVector.Length() > 0)
			{
				area.Add(endOfStraightSegment);
				area.Add(LineSeg(m_Position.Location(), endOfStraightSegment));
			}
		}
		const float distanceAfterFirstTurn = max(
			0, m_MoveType.Distance() - m_MoveType.StraightBeforeTurn() + m_DistanceAfterLastTurn);
		if (m_MoveType.TurnCount() == 2)
		{
			const float distanceAfterSecondTurn = max(
				0, m_MoveType.Distance() - m_MoveType.StraightBeforeTurn() * 2 + m_DistanceAfterLastTurn);
			if (distanceAfterFirstTurn > 0)
			{
				area.Add(CircArc(endOfStraightSegment, distanceAfterFirstTurn, Direction(-m_MoveType.TurnAngle()),
				                 Direction(m_MoveType.TurnAngle())));
			}
			if (distanceAfterSecondTurn > 0)
			{
				const Point2d leftTurnPoint = Point2d(
					endOfStraightSegment + (m_Position.RadianDirection() + m_MoveType.TurnAngle()).ToVector2d() *
					m_MoveType.StraightBeforeTurn());
				const Point2d rightTurnPoint = Point2d(
					endOfStraightSegment + (m_Position.RadianDirection() - m_MoveType.TurnAngle()).ToVector2d() *
					m_MoveType.StraightBeforeTurn());
				const Point2d maxLeftTurnPoint = Point2d(
					leftTurnPoint + (m_Position.RadianDirection() + 2 * m_MoveType.TurnAngle()).ToVector2d() *
					distanceAfterSecondTurn);
				const Point2d maxRightTurnPoint = Point2d(
					rightTurnPoint + (m_Position.RadianDirection() - 2 * m_MoveType.TurnAngle()).ToVector2d() *
					distanceAfterSecondTurn);
				area.Add(leftTurnPoint);
				area.Add(maxLeftTurnPoint);
				area.Add(rightTurnPoint);
				area.Add(maxRightTurnPoint);

				area.Add(LineSeg(leftTurnPoint, rightTurnPoint));
				area.Add(LineSeg(leftTurnPoint, maxLeftTurnPoint));
				area.Add(LineSeg(rightTurnPoint, maxRightTurnPoint));

				area.Add(CircArc(leftTurnPoint, distanceAfterSecondTurn, Direction(m_MoveType.TurnAngle()),
				                 Direction(2 * m_MoveType.TurnAngle())));
				area.Add(CircArc(rightTurnPoint, distanceAfterSecondTurn, Direction(-m_MoveType.TurnAngle()),
				                 Direction(-2 * m_MoveType.TurnAngle())));
			}
			else
			{
				const Point2d leftCorner = Point2d(
					endOfStraightSegment + (m_Position.RadianDirection() + m_MoveType.TurnAngle()).ToVector2d() *
					distanceAfterFirstTurn);
				const Point2d rightCorner = Point2d(
					endOfStraightSegment + (m_Position.RadianDirection() - m_MoveType.TurnAngle()).ToVector2d() *
					distanceAfterFirstTurn);
				area.Add(leftCorner);
				area.Add(rightCorner);

				area.Add(LineSeg(leftCorner, rightCorner));
			}
			return area;
		}
		if (distanceAfterFirstTurn > 0)
		{
			const Point2d leftCorner = Point2d(
				endOfStraightSegment + (m_Position.RadianDirection() + m_MoveType.TurnAngle()).ToVector2d() *
				distanceAfterFirstTurn);
			const Point2d rightCorner = Point2d(
				endOfStraightSegment + (m_Position.RadianDirection() - m_MoveType.TurnAngle()).ToVector2d() *
				distanceAfterFirstTurn);
			area.Add(leftCorner);
			area.Add(rightCorner);

			area.Add(LineSeg(leftCorner, rightCorner));
			area.Add(CircArc(endOfStraightSegment, distanceAfterFirstTurn, Direction(-m_MoveType.TurnAngle()),
			                 Direction(m_MoveType.TurnAngle())));
		}
		return area;
	}
}
