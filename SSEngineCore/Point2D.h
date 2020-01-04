#pragma once

#include "Common.h"
#include "Vector2d.h"
#include "Direction.h"

namespace SpaceStrategy{
	struct Point2d
	{
	public:
		Point2d();
		Point2d(float x, float y);
			
		~Point2d();
		
		PROPERTY(float, X);
		PROPERTY(float, Y);

		PROPERTY_ALIAS(float, Ro, X);
		PROPERTY_ALIAS(float, R, Y);
		
		Vector2d VectorTo(const Point2d& targetPoint) const;
		Vector2d ToVector() const;
		float DistanceSqrTo(const Point2d& targetPoint) const;
		float DistanceTo(const Point2d& targetPoint) const;
		Direction DirectionTo(const Point2d& targetPoint) const;
		bool operator==(const Point2d& r) const;
		bool operator!=(const Point2d& r) const;
		
		Point2d operator +(Vector2d v) const
		{
			return Point2d(m_X + v.X(), m_Y + v.Y());
		}

		//static Point2d operator *(Point2d l, double r)
		//{
		//	return new Point2d(l.m_X * r, l.m_Y * r);
		//}
		//static Point2d operator *(double l, Point2d r)
		//{
		//	return r * l;
		//}
		//static Point2d operator /(Point2d l, double r)
		//{
		//	return l * (1 / r);
		//}

		//static Point2d operator -(Point2d p, Vector v)
		//{
		//	return new Point2d(p.m_X - v.X, p.m_Y - v.Y);
		//}
		//static implicit operator PointF(Point2d p){
		//	return new PointF((float)p.m_X, (float)p.m_Y);
		//}
		//Point2d UnTransformBy(Position p)
		//{
		//	Point2d result = new Point2d(m_X - p.Location.m_X, m_Y - p.Location.m_Y);
		//	result = new Point2d(
		//		GeometryHelper.Cos(p.Angle) * result.m_X + GeometryHelper.Sin(p.Angle) * result.m_Y,
		//		-GeometryHelper.Sin(p.Angle) * result.m_X + GeometryHelper.Cos(p.Angle) * result.m_Y
		//		);
		//	return result;
		//}
		//Point2d TransformBy(Position p)
		//{
		//	Point2d result = new Point2d(
		//		GeometryHelper.Cos(p.Angle) * m_X - GeometryHelper.Sin(p.Angle) * m_Y,
		//		GeometryHelper.Sin(p.Angle) * m_X + GeometryHelper.Cos(p.Angle) * m_Y
		//		);
		//	result = new Point2d(result.m_X + p.Location.m_X, result.m_Y + p.Location.m_Y);
		//	return result;
		//}
		////public override string ToString()
		////{
		////	return string.Format("({0}; {1})", X.ToString(), Y.ToString());
		////}
		///// <summary>
		///// Point2d(Rad, Dist)
		///// </summary>
		///// <param name="origin"></param>
		///// <returns></returns>
		//Point2d ToPolarCS(Point2d origin)
		//{
		//	double distance = DistanceTo(origin);
		//	double ang = origin.VectorTo(this).ToRadian();
		//	return new Point2d(ang, distance);
		//}
		//Point2d ToPolarCS()
		//{
		//	return ToPolarCS(new Point2d());
		//}
		//Point2d ToEuclidCS(Point2d origin)
		//{
		//	return new Point2d(GeometryHelper.Cos(Ro) * R + origin.X, GeometryHelper.Sin(Ro) * R + origin.Y);
		//}
		//Point2d ToEuclidCS()
		//{
		//	return ToEuclidCS(new Point2d());
		//}
		//override string ToString()
		//{
		//	return string.Format("({0}; {1})", m_X, m_Y);
		//}
	};
}

