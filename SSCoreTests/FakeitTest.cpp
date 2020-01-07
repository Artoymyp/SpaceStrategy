#include "stdafx.h"
#include "CppUnitTest.h"
#include <memory>
#include <fakeit.hpp>

using namespace Microsoft::VisualStudio::CppUnitTestFramework;
using namespace fakeit;

namespace SSCoreTests
{
	TEST_CLASS(FakeitTest)
	{
		class Product
		{
		};

		class IFactory
		{
		public:
			virtual std::unique_ptr<Product> Create() = 0;
		};

		class IFactoryMock : public IFactory
		{
		public:
			virtual Product* CreateRaw() { return nullptr; };

			std::unique_ptr<Product> Create() override
			{
				return std::unique_ptr<Product>(CreateRaw());
			}
		};

	public:

		TEST_METHOD(TestMethod1)
		{
			IFactoryMock factory;
			fakeit::Mock<IFactoryMock> spy1(factory);
			When(Method(spy1, CreateRaw)).Return(new Product());
			auto a = spy1.get().Create();
			Verify(Method(spy1, CreateRaw)).Once();
		}
	};
}
