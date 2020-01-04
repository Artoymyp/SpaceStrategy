#include "stdafx.h"
#include "CppUnitTest.h"
#include "ToString.h"
#include "Position2D.h"
#include "Direction.h"
#include "Spaceship.h"
#include "Trajectory.h"
#include "LineSeg.h"
#include <vector>
#define _USE_MATH_DEFINES
#include "Math.h"
#include "MovementDescriptor.h"
#include <memory>

#include "fakeit.hpp"

using namespace fakeit;


using namespace Microsoft::VisualStudio::CppUnitTestFramework;
using namespace SpaceStrategy;

namespace SpaceStrategy{
	//class IMovementManagerMock : public IMovementManager {
	//private:
	//	Trajectory* m_createdTrajectory;
	//public:
	//	virtual std::unique_ptr<ITrajectory> CreateTrajectory() override
	//	{
	//		return std::make_unique<Trajectory>(*this);
	//	}
	//	Trajectory& CreatedTrajectory(){ return *m_createdTrajectory; }
	//};
}
namespace SSCoreTests
{
	
	TEST_CLASS(SpaceshipTests)
	{
	private:
	public:
		SpaceshipTests() 
		{

		}
		TEST_METHOD_INITIALIZE(InitializeTest)
		{			
		}

		TEST_METHOD_CLEANUP(CleanupTest)
		{

		}



	};

}