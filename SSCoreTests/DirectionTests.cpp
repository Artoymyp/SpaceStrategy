#include "stdafx.h"
#include "CppUnitTest.h"
#include "ToString.h"
#include "Direction.h"

#define _USE_MATH_DEFINES
#include "Math.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;
using namespace SpaceStrategy;

namespace SSCoreTests
{
	TEST_CLASS(DirectionTests)
	{
	public:
		TEST_METHOD(DirectionPlusFloat)
		{
			Assert::AreEqual(Direction(M_PI / 4.0), Direction(M_PI / 4.0 * 7.0) + M_PI_2);
		}

		TEST_METHOD(DirectionConstruction)
		{
			Assert::IsTrue(Direction() == 0);
		}

		TEST_METHOD(DirectionEqualTest)
		{
			float angle = 1;
			Direction dir1 = Direction(angle);
			Direction dir2 = Direction(angle);
			Assert::IsTrue(dir1 == dir2);
		}

		TEST_METHOD(DirectionEqualToFloatTest)
		{
			float dir = 1;
			Direction direction = Direction(dir + 2 * M_PI);
			Assert::IsTrue(direction == dir);
			Assert::IsTrue(direction == dir + 2 * M_PI);
			Assert::IsTrue(direction == dir + 4 * M_PI);
			Assert::IsTrue(direction == dir - 2 * M_PI);
		}

		TEST_METHOD(DirectionToVectorTest)
		{
			Assert::IsTrue(Direction(0).ToVector2d() == Vector2d(1, 0));
			Assert::IsTrue(Direction(M_PI_2).ToVector2d() == Vector2d(0, 1));
		}
	};
}
