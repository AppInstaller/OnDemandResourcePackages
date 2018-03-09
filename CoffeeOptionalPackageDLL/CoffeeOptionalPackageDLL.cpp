#include "pch.h"
#include "CoffeeOptionalPackageDLL.h"

// Optional Package Code
__declspec(dllexport) UINT32 Factorial(UINT32 smallNumber) 
{
	if (smallNumber <= 1) return 1;

	return smallNumber * Factorial(smallNumber - 1);
}


