#include "stdafx.h"
#include "CppUnitTest.h"
#include "Point2D.h"
#include "Position2D.h"
#include <MovementDescriptor.h>
#include "MoveAreaCalculator.h"
#define _USE_MATH_DEFINES
#include <math.h>
#include "ToString.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;
using namespace SpaceStrategy;
using namespace std;

namespace SSCoreTests
{
	TEST_CLASS(MovaAreaCalculatorTests)
	{
	public:
		unique_ptr<MoveAreaCalculator> moveAreaCalculator;
		TEST_METHOD_INITIALIZE(InitializeTest)
		{
			moveAreaCalculator = make_unique<MoveAreaCalculator>();
		}

		TEST_METHOD(Spaceship_GetMoveArea_NoAvailableMovement)
		{
			const Position2d position(Point2d(3, 4), 0);
			const MovementDescriptor movement_descriptor;

			const auto area = moveAreaCalculator->GetMoveArea(0, movement_descriptor, position);
			Assert::AreEqual(size_t(0), area.AnchorPoints().size());
			Assert::AreEqual(size_t(0), area.LineSegments().size());
			Assert::AreEqual(size_t(0), area.CircleArcs().size());
		}

		TEST_METHOD(Spaceship_GetMoveArea_OnlyStraightMovement)
		{
			const float straightDist = 3;
			const float maxDist = 2;

			//Setup
			const Position2d position(Point2d(3, 4), 0);
			MovementDescriptor movement_descriptor;
			{
				movement_descriptor.Distance(maxDist);
				movement_descriptor.StraightBeforeTurn(straightDist);
			}

			//Assert
			const auto area = moveAreaCalculator->GetMoveArea(0, movement_descriptor, position);
			Assert::AreEqual(size_t(1), area.AnchorPoints().size());
			Assert::AreEqual(size_t(1), area.LineSegments().size());
			Assert::AreEqual(size_t(0), area.CircleArcs().size());

			const Point2d endOfStraightSegment = position.GetTranslated(
				                                             std::fmin(movement_descriptor.StraightBeforeTurn(), movement_descriptor.Distance()))
			                                             .Location();
			Assert::AreEqual(endOfStraightSegment, area.AnchorPoints()[0]);

			Assert::AreEqual(LineSeg(position.Location(), endOfStraightSegment), area.LineSegments()[0]);
		}

		TEST_METHOD(Spaceship_GetMoveArea_TurnWithoutStraightMovement)
		{
			const float straightDist = 0;
			const float maxDist = 2;
			const float maxTurnAngle = M_PI_2;
			const int maxTurnCount = 1;

			//Setup
			const Point2d location = Point2d(3, 4);
			const Position2d position(location, 0);
			MovementDescriptor movement_descriptor;
			{
				movement_descriptor.Distance(maxDist);
				movement_descriptor.StraightBeforeTurn(straightDist);
				movement_descriptor.TurnAngle(maxTurnAngle);
				movement_descriptor.TurnCount(maxTurnCount);
			}

			//Assert
			const auto area = moveAreaCalculator->GetMoveArea(0, movement_descriptor, position);
			Assert::AreEqual(size_t(2), area.AnchorPoints().size(), L"Wrong anchor points count.");
			Assert::AreEqual(size_t(1), area.LineSegments().size(), L"Wrong line segments count.");
			Assert::AreEqual(size_t(1), area.CircleArcs().size(), L"Wrong circ arcs count.");

			Point2d turnStart = location;
			const Point2d leftCorner = Point2d(3, 6);
			const Point2d rightCorner = Point2d(3, 2);
			Assert::AreEqual(leftCorner, area.AnchorPoints()[0]);
			Assert::AreEqual(rightCorner, area.AnchorPoints()[1]);

			Assert::AreEqual(LineSeg(leftCorner, rightCorner), area.LineSegments()[0]);

			Assert::AreEqual(CircArc(location, maxDist, Direction(-maxTurnAngle), Direction(maxTurnAngle)),
				area.CircleArcs()[0]);
		}

		TEST_METHOD(Spaceship_GetMoveArea_StraightWithTurnMovement)
		{
			const float straightDist = 1;
			const float maxDist = 2;
			const float maxTurnAngle = M_PI_2;
			const int maxTurnCount = 1;

			//Setup
			const Point2d location = Point2d(3, 4);
			const Position2d position(location, 0);
			MovementDescriptor movement_descriptor;
			{
				movement_descriptor.Distance(maxDist);
				movement_descriptor.StraightBeforeTurn(straightDist);
				movement_descriptor.TurnAngle(maxTurnAngle);
				movement_descriptor.TurnCount(maxTurnCount);
			}

			//Assert
			const auto area = moveAreaCalculator->GetMoveArea(0, movement_descriptor, position);
			Assert::AreEqual(size_t(3), area.AnchorPoints().size(), L"Wrong anchor points count.");
			Assert::AreEqual(size_t(2), area.LineSegments().size(), L"Wrong line segments count.");
			Assert::AreEqual(size_t(1), area.CircleArcs().size(), L"Wrong circ arcs count.");

			const Point2d turnStart = Point2d(4, 4);
			const Point2d leftCorner = Point2d(4, 5);
			const Point2d rightCorner = Point2d(4, 3);
			Assert::AreEqual(turnStart, area.AnchorPoints()[0]);
			Assert::AreEqual(leftCorner, area.AnchorPoints()[1]);
			Assert::AreEqual(rightCorner, area.AnchorPoints()[2]);

			Assert::AreEqual(LineSeg(location, turnStart), area.LineSegments()[0]);
			Assert::AreEqual(LineSeg(leftCorner, rightCorner), area.LineSegments()[1]);

			Assert::AreEqual(
				CircArc(turnStart, maxDist - straightDist, Direction(-maxTurnAngle), Direction(maxTurnAngle)),
				area.CircleArcs()[0]);
		}

		TEST_METHOD(Spaceship_GetMoveArea_DoubleTurnWithoutStraightMovement)
		{
			const float straightDist = 1;
			const float maxDist = 2;
			const float maxTurnAngle = M_PI_2;
			const int maxTurnCount = 2;

			//Setup
			const Position2d position(Point2d(4, 4), 0);
			MovementDescriptor movement_descriptor;
			{
				movement_descriptor.Distance(maxDist);
				movement_descriptor.StraightBeforeTurn(straightDist);
				movement_descriptor.TurnAngle(maxTurnAngle);
				movement_descriptor.TurnCount(maxTurnCount);
			}

			//Assert
			const auto area = moveAreaCalculator->GetMoveArea(straightDist, movement_descriptor, position);

			Assert::AreEqual(size_t(4), area.AnchorPoints().size(), L"Wrong anchor points count.");
			Assert::AreEqual(size_t(3), area.LineSegments().size(), L"Wrong line segments count.");
			Assert::AreEqual(size_t(3), area.CircleArcs().size(), L"Wrong circ arcs count.");

			const Point2d turnStart = Point2d(4, 4);
			const Point2d leftTurnCorner = Point2d(4, 5);
			const Point2d maxLeftMovePoint = Point2d(3, 5);
			const Point2d rightTurnCorner = Point2d(4, 3);
			const Point2d maxRightMovePoint = Point2d(3, 3);
			Assert::AreEqual(leftTurnCorner, area.AnchorPoints()[0]);
			Assert::AreEqual(maxLeftMovePoint, area.AnchorPoints()[1]);
			Assert::AreEqual(rightTurnCorner, area.AnchorPoints()[2]);
			Assert::AreEqual(maxRightMovePoint, area.AnchorPoints()[3]);

			Assert::AreEqual(LineSeg(leftTurnCorner, rightTurnCorner), area.LineSegments()[0]);
			Assert::AreEqual(LineSeg(leftTurnCorner, maxLeftMovePoint), area.LineSegments()[1]);
			Assert::AreEqual(LineSeg(rightTurnCorner, maxRightMovePoint), area.LineSegments()[2]);

			Assert::AreEqual(CircArc(turnStart, maxDist, Direction(-maxTurnAngle), Direction(maxTurnAngle)),
				area.CircleArcs()[0]);
			Assert::AreEqual(CircArc(leftTurnCorner, maxDist - straightDist, Direction(maxTurnAngle),
				Direction(2 * maxTurnAngle)), area.CircleArcs()[1]);
			Assert::AreEqual(CircArc(rightTurnCorner, maxDist - straightDist, Direction(-maxTurnAngle),
				Direction(-2 * maxTurnAngle)), area.CircleArcs()[2]);
		}

		TEST_METHOD(Spaceship_GetMoveArea_StraightWithDoubleTurnMovement)
		{
			const float straightDist = 1;
			const float maxDist = 3;
			const float maxTurnAngle = M_PI_2;
			const int maxTurnCount = 2;

			//Setup
			const Point2d location = Point2d(3, 4);
			const Position2d position(location, 0);
			MovementDescriptor movement_descriptor;
			{
				movement_descriptor.Distance(maxDist);
				movement_descriptor.StraightBeforeTurn(straightDist);
				movement_descriptor.TurnAngle(maxTurnAngle);
				movement_descriptor.TurnCount(maxTurnCount);
			}

			//Assert
			const auto area = moveAreaCalculator->GetMoveArea(0, movement_descriptor, position);

			Assert::AreEqual(size_t(5), area.AnchorPoints().size(), L"Wrong anchor points count.");
			Assert::AreEqual(size_t(4), area.LineSegments().size(), L"Wrong line segments count.");
			Assert::AreEqual(size_t(3), area.CircleArcs().size(), L"Wrong circ arcs count.");

			const Point2d turnStart = Point2d(4, 4);
			const Point2d leftTurnCorner = Point2d(4, 5);
			const Point2d maxLeftMovePoint = Point2d(3, 5);
			const Point2d rightTurnCorner = Point2d(4, 3);
			const Point2d maxRightMovePoint = Point2d(3, 3);
			Assert::AreEqual(turnStart, area.AnchorPoints()[0]);
			Assert::AreEqual(leftTurnCorner, area.AnchorPoints()[1]);
			Assert::AreEqual(maxLeftMovePoint, area.AnchorPoints()[2]);
			Assert::AreEqual(rightTurnCorner, area.AnchorPoints()[3]);
			Assert::AreEqual(maxRightMovePoint, area.AnchorPoints()[4]);

			Assert::AreEqual(LineSeg(location, turnStart), area.LineSegments()[0]);
			Assert::AreEqual(LineSeg(leftTurnCorner, rightTurnCorner), area.LineSegments()[1]);
			Assert::AreEqual(LineSeg(leftTurnCorner, maxLeftMovePoint), area.LineSegments()[2]);
			Assert::AreEqual(LineSeg(rightTurnCorner, maxRightMovePoint), area.LineSegments()[3]);

			Assert::AreEqual(area.CircleArcs()[0],
				CircArc(turnStart, maxDist - straightDist, Direction(-maxTurnAngle),
					Direction(maxTurnAngle)));
			Assert::AreEqual(area.CircleArcs()[1], CircArc(leftTurnCorner, maxDist - 2 * straightDist,
				Direction(maxTurnAngle), Direction(2 * maxTurnAngle)));
			Assert::AreEqual(area.CircleArcs()[2], CircArc(rightTurnCorner, maxDist - 2 * straightDist,
				Direction(-maxTurnAngle), Direction(-2 * maxTurnAngle)));
		}
	};
}