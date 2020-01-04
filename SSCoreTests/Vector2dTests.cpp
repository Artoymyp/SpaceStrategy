#include "stdafx.h"
#include "CppUnitTest.h"
#include "Vector2d.h"
#include "ToString.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;
using namespace SpaceStrategy;

namespace SSCoreTests
{		
	TEST_CLASS(Vector2dTests)
	{
	public:		
		TEST_METHOD(VectorNormalizeTest){
			float x = 1;
			float y = 2;
			Vector2d v1 = Vector2d(x, y);
			float nx = x / v1.Length();
			float ny = y / v1.Length();
			Assert::AreEqual(v1.Normalize(), Vector2d(nx, ny));
		}
		TEST_METHOD(VectorAddTest){
			Assert::AreEqual(Vector2d(1,0) + Vector2d(0,1), Vector2d(1,1));
		}

		TEST_METHOD(VectorFloatMultiplicationTest){
			float x = 1;
			float y = 2;
			float c = 3;
			
			Vector2d vUnchanged = Vector2d(x, y);
			Vector2d vCorrect = Vector2d(x*c, y*c);
			Vector2d vTested = Vector2d(x, y);

			Vector2d vPostMultiplied = vTested*c;
			Assert::IsTrue(vPostMultiplied == vCorrect);
			Assert::IsTrue(vTested == vUnchanged);

			Vector2d vPreMultiplied = c*vTested;
			Assert::IsTrue(vPreMultiplied == vCorrect);
			Assert::IsTrue(vTested == vUnchanged);
		}
		TEST_METHOD(VectorComponentsTest){
			float y = 12.5;
			float x = 10.5;
			Vector2d v = Vector2d(x, y);

			Assert::AreEqual(x, v.X(), L"X component is wrong.");
			Assert::AreEqual(y, v.Y(), L"Y component is wrong.");
		}

		TEST_METHOD(VectorLengthTest){
			float y = 3;
			float x = 4;
			Vector2d v = Vector2d(x, y);
			float length = 5;

			Assert::AreEqual(length, v.Length(), L"Length is wrong.");
		}

		TEST_METHOD(VectorsEqualUnequalTest)
		{
			float x = 10;
			float y = 11.5;
			float y1 = 12.5;
			float x1 = 10.5;

			Vector2d v11= Vector2d(x,y);
			Vector2d v12 = Vector2d(x, y);
			Vector2d v21 = Vector2d(x1, y);
			Vector2d v22 = Vector2d(x, y1);
			Vector2d v23 = Vector2d(x1, y1);
			
			Assert::IsTrue(v11 == v12);
			Assert::IsFalse(v11 == v21);
			Assert::IsFalse(v11 == v22);
			Assert::IsFalse(v11 == v23);

			Assert::IsFalse(v11 != v12);
			Assert::IsTrue(v11 != v21);
			Assert::IsTrue(v11 != v22);
			Assert::IsTrue(v11 != v23);
		}

	};
}