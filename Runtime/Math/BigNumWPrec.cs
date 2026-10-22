using System;
using System.Numerics;
using UnityEngine;

namespace MyTools.Math
{
    /// <summary>
    /// Represents a big number (signed) with limited precision using scientific notation.
    /// Format: mantissa × 10^exponent, where mantissa is limited to 'precision' digits.
    /// Supports both integers and decimals. Exponent can be negative for fractional values.
    /// Example: 123.456 with precision=6 is stored as mantissa=123456, exponent=-3
    /// </summary>
    /// <remarks>
    /// To maximize performance and minimize memory usage, choose 'precision' as a multiple of 9.
    /// </remarks>
    [Serializable]
    public struct BigNumWPrec : IEquatable<BigNumWPrec>, IComparable<BigNumWPrec>, ISerializationCallbackReceiver
    {
        private const int DEFAULT_PRECISION = 90;   // Default precision for mantissa (can be adjusted)

        private BigInteger _mantissa;  // The significant digits (limited by precision)
        private int _exponent;         // Power of 10 (can be negative for decimals)
        [SerializeField] private string _valueStr;

        public uint precision;        // Maximum number of digits in mantissa

        public void OnBeforeSerialize() => _valueStr = ToString();

        public void OnAfterDeserialize()
        {
            if (string.IsNullOrEmpty(_valueStr)) return;
            int eIdx = _valueStr.IndexOf('e');
            if (eIdx >= 0)
            {
                BigInteger.TryParse(_valueStr[..eIdx], out _mantissa);
                int.TryParse(_valueStr[(eIdx + 1)..], out _exponent);
            }
            else
            {
                BigInteger.TryParse(_valueStr, out _mantissa);
                _exponent = 0;
            }
            Normalize();
        }

        public BigNumWPrec(BigInteger value, uint precision = DEFAULT_PRECISION)
        {
            if (precision == 0)
                throw new ArgumentException("Precision must be positive", nameof(precision));

            this.precision = precision;
            this._exponent = 0;
            this._mantissa = value;
            this._valueStr = null;

            Normalize();
        }

        public BigNumWPrec(int value, uint precision = DEFAULT_PRECISION)
        {
            if (precision == 0)
                throw new ArgumentException("Precision must be positive", nameof(precision));

            this.precision = precision;
            this._exponent = 0;
            this._mantissa = new BigInteger(value);
            this._valueStr = null;

            Normalize();
        }

        public BigNumWPrec(long value, uint precision = DEFAULT_PRECISION)
        {
            if (precision == 0)
                throw new ArgumentException("Precision must be positive", nameof(precision));

            this.precision = precision;
            this._exponent = 0;
            this._mantissa = new BigInteger(value);
            this._valueStr = null;

            Normalize();
        }

        /// <summary>
        /// Creates from a double value. The decimal part will be preserved up to precision limit.
        /// </summary>
        public BigNumWPrec(double value, uint precision = DEFAULT_PRECISION)
        {
            if (precision == 0)
                throw new ArgumentException("Precision must be positive", nameof(precision));

            this.precision = precision;
            this._valueStr = null;

            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentException("Cannot create from NaN or Infinity", nameof(value));
            
            if (value == 0)
            {
                this._mantissa = 0;
                this._exponent = 0;
                return;
            }
            
            // Convert double to string to preserve all digits
            string valueStr = value.ToString("R"); // Round-trip format
            
            // Remove sign for processing
            bool isNegative = valueStr.StartsWith("-");
            if (isNegative)
                valueStr = valueStr.Substring(1);
            
            // Handle scientific notation (e.g., "1.23E+10")
            int eIndex = valueStr.IndexOfAny(new[] { 'E', 'e' });
            int scientificExponent = 0;
            if (eIndex >= 0)
            {
                scientificExponent = int.Parse(valueStr.Substring(eIndex + 1));
                valueStr = valueStr.Substring(0, eIndex);
            }
            
            // Split into integer and fractional parts
            string[] parts = valueStr.Split('.');
            string integerPart = parts[0];
            string fractionalPart = parts.Length > 1 ? parts[1] : "";
            
            // Combine all digits
            string allDigits = integerPart + fractionalPart;
            
            // Remove leading zeros
            allDigits = allDigits.TrimStart('0');
            if (string.IsNullOrEmpty(allDigits))
                allDigits = "0";
            
            // Parse mantissa
            this._mantissa = BigInteger.Parse(allDigits);
            if (isNegative)
                this._mantissa = -this._mantissa;
            
            // Calculate exponent: scientific exponent - number of fractional digits
            this._exponent = scientificExponent - fractionalPart.Length;
            
            Normalize();
        }

        public BigNumWPrec(BigInteger mantissa, int exponent, uint precision)
        {
            if (precision == 0)
                throw new ArgumentException("Precision must be positive", nameof(precision));

            this._mantissa = mantissa;
            this._exponent = exponent;
            this.precision = precision;
            this._valueStr = null;

            Normalize();
        }

        /// <summary>
        /// Normalizes the number to ensure mantissa doesn't exceed precision digits.
        /// Moves excess digits to the exponent.
        /// </summary>
        private void Normalize()
        {
            if (_mantissa == 0)
            {
                _exponent = 0;
                return;
            }

            // Remove trailing zeros and adjust exponent
            while (_mantissa % 10 == 0)
            {
                _mantissa /= 10;
                _exponent++;
            }

            // Get number of digits in mantissa (excluding sign for negative numbers)
            BigInteger absValue = BigInteger.Abs(_mantissa);
            int digitCount = absValue.ToString().Length;
            
            // If mantissa exceeds precision, truncate and adjust exponent
            if (digitCount > (int)precision)
            {
                int excess = digitCount - (int)precision;
                BigInteger divisor = BigInteger.Pow(10, excess);
                _mantissa /= divisor;
                _exponent += excess;
            }
        }

        // Arithmetic operators
        public static BigNumWPrec operator +(BigNumWPrec a, BigNumWPrec b)
        {
            uint maxPrecision = System.Math.Max(a.precision, b.precision);
            
            // Align exponents
            if (a._exponent == b._exponent)
            {
                return new BigNumWPrec(a._mantissa + b._mantissa, a._exponent, maxPrecision);
            }
            else if (a._exponent > b._exponent)
            {
                int expDiff = a._exponent - b._exponent;
                BigInteger aAligned = a._mantissa * BigInteger.Pow(10, expDiff);
                return new BigNumWPrec(b._mantissa + aAligned, b._exponent, maxPrecision);
            }
            else    // b._exponent > a._exponent
            {
                int expDiff = b._exponent - a._exponent;
                BigInteger bAligned = b._mantissa * BigInteger.Pow(10, expDiff);
                return new BigNumWPrec(a._mantissa + bAligned, a._exponent, maxPrecision);
            }
        }

        public static BigNumWPrec operator -(BigNumWPrec a, BigNumWPrec b)
        {
            uint maxPrecision = System.Math.Max(a.precision, b.precision);
            
            // Align exponents
            BigInteger result;
            int resultExponent;
            
            if (a._exponent == b._exponent)
            {
                result = a._mantissa - b._mantissa;
                resultExponent = a._exponent;
            }
            else if (a._exponent > b._exponent)
            {
                int expDiff = a._exponent - b._exponent;
                BigInteger aAligned = a._mantissa * BigInteger.Pow(10, expDiff);
                result = aAligned - b._mantissa;
                resultExponent = b._exponent;
            }
            else
            {
                int expDiff = b._exponent - a._exponent;
                BigInteger bAligned = b._mantissa * BigInteger.Pow(10, expDiff);
                result = a._mantissa - bAligned;
                resultExponent = a._exponent;
            }
            
            return new BigNumWPrec(result, resultExponent, maxPrecision);
        }

        public static BigNumWPrec operator *(BigNumWPrec a, BigNumWPrec b)
        {
            uint maxPrecision = System.Math.Max(a.precision, b.precision);
            BigInteger resultMantissa = a._mantissa * b._mantissa;
            int resultExponent = a._exponent + b._exponent;
            
            return new BigNumWPrec(resultMantissa, resultExponent, maxPrecision);
        }

        public static BigNumWPrec operator /(BigNumWPrec a, BigNumWPrec b)
        {
            if (b._mantissa == 0)
                throw new DivideByZeroException();
            
            uint maxPrecision = System.Math.Max(a.precision, b.precision);
            
            // Scale up 'a' to maintain precision during integer division
            int scaleUp = (int)maxPrecision;
            BigInteger scaledMantissa = a._mantissa * BigInteger.Pow(10, scaleUp);
            BigInteger resultMantissa = scaledMantissa / b._mantissa;
            
            // Adjust exponent
            int resultExponent = a._exponent - b._exponent - scaleUp;
            
            return new BigNumWPrec(resultMantissa, resultExponent, maxPrecision);
        }

        public static BigNumWPrec operator %(BigNumWPrec a, BigNumWPrec b)
        {
            if (b._mantissa == 0)
                throw new DivideByZeroException();
            
            // For modulo, we need to convert both to same scale
            uint maxPrecision = System.Math.Max(a.precision, b.precision);
            
            if (a._exponent == b._exponent)
            {
                return new BigNumWPrec(a._mantissa % b._mantissa, a._exponent, maxPrecision);
            }
            
            // Align to smaller exponent (more decimal places)
            if (a._exponent > b._exponent)
            {
                int expDiff = a._exponent - b._exponent;
                BigInteger aAligned = a._mantissa * BigInteger.Pow(10, expDiff);
                return new BigNumWPrec(aAligned % b._mantissa, b._exponent, maxPrecision);
            }
            else
            {
                int expDiff = b._exponent - a._exponent;
                BigInteger bAligned = b._mantissa * BigInteger.Pow(10, expDiff);
                return new BigNumWPrec(a._mantissa % bAligned, a._exponent, maxPrecision);
            }
        }

        // Unary operators
        public static BigNumWPrec operator -(BigNumWPrec a)
        {
            return new BigNumWPrec(-a._mantissa, a._exponent, a.precision);
        }

        public static BigNumWPrec operator +(BigNumWPrec a)
        {
            return a;
        }

        // Comparison operators
        public static bool operator ==(BigNumWPrec a, BigNumWPrec b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(BigNumWPrec a, BigNumWPrec b)
        {
            return !a.Equals(b);
        }

        public static bool operator <(BigNumWPrec a, BigNumWPrec b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator >(BigNumWPrec a, BigNumWPrec b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator <=(BigNumWPrec a, BigNumWPrec b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static bool operator >=(BigNumWPrec a, BigNumWPrec b)
        {
            return a.CompareTo(b) >= 0;
        }

        // Implicit conversions (from smaller types to BigNumWPrec)
        
        /// <summary>
        /// Implicitly converts an int to BigNumWPrec.
        /// </summary>
        public static implicit operator BigNumWPrec(int value)
        {
            return new BigNumWPrec(value);
        }

        /// <summary>
        /// Implicitly converts a long to BigNumWPrec.
        /// </summary>
        public static implicit operator BigNumWPrec(long value)
        {
            return new BigNumWPrec(value);
        }

        /// <summary>
        /// Implicitly converts a float to BigNumWPrec.
        /// </summary>
        public static implicit operator BigNumWPrec(float value)
        {
            return new BigNumWPrec(value);
        }

        /// <summary>
        /// Implicitly converts a double to BigNumWPrec.
        /// </summary>
        public static implicit operator BigNumWPrec(double value)
        {
            return new BigNumWPrec(value);
        }

        /// <summary>
        /// Implicitly converts a BigInteger to BigNumWPrec.
        /// </summary>
        public static implicit operator BigNumWPrec(BigInteger value)
        {
            return new BigNumWPrec(value);
        }

        /// <summary>
        /// Explicitly converts BigNumWPrec to BigInteger. Truncates decimal part if present.
        /// </summary>
        public static explicit operator BigInteger(BigNumWPrec value)
        {
            if (value._exponent >= 0)
                return value._mantissa * BigInteger.Pow(10, value._exponent);
            else
                return value._mantissa / BigInteger.Pow(10, -value._exponent);
        }

        /// <summary>
        /// Explicitly converts BigNumWPrec to double. May lose precision for very large or very small numbers.
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator double(BigNumWPrec value)
        {
            return value.ToDouble();
        }

        /// <summary>
        /// Explicitly converts BigNumWPrec to float. May lose precision for very large or very small numbers.
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator float(BigNumWPrec value)
        {
            return (float)value.ToDouble();
        }

        // Other utility methods

        /// <summary>
        /// Raises the base number to the specified integer power using fast exponentiation algorithm.
        /// Time complexity: O(log n) where n is the exponent.
        /// </summary>
        /// <param name="baseNum">The base number to raise to a power</param>
        /// <param name="exponent">The integer exponent (can be negative)</param>
        /// <returns>The result of baseNum raised to the exponent power</returns>
        public static BigNumWPrec Pow(BigNumWPrec baseNum, int exponent)
        {
            // Handle special cases
            if (exponent == 0)
                return new BigNumWPrec(1, baseNum.precision);

            if (exponent == 1)
                return baseNum;

            // Handle negative exponents: a^(-n) = 1 / (a^n)
            if (exponent < 0)
            {
                if (baseNum._mantissa == 0)
                    throw new DivideByZeroException("Cannot raise zero to a negative power");

                var positiveResult = Pow(baseNum, -exponent);
                return new BigNumWPrec(1, baseNum.precision) / positiveResult;
            }

            // Fast exponentiation using iterative approach
            BigNumWPrec result = new BigNumWPrec(1, baseNum.precision);
            BigNumWPrec current = baseNum;
            int exp = exponent;

            while (exp > 0)
            {
                // If exponent is odd, multiply result by current base
                if ((exp & 1) == 1)  // Equivalent to: exp % 2 == 1
                    result *= current;

                // Square the base
                current *= current;

                // Halve the exponent
                exp >>= 1;  // Equivalent to: exp /= 2
            }

            return result;
        }

        public static BigNumWPrec PowOf10(BigNumWPrec original, int exponent)
        {
            return new BigNumWPrec(original._mantissa, original._exponent + exponent, original.precision);
        }

        /// <summary>
        /// Returns the absolute value of this number.
        /// </summary>
        public static BigNumWPrec Abs(BigNumWPrec value)
        {
            if (value._mantissa < 0)
                return new BigNumWPrec(-value._mantissa, value._exponent, value.precision);
            return value;
        }

        /// <summary>
        /// Returns the absolute value of this number (instance method).
        /// </summary>
        public BigNumWPrec Abs()
        {
            return Abs(this);
        }

        // IEquatable implementation
        public bool Equals(BigNumWPrec other)
        {
            // Two numbers are equal if their mantissa and exponent are equal after normalization
            if (_mantissa == 0 && other._mantissa == 0)
                return true;

            if (_exponent == other._exponent)
                return _mantissa == other._mantissa;

            // Need to align exponents for comparison
            if (_exponent > other._exponent)
            {
                int expDiff = _exponent - other._exponent;
                BigInteger thisAligned = _mantissa * BigInteger.Pow(10, expDiff);
                return thisAligned == other._mantissa;
            }
            else
            {
                int expDiff = other._exponent - _exponent;
                BigInteger otherAligned = other._mantissa * BigInteger.Pow(10, expDiff);
                return _mantissa == otherAligned;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is BigNumWPrec other && Equals(other);
        }

        public override int GetHashCode()
        {
            // Normalize for hash code
            var normalized = this;
            normalized.Normalize();
            return HashCode.Combine(normalized._mantissa, normalized._exponent, precision);
        }

        // IComparable implementation
        public int CompareTo(BigNumWPrec other)
        {
            // Compare mantissa × 10^exponent
            if (_mantissa == 0 && other._mantissa == 0)
                return 0;
            
            // Handle negative vs positive
            int thisSign = _mantissa.Sign;
            int otherSign = other._mantissa.Sign;
            
            if (thisSign != otherSign)
                return thisSign.CompareTo(otherSign);
            
            if (thisSign == 0)  // Both are zero
                return 0;
            
            // Both have the same sign, compare magnitudes
            // For negative numbers, smaller magnitude is actually larger value
            int magnitudeComparison;
            
            // Compare by exponent and digit count for rough magnitude
            BigInteger absThis = BigInteger.Abs(_mantissa);
            BigInteger absOther = BigInteger.Abs(other._mantissa);
            int thisDigits = absThis.ToString().Length;
            int otherDigits = absOther.ToString().Length;
            int thisMagnitude = thisDigits + (int)_exponent;
            int otherMagnitude = otherDigits + (int)other._exponent;
            
            if (thisMagnitude != otherMagnitude)
            {
                magnitudeComparison = thisMagnitude.CompareTo(otherMagnitude);
                return thisSign < 0 ? -magnitudeComparison : magnitudeComparison;
            }
            
            // Need precise comparison - align exponents
            if (_exponent == other._exponent)
            {
                return _mantissa.CompareTo(other._mantissa);
            }
            else if (_exponent > other._exponent)
            {
                int expDiff = _exponent - other._exponent;
                BigInteger thisAligned = _mantissa * BigInteger.Pow(10, expDiff);
                return thisAligned.CompareTo(other._mantissa);
            }
            else
            {
                int expDiff = other._exponent - _exponent;
                BigInteger otherAligned = other._mantissa * BigInteger.Pow(10, expDiff);
                return _mantissa.CompareTo(otherAligned);
            }
        }

        // Utility methods
        
        /// <summary>
        /// Creates a new instance with a different precision limit.
        /// </summary>
        public BigNumWPrec WithPrecision(uint newPrecision)
        {
            return new BigNumWPrec(_mantissa, _exponent, newPrecision);
        }

        /// <summary>
        /// Converts to double. May lose precision for very large or very small numbers.
        /// </summary>
        public double ToDouble()
        {
            if (_mantissa == 0)
                return 0.0;
            
            double mantissaDouble = (double)_mantissa;
            
            // Handle extreme exponents to avoid overflow
            if (_exponent > 308)
                return mantissaDouble > 0 ? double.PositiveInfinity : double.NegativeInfinity;
            if (_exponent < -324)
                return 0.0;
            
            double exponentMultiplier = System.Math.Pow(10, _exponent);
            return mantissaDouble * exponentMultiplier;
        }

        /// <summary>
        /// Returns the number of total digits (magnitude).
        /// For decimals, returns negative values indicating decimal places.
        /// </summary>
        public int GetTotalDigitCount()
        {
            return BigInteger.Abs(_mantissa).ToString().Length + _exponent;
        }
        
        /// <summary>
        /// Returns a string representation of the number, 
        /// automatically choosing between standard and scientific notation 
        /// based on the total digit count and a specified limit.
        /// </summary>
        /// <param name="standardLimit"></param>
        /// <param name="significantDigits"></param>
        /// <returns></returns>
        public string ToAutoDisplayNotation(int standardLimit, int significantDigits, bool withComma)
        {
            int digitCount = GetTotalDigitCount();
            if (digitCount <= standardLimit)
                return ToStandardNotation(withComma);
            else
                return ToScientificNotation(significantDigits);
        }

        /// <summary>
        /// String representation in standard notation (may be very long for large numbers).
        /// For decimals, shows the decimal point.
        /// </summary>
        public string ToStandardNotation(bool withComma = false)
        {
            if (_mantissa == 0)
                return "0";

            if (_exponent == 0)
            {
                string s = _mantissa.ToString();
                if (!withComma) return s;
                bool neg = s[0] == '-';
                string abs = neg ? s[1..] : s;
                var sb = new System.Text.StringBuilder();
                int rem = abs.Length % 3;
                for (int i = 0; i < abs.Length; i++)
                {
                    if (i != 0 && (i - rem) % 3 == 0) sb.Append(',');
                    sb.Append(abs[i]);
                }
                return neg ? "-" + sb : sb.ToString();
            }

            // Handle negative numbers separately
            bool isNegative = _mantissa < 0;
            BigInteger absMantissa = BigInteger.Abs(_mantissa);
            string mantissaStr = absMantissa.ToString();

            string result;
            if (_exponent > 0)
            {
                // Positive exponent: multiply by 10^exponent (add zeros)
                result = mantissaStr + new string('0', _exponent);
            }
            else
            {
                // Negative exponent: insert decimal point
                int decimalPos = mantissaStr.Length + _exponent;

                if (decimalPos <= 0)
                {
                    // Need leading zeros (e.g., 0.00123)
                    result = "0." + new string('0', -decimalPos) + mantissaStr;
                }
                else if (decimalPos >= mantissaStr.Length)
                {
                    // No decimal needed (exponent brings it back to integer)
                    result = mantissaStr + new string('0', decimalPos - mantissaStr.Length);
                }
                else
                {
                    // Insert decimal point in the middle
                    result = mantissaStr.Insert(decimalPos, ".");
                }
            }

            if (withComma)
            {
                int dotIndex = result.IndexOf('.');
                string intPart = dotIndex >= 0 ? result[..dotIndex] : result;
                string decPart = dotIndex >= 0 ? result[dotIndex..] : "";

                var sb = new System.Text.StringBuilder();
                int rem = intPart.Length % 3;
                for (int i = 0; i < intPart.Length; i++)
                {
                    if (i != 0 && (i - rem) % 3 == 0) sb.Append(',');
                    sb.Append(intPart[i]);
                }
                result = sb.ToString() + decPart;
            }

            return isNegative ? "-" + result : result;
        }

        public string ToScientificNotation(int significantDigits)
        {
            if (_mantissa == 0)
                return "0";

            if (significantDigits <= 0)
                throw new ArgumentException("Significant digits must be positive", nameof(significantDigits));

            bool isNegative = _mantissa < 0;
            string digits = BigInteger.Abs(_mantissa).ToString();
            int digitCount = digits.Length;
            int sciExponent = _exponent + digitCount - 1;

            int actualSigDigits = System.Math.Min(significantDigits, digitCount);
            string sigDigits = digits[..actualSigDigits];

            string formatted = sigDigits.Length == 1
                ? sigDigits
                : (sigDigits[0] + "." + sigDigits[1..]).TrimEnd('0').TrimEnd('.');

            return $"{(isNegative ? "-" : "")}{formatted}e{sciExponent}";
        }

        public override string ToString()
        {
            return _mantissa + "e" + _exponent;
        }
    }
}