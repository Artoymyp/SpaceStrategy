#pragma once
#ifndef COMMON_H
#define COMMON_H
#include <memory>

#define PROPERTY(TYPE, NAME)\
private:\
	TYPE m_##NAME; \
public:\
	TYPE NAME() const { return m_##NAME; }\
	void NAME(TYPE value) { m_##NAME = value; }

#define IPROPERTY(TYPE, NAME)\
public:\
	virtual TYPE NAME() const = 0;\
	virtual void NAME(TYPE value) = 0;

#define PROPERTY_DEFAULT_VAL(TYPE, NAME, DEFAULT_VAL)\
private:\
	TYPE m_##NAME = DEFAULT_VAL; \
public:\
	TYPE NAME() const { return m_##NAME; }\
	void NAME(TYPE value) { m_##NAME = value; }

#define PROPERTY_ALIAS(TYPE, NAME, PRIVATE_NAME)\
public:\
	TYPE NAME() const { return m_##PRIVATE_NAME; }\
	void NAME(TYPE value) { m_##PRIVATE_NAME = value; }

#endif
