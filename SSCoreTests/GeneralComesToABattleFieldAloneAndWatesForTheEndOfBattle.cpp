#include "stdafx.h"
#include "CppUnitTest.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace SSCoreTests
{
	class IGameServer
	{
	public:
		virtual void StartMatch() = 0;
		virtual void EndMatch() = 0;
		virtual void AddPlayer() = 0;
	};

	class IGameDriver
	{
	};

	TEST_CLASS(EndToEndTests)
	{
	public:

		TEST_METHOD(GeneralComesToABattleFieldAloneAndWaitsForTheEndOfBattle)
		{
		}
	};
}
