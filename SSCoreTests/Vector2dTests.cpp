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
		TEST_METHOD(VectorNormalizeTest)
		{
			const float x = 1;
			const float y = 2;
			const Vector2d v1 = Vector2d(x, y);
			const float nx = x / v1.Length();
			const float ny = y / v1.Length();
			Assert::AreEqual(v1.Normalize(), Vector2d(nx, ny));
		}

		TEST_METHOD(VectorAddTest)
		{
			Assert::AreEqual(Vector2d(1, 0) + Vector2d(0, 1), Vector2d(1, 1));
		}

		TEST_METHOD(VectorFloatMultiplicationTest)
		{
			const float x = 1;
			const float y = 2;
			const float c = 3;

			const Vector2d vUnchanged = Vector2d(x, y);
			const Vector2d vCorrect = Vector2d(x * c, y * c);
			const Vector2d vTested = Vector2d(x, y);

			const Vector2d vPostMultiplied = vTested * c;
			Assert::IsTrue(vPostMultiplied == vCorrect);
			Assert::IsTrue(vTested == vUnchanged);

			const Vector2d vPreMultiplied = c * vTested;
			Assert::IsTrue(vPreMultiplied == vCorrect);
			Assert::IsTrue(vTested == vUnchanged);
		}

		TEST_METHOD(VectorComponentsTest)
		{
			const float y = 12.5;
			const float x = 10.5;
			const Vector2d v = Vector2d(x, y);

			Assert::AreEqual(x, v.X(), L"X component is wrong.");
			Assert::AreEqual(y, v.Y(), L"Y component is wrong.");
		}

		TEST_METHOD(VectorLengthTest)
		{
			const float y = 3;
			const float x = 4;
			const Vector2d v = Vector2d(x, y);
			const float length = 5;

			Assert::AreEqual(length, v.Length(), L"Length is wrong.");
		}

		TEST_METHOD(VectorsEqualUnequalTest)
		{
			const float x = 10;
			const float y = 11.5;
			const float y1 = 12.5;
			const float x1 = 10.5;

			const Vector2d v11 = Vector2d(x, y);
			const Vector2d v12 = Vector2d(x, y);
			const Vector2d v21 = Vector2d(x1, y);
			const Vector2d v22 = Vector2d(x, y1);
			const Vector2d v23 = Vector2d(x1, y1);

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
