using System;
using System.Globalization;
using System.Numerics;

namespace CoffeeUtilities
{
    public class PiCalculator
    {
        // Machin's formula: Pi = 16 * ArcTan(1/5) - 4 * ArcTan(1/239)
        public static string Calculate(int numberOfDigits)
        {
            numberOfDigits += 8; //  To be safe, compute 8 extra digits, to be dropped at end. The 8 is arbitrary

            BigInteger a = BigInteger.Multiply(ArcTan(5, numberOfDigits), new BigInteger(16));
            BigInteger b = BigInteger.Multiply(ArcTan(239, numberOfDigits), new BigInteger(4));
            BigInteger pi = BigInteger.Subtract(a, b);

            string piAsString = BigInteger.Divide(pi, new BigInteger(100000000)).ToString();
            string piFormatted = string.Format(CultureInfo.CurrentCulture, "{0}.{1}", piAsString[0], piAsString.Substring(1, numberOfDigits - 8));
            return piFormatted;
        }

        private static BigInteger ArcTan(int denominator, int numberOfDigits)
        {
            // The degree of the Taylor polynomial needed to achieve numberOfDigits accuracy of ArcTan(1/denominator).
            int demonimatorSquared = denominator * denominator;
            int degreeNeeded = 0;
            while ((Math.Log(2 * degreeNeeded + 3) + (degreeNeeded + 1) * Math.Log10(demonimatorSquared)) <= numberOfDigits * Math.Log(10))
            {
                degreeNeeded++;
            }

            // 10^numberOfDigits.
            BigInteger tenToPowerOfNumberOfDigits = new BigInteger(1);
            for (int i = 0; i < numberOfDigits; i++)
            {
                tenToPowerOfNumberOfDigits = BigInteger.Multiply(tenToPowerOfNumberOfDigits, new BigInteger(10));
            }

            // s = (10^N)/c
            int c = 2 * degreeNeeded + 1;
            BigInteger s = BigInteger.Divide(tenToPowerOfNumberOfDigits, new BigInteger(c));
            for (int i = 0; i < degreeNeeded; i++)
            {
                c = c - 2;
                BigInteger temp1 = BigInteger.Divide(tenToPowerOfNumberOfDigits, new BigInteger(c));
                BigInteger temp2 = BigInteger.Divide(s, new BigInteger(demonimatorSquared));
                s = BigInteger.Subtract(temp1, temp2);
            }

            // Return s/denominator, which is integer part of 10^numberOfDigits x ArcTan(1/k).
            return BigInteger.Divide(s, new BigInteger(denominator));
        }
    }
}
