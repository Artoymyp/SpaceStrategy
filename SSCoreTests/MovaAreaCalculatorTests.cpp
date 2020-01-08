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
			Position2d position(Point2d(3, 4), 0);
			MovementDescriptor movement_descriptor;

			auto area = moveAreaCalculator->GetMoveArea(0, movement_descriptor, position);
			Assert::AreEqual(size_t(0), area.AnchorPoints().size());
			Assert::AreEqual(size_t(0), area.LineSegments().size());
			Assert::AreEqual(size_t(0), area.CircleArcs().size());
		}

		TEST_METHOD(Spaceship_GetMoveArea_OnlyStraightMovement)
		{
			float straightDist = 3;
			float maxDist = 2;

			//Setup
			Position2d position(Point2d(3, 4), 0);
			MovementDescriptor movement_descriptor;
			{
				movement_descriptor.Distance(maxDist);
				movement_descriptor.StraightBeforeTurn(straightDist);
			}

			//Assert
			auto area = moveAreaCalculator->GetMoveArea(0, movement_descriptor, position);
			Assert::AreEqual(size_t(1), area.AnchorPoints().size());
			Assert::AreEqual(size_t(1), area.LineSegments().size());
			Assert::AreEqual(size_t(0), area.CircleArcs().size());

			Point2d endOfStraightSegment = position.GetTranslated(
				std::fmin(movement_descriptor.StraightBeforeTurn(), movement_descriptor.Distance()))
				.Location();
			Assert::AreEqual(endOfStraightSegment, area.AnchorPoints()[0]);

			Assert::AreEqual(LineSeg(position.Location(), endOfStraightSegment), area.LineSegments()[0]);
		}

		TEST_METHOD(Spaceship_GetMoveArea_TurnWithoutStraightMovement)
		{
			float straightDist = 0;
			float maxDist = 2;
			float maxTurnAngle = M_PI_2;
			int maxTurnCount = 1;

			//Setup
			Point2d location = Point2d(3, 4);
			Position2d position(location, 0);
			MovementDescriptor movement_descriptor;
			{
				movement_descriptor.Distance(maxDist);
				movement_descriptor.StraightBeforeTurn(straightDist);
				movement_descriptor.TurnAngle(maxTurnAngle);
				movement_descriptor.TurnCount(maxTurnCount);
			}

			//Assert
			auto area = moveAreaCalculator->GetMoveArea(0, movement_descriptor, position);
			Assert::AreEqual(size_t(2), area.AnchorPoints().size(), L"Wrong anchor points count.");
			Assert::AreEqual(size_t(1), area.LineSegments().size(), L"Wrong line segments count.");
			Assert::AreEqual(size_t(1), area.CircleArcs().size(), L"Wrong circ arcs count.");

			Point2d turnStart = location;
			Point2d leftCorner = Point2d(3, 6);
			Point2d rightCorner = Point2d(3, 2);
			Assert::AreEqual(leftCorner, area.AnchorPoints()[0]);
			Assert::AreEqual(rightCorner, area.AnchorPoints()[1]);

			Assert::AreEqual(LineSeg(leftCorner, rightCorner), area.LineSegments()[0]);

			Assert::AreEqual(CircArc(location, maxDist, Direction(-maxTurnAngle), Direction(maxTurnAngle)),
				area.CircleArcs()[0]);
		}

		TEST_METHOD(Spaceship_GetMoveArea_StraightWithTurnMovement)
		{
			float straightDist = 1;
			float maxDist = 2;
			float maxTurnAngle = M_PI_2;
			float maxTurnCount = 1;

			//Setup
			Point2d location = Point2d(3, 4);
			Position2d position(location, 0);
			MovementDescriptor movement_descriptor;
			{
				movement_descriptor.Distance(maxDist);
				movement_descriptor.StraightBeforeTurn(straightDist);
				movement_descriptor.TurnAngle(maxTurnAngle);
				movement_descriptor.TurnCount(maxTurnCount);
			}

			//Assert
			auto area = moveAreaCalculator->GetMoveArea(0, movement_descriptor, position);
			Assert::AreEqual(size_t(3), area.AnchorPoints().size(), L"Wrong anchor points count.");
			Assert::AreEqual(size_t(2), area.LineSegments().size(), L"Wrong line segments count.");
			Assert::AreEqual(size_t(1), area.CircleArcs().size(), L"Wrong circ arcs count.");

			Point2d turnStart = Point2d(4, 4);
			Point2d leftCorner = Point2d(4, 5);
			Point2d rightCorner = Point2d(4, 3);
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
			float straightDist = 1;
			float maxDist = 2;
			float maxTurnAngle = M_PI_2;
			int maxTurnCount = 2;

			//Setup
			Position2d position(Point2d(4, 4), 0);
			MovementDescriptor movement_descriptor;
			{
				movement_descriptor.Distance(maxDist);
				movement_descriptor.StraightBeforeTurn(straightDist);
				movement_descriptor.TurnAngle(maxTurnAngle);
				movement_descriptor.TurnCount(maxTurnCount);
			}

			//Assert
			auto area = moveAreaCalculator->GetMoveArea(straightDist, movement_descriptor, position);

			Assert::AreEqual(size_t(4), area.AnchorPoints().size(), L"Wrong anchor points count.");
			Assert::AreEqual(size_t(3), area.LineSegments().size(), L"Wrong line segments count.");
			Assert::AreEqual(size_t(3), area.CircleArcs().size(), L"Wrong circ arcs count.");

			Point2d turnStart = Point2d(4, 4);
			Point2d leftTurnCorner = Point2d(4, 5);
			Point2d maxLeftMovePoint = Point2d(3, 5);
			Point2d rightTurnCorner = Point2d(4, 3);
			Point2d maxRightMovePoint = Point2d(3, 3);
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
			float straightDist = 1;
			float maxDist = 3;
			float maxTurnAngle = M_PI_2;
			int maxTurnCount = 2;

			//Setup
			Point2d location = Point2d(3, 4);
			Position2d position(location, 0);
			MovementDescriptor movement_descriptor;
			{
				movement_descriptor.Distance(maxDist);
				movement_descriptor.StraightBeforeTurn(straightDist);
				movement_descriptor.TurnAngle(maxTurnAngle);
				movement_descriptor.TurnCount(maxTurnCount);
			}

			//Assert
			auto area = moveAreaCalculator->GetMoveArea(0, movement_descriptor, position);

			Assert::AreEqual(size_t(5), area.AnchorPoints().size(), L"Wrong anchor points count.");
			Assert::AreEqual(size_t(4), area.LineSegments().size(), L"Wrong line segments count.");
			Assert::AreEqual(size_t(3), area.CircleArcs().size(), L"Wrong circ arcs count.");

			Point2d turnStart = Point2d(4, 4);
			Point2d leftTurnCorner = Point2d(4, 5);
			Point2d maxLeftMovePoint = Point2d(3, 5);
			Point2d rightTurnCorner = Point2d(4, 3);
			Point2d maxRightMovePoint = Point2d(3, 3);
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