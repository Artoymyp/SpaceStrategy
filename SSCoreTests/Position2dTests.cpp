#include "stdafx.h"
#include "CppUnitTest.h"
#include "ToString.h"
#include "Point2d.h"
#include "Position2d.h"
#include "Direction.h"

#define _USE_MATH_DEFINES
#include "Math.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;
using namespace SpaceStrategy;

namespace SSCoreTests
{
	TEST_CLASS(Position2dTests)
	{
	public:
		TEST_METHOD(PositionGetTranslated)
		{
			auto initialPoint = Point2d(1, 0);
			auto initialDir = Direction(M_PI_2);
			auto translateDir = Direction(0);
			float translateDist = 1;

			auto correctPos = Position2d(initialPoint + translateDir.ToVector2d() * translateDist, initialDir);
			auto pos = Position2d(initialPoint, initialDir);

			Assert::AreEqual(correctPos, pos.GetTranslated(translateDist, translateDir), L"Translation incorrect.");
			Assert::AreEqual(Position2d(initialPoint, initialDir), pos, L"Source position should not change.");
		}

		TEST_METHOD(PositionMoveAlongDirection)
		{
			Position2d position = Position2d(Point2d(), Direction());
			Position2d correctPosition = Position2d(Point2d(1, 0), Direction());
			position.MoveAlongDirection(1.0);
			Assert::IsTrue(position == correctPosition);
		}

		TEST_METHOD(PositionEqualTest)
		{
			Point2d location = Point2d(1, 1);
			float direction = 1;
			Position2d pos1 = Position2d(location, direction);
			Position2d pos2 = Position2d(location, direction);
			Assert::IsTrue(pos1 == pos2);
		}

		TEST_METHOD(PositionLocationTest)
		{
			Point2d location = Point2d(2, 1);
			Position2d position = Position2d(location);
			Assert::IsTrue(position.Location() == location, L"Location is wrong.");
		}

		TEST_METHOD(PositionDirectionTest)
		{
			float direction = 1;
			Position2d position = Position2d(Point2d(), direction);
			Assert::IsTrue(position.RadianDirection() == direction, L"Direction is wrong.");
		}
	};
}
