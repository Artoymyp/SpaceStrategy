#include "stdafx.h"
#include "CppUnitTest.h"
#include "Vector2d.h"
#include "Position2d.h"
#include "LineSeg.h"
#include "CircArc.h"
#include "Direction.h"

using namespace SpaceStrategy;
namespace Microsoft{
	namespace VisualStudio {
		namespace CppUnitTestFramework {
			
			template<>
			static std::wstring ToString<Vector2d>(const Vector2d  & v) {
				std::wstringstream ss;
				ss << "(" << v.X() << ";" << v.Y() << ")";
				return ss.str();
			}
			
			template<>
			static std::wstring ToString<Point2d>(const Point2d  & p) {
				std::wstringstream ss;
				ss << ToString(p.ToVector());
				return ss.str();
			}
			
			template<>
			static std::wstring ToString<LineSeg>(const LineSeg  & s) {
				std::wstringstream ss;
				ss << "(" << ToString(s.Start()) << ";" << ToString(s.End()) << ")";
				return ss.str();
			}

			template<>
			static std::wstring ToString<Position2d>(const Position2d  & pos) {
				std::wstringstream ss;
				ss << ToString(pos.Location()) << "; " << pos.RadianDirection();
				return ss.str();
			}

			template<>
			static std::wstring ToString<CircArc>(const CircArc  & c) {
				std::wstringstream ss;
				ss << "(C: " << ToString(c.Center()) << "; R: " << c.Radius() << "; StartDir: " << c.StartDir() << "; EndDir: " << c.EndDir() << ")";
				return ss.str();
			}

			template<>
			static std::wstring ToString<Direction>(const Direction  & d) {
				std::wstringstream ss;
				ss << ToString(d.ToVector2d());
				return ss.str();
			}
		}
	}
}