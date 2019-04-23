using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace foundation
{
    public static class BigIntegerExtension
    {
        private static List<string> SymbolTable;

        public static string ToServerString(this BigInteger bigInteger)
        {
            return bigInteger.ToString(10);
        }
        public static string ToViewString(this BigInteger bigInteger)
        {
            if (SymbolTable == null)
            {
                SymbolTable = new List<string>() { "", "K", "M", "G", "T", "P", "E", "Z", "Y", "A", "B", "C", "D", "F", "H", "I", "J" };
            }
            string value = bigInteger.ToString(10);
            int len = value.Length;
            int vTotal = (int)(len / 3);
            int yValue = len % 3;

            int subDotCount = 3 - yValue;
            if (yValue == 0)
            {
                vTotal--;
                yValue = 3;
                subDotCount = 1;
            }

            if (vTotal > 0)
            {
                string symbol;
                if (vTotal >= SymbolTable.Count)
                {
                    symbol = SymbolTable.Last();
                }
                else
                {
                    symbol = SymbolTable[vTotal];
                }
                string preDot = value.Substring(0, yValue);
                string subDot = value.Substring(yValue, subDotCount);
                return preDot + "." + subDot + symbol;
            }

            return value;
        }
    }
    public class BigIntegerHelper
    {
        public static float DivideFloat(BigInteger a, BigInteger b)
        {
            if (a == null || b == null)
            {
                return 0.0f;
            }
            BigInteger result = (a * 100) / b;
            long resultInt = BigIntegerHelper.ToInt64(result);
            return resultInt / 100.0f;
        }
        public static long ToInt64(BigInteger value)
        {
            if (object.ReferenceEquals(value, null))
            {
                throw new ArgumentNullException("value");
            }
            return System.Int64.Parse(value.ToString(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture);
        }
        public static int ToInt32(BigInteger value)
        {
            if (object.ReferenceEquals(value, null))
            {
                throw new ArgumentNullException("value");
            }
            return System.Int32.Parse(value.ToString(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture);
        }

        public static BigInteger Multiply(BigInteger leftSide, float value)
        {
            BigInteger result = leftSide * (int)(value * 10000);
            return result / 10000;
        }
        public static BigInteger Multiply(BigInteger leftSide, BigInteger rightSide)
        {
            return leftSide * rightSide;
        }

        public static BigInteger FromScientific(string digits)
        {
            if (string.IsNullOrEmpty(digits) == true)
            {
                return 0;
            }

            long v = 0;
            if (Int64.TryParse(digits, out v))
            {
                return new BigInteger(v);
            }

            digits = digits.ToUpper().Trim();
            StringBuilder result = new StringBuilder();

            int eIndex = -1;
            int dotIndex = -1;
            int len = digits.Length;


            for (int idx = 0; idx < len; idx++)
            {
                int d = (int)digits[idx];

                if (d == '.')
                {
                    dotIndex = idx;
                    continue;
                }

                if (d == 'E')
                {
                    eIndex = idx;
                    break;
                }

                result.Append(digits[idx]);
            }

            int dotCount = 0;
            int powerValue = 0;
            if (eIndex != -1)
            {
                if (dotIndex != -1)
                {
                    dotCount = eIndex - dotIndex - 1;
                }

                string powerStringValue = digits.Substring(eIndex + 2);
                powerValue = int.Parse(powerStringValue);
                if (dotCount > 0)
                {
                    powerValue -= dotCount;
                }

                for (int i = 0; i < powerValue; i++)
                {
                    result.Append('0');
                }
            }
            return new BigInteger(result.ToString(), 10);
        }

    }
}