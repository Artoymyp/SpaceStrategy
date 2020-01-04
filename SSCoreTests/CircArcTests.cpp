#pragma once
#include "stdafx.h"
#include "CppUnitTest.h"
#include "Point2D.h"
#include "CircArc.h"
#include "ToString.h"
#define _USE_MATH_DEFINES
#include "Math.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;
using namespace SpaceStrategy;

namespace SSCoreTests
{
	TEST_CLASS(CircArcTests)
	{
	public:
		TEST_METHOD(CCWCircArcTest){
			auto c = CircArc(Point2d(3, 4), 2, Direction(M_PI_2), Direction(M_PI));
			Assert::AreEqual(Point2d(3, 6), c.GetStartPoint(), L"StartPoint error.");
			Assert::AreEqual(Point2d(1, 4), c.GetEndPoint(), L"EndPoint error.");
		}
		TEST_METHOD(CWCircArcTest){
			auto c = CircArc(Point2d(3, 4), 2, Direction(M_PI), Direction(M_PI_2));
			Assert::AreEqual(Point2d(1, 4), c.GetStartPoint(), L"StartPoint error.");
			Assert::AreEqual(Point2d(3, 6), c.GetEndPoint(), L"EndPoint error.");
		}
	};
}

