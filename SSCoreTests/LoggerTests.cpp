#include "stdafx.h"
#include "CppUnitTest.h"
#include "ToString.h"
#include "Trajectory.h"
#include "Point2D.h"
#include "Position2D.h"
#include "fakeit.hpp"
#include <string.h>
#define _USE_MATH_DEFINES
#include <math.h>
#include <thread>
#include <iostream>
#include <fstream>

using namespace Microsoft::VisualStudio::CppUnitTestFramework;
using namespace SpaceStrategy;
using namespace fakeit;
using namespace std;

namespace SSCoreTests
{
	TEST_CLASS(EventReaderTests)
	{
	private:
		class EventSource
		{
			class EventGeneratorApplication
			{
			public:
				void main()
				{
					std::ofstream my_file;
					my_file.open("log.txt");
					my_file << "Writing this to a file.\n";
					my_file.close();
				}
			};

		public:
			void GenerateEvent()
			{
				std::thread t(&EventGeneratorApplication::main, EventGeneratorApplication());
				t.join();
			}
		};

		typedef string LogEvent;

		class LogEventReader
		{
		public:
			LogEvent GetEvent(string logFilename, int timeout_in_ms)
			{
				ifstream logFile(logFilename);
				if (!logFile.is_open())
				{
					throw exception("No log file found.");
				}

				string line;
				string tempLine;
				while (std::getline(logFile, line))
				{
					tempLine = line;
				}
				logFile.close();
				return tempLine;
			}
		};

		LogEventReader eventReader;
		EventSource eventSource;

		void EventReaderHasReadEvent()
		{
			try
			{
				auto s = eventReader.GetEvent("log.txt", 20);
				Assert::IsTrue(s.length() > 0, L"No log event was read.");
			}
			catch (exception ex)
			{
				Assert::Fail(GetWC(ex.what()));
			}
		}

		const wchar_t* GetWC(const char* c)
		{
			const size_t cSize = strlen(c) + 1;
			wchar_t* wc = new wchar_t[cSize];
			mbstowcs(wc, c, cSize);

			return wc;
		}

	public:
		TEST_METHOD_INITIALIZE(InitializeTest)
		{
		}

		TEST_METHOD_CLEANUP(CleanupTest)
		{
		}

		TEST_METHOD(EventReaderGetsEvent)
		{
			eventSource.GenerateEvent();
			EventReaderHasReadEvent();
		}
	};
}