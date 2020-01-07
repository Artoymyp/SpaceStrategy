#include "stdafx.h"
#include "CppUnitTest.h"
#include "Point2d.h"
#include "ToString.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;
using namespace SpaceStrategy;

namespace SSCoreTests
{
	TEST_CLASS(Point2dTests)
	{
	public:
		TEST_METHOD(PointDistanceToPointTest)
		{
			Assert::AreEqual(Point2d(1, 1).DistanceTo(Point2d(4, 5)), 5.0f);
		}

		TEST_METHOD(PointComponentsTest)
		{
			float y = 12.5;
			float x = 10.5;
			Point2d v = Point2d(x, y);

			Assert::AreEqual(x, v.X(), L"X component is wrong.");
			Assert::AreEqual(y, v.Y(), L"Y component is wrong.");
		}

		TEST_METHOD(PointsEqualUnequalTest)
		{
			float x = 10;
			float y = 11.5;
			float y1 = 12.5;
			float x1 = 10.5;

			Point2d p11 = Point2d(x, y);
			Point2d p12 = Point2d(x, y);
			Point2d p21 = Point2d(x1, y);
			Point2d p22 = Point2d(x, y1);
			Point2d p23 = Point2d(x1, y1);

			Assert::IsTrue(p11 == p12);
			Assert::IsFalse(p11 == p21);
			Assert::IsFalse(p11 == p22);
			Assert::IsFalse(p11 == p23);

			Assert::IsFalse(p11 != p12);
			Assert::IsTrue(p11 != p21);
			Assert::IsTrue(p11 != p22);
			Assert::IsTrue(p11 != p23);
		}

		TEST_METHOD(PointAddVector)
		{
			Assert::IsTrue(Point2d(1, 2) + Vector2d(1, 2) == Point2d(2, 4));
		}
	};
}
