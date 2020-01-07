#pragma once
#include <math.h>
#include "Common.h"

namespace SpaceStrategy
{
	struct Vector2d
	{
	public:
		Vector2d();
		Vector2d(float x, float y);
		~Vector2d();

	PROPERTY(float, X);
	PROPERTY(float, Y);

		Vector2d Normalize() const;
		float Length() const;

		bool operator==(const Vector2d& right) const;
		bool operator!=(const Vector2d& right) const;
		float ToRadian() const;
	};

	static Vector2d operator *(Vector2d l, float r)
	{
		return Vector2d(l.X() * r, l.Y() * r);
	}

	static Vector2d operator *(float l, Vector2d r)
	{
		return r * l;
	}

	static Vector2d operator +(Vector2d l, Vector2d r)
	{
		return Vector2d(l.X() + r.X(), l.Y() + r.Y());
	}

	/*
	static Vector operator /(Vector l, double r)
	{
	return l*(1 / r);
	}
	static Vector operator -(Vector l, Vector r)
	{
	return new Vector(l.X - r.X, l.Y - r.Y);
	}
	Point2d ToPoint2d()
	{
	return new Point2d(X, Y);
	}
	override string ToString()
	{
	return string.Format("({0}; {1})", X, Y);
	}*/
}
