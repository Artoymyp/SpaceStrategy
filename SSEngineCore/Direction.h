#pragma once
#include "Vector2d.h"
#include <iostream>

namespace SpaceStrategy{
	struct Direction
	{
	private:
		float m_precision = 0.001f;
		float m_direction;
		float ForceInRadianRange(float f) const;
	public:
		Direction(){ m_direction = 0; };
		Direction(const float& direction);
		~Direction();

		Vector2d ToVector2d() const;

		Direction operator+(const float& r) const;
		Direction operator-(const float& r) const; 
		bool operator==(const float& r) const;
		bool operator==(const Direction& r) const;

		bool operator!=(const float& r) const;
		bool operator!=(const Direction& r) const;

		friend std::wostream& operator<<(std::wostream& stream,
			const SpaceStrategy::Direction& dir) {
			stream << dir.m_direction << " radian";
			return stream;
		}
	};
}

