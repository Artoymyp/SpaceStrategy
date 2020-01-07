#include "stdafx.h"
#include "CppUnitTest.h"
#include "ToString.h"
#include "Trajectory.h"
#include "Point2D.h"
#include "Position2D.h"
#include "fakeit.hpp"
#define _USE_MATH_DEFINES
#include <math.h>

using namespace Microsoft::VisualStudio::CppUnitTestFramework;
using namespace SpaceStrategy;
using namespace fakeit;
using namespace std;

namespace SSCoreTests
{
	TEST_CLASS(TrajectoryTests)
	{
	public:
		Mock<IMovementManager> movementManager;
		unique_ptr<Trajectory> trajectory;
	TEST_METHOD_INITIALIZE(InitializeTest)
		{
			trajectory = make_unique<Trajectory>(movementManager.get());
		}

	TEST_METHOD_CLEANUP(CleanupTest)
		{
			movementManager.Reset();
		}

		TEST_METHOD(TrajectoryNotMovePositionIfHasNoPoints)
		{
			//Setup
			AssertThatTrajectoryHasNoPoints();
			Position2d correctPosition;
			Position2d position = correctPosition;

			//Affect
			trajectory->MovePosition(position, 1);

			//Assert
			Assert::AreEqual(correctPosition, position);
		}

		TEST_METHOD(Trajectory_UpdatesDistanceAfterLastTurn_UpdatesPosition_AfterMoving)
		{
			//Setup
			float distance = 1;
			trajectory->AddSegment(Point2d(0, distance * 2));

			//Affect
			Position2d position;
			trajectory->MovePosition(position, distance);

			//Assert
			Assert::AreEqual(Position2d(Point2d(0, 1), Direction(M_PI_2)), position);
			Assert::AreEqual(distance, trajectory->DistanceAfterLastTurn());
		}

		TEST_METHOD(Trajectory_ResetsDistanceAfterLastTurn_UpdatesPosition_AfterMovingOverTurn)
		{
			//Setup
			trajectory->AddSegment(Point2d(3, 0));
			trajectory->AddSegment(Point2d(3, 4));

			//Affect
			Position2d position;
			trajectory->MovePosition(position, 4);

			//Assert
			Assert::AreEqual(Position2d(Point2d(3, 1), Direction(M_PI_2)), position);
			Assert::AreEqual(1.0f, trajectory->DistanceAfterLastTurn());
		}

		TEST_METHOD(TrajectoryMovesPositionNoFartherThanPossible)
		{
			//Setup
			float maxDistance = 1;
			trajectory->AddSegment(Point2d(0, maxDistance));

			//Affect
			trajectory->MovePosition(Position2d(), maxDistance * 2);

			//Assert
			Assert::AreEqual(maxDistance, trajectory->DistanceAfterLastTurn());
		}

		TEST_METHOD(Spaceship_MoveInAvailableArea)
		{
			//Assert::AreEqual(ship->Position(), correctPosition);
		}

		void AssertThatTrajectoryHasNoPoints()
		{
			Assert::IsTrue(trajectory->IsEmpty());
		}
	};
}
