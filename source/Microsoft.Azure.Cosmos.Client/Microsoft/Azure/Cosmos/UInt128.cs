// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.UInt128
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Cosmos
{
  internal readonly struct UInt128 : IComparable, IComparable<UInt128>, IEquatable<UInt128>
  {
    public const int Length = 16;
    public static readonly UInt128 MaxValue = new UInt128(ulong.MaxValue, ulong.MaxValue);
    public static readonly UInt128 MinValue = (UInt128) 0;
    private readonly ulong low;
    private readonly ulong high;

    private UInt128(ulong low, ulong high)
    {
      this.low = low;
      this.high = high;
    }

    public static implicit operator UInt128(int value) => new UInt128((ulong) value, 0UL);

    public static implicit operator UInt128(long value) => new UInt128((ulong) value, 0UL);

    public static implicit operator UInt128(uint value) => new UInt128((ulong) value, 0UL);

    public static implicit operator UInt128(ulong value) => new UInt128(value, 0UL);

    public static UInt128 operator +(UInt128 augend, UInt128 addend)
    {
      long low = (long) augend.low + (long) addend.low;
      ulong high = augend.high + addend.high;
      if ((ulong) low < augend.low)
        ++high;
      return new UInt128((ulong) low, high);
    }

    public static UInt128 operator -(UInt128 minuend, UInt128 subtrahend)
    {
      long low = (long) minuend.low - (long) subtrahend.low;
      ulong high = minuend.high - subtrahend.high;
      if ((ulong) low > minuend.low)
        --high;
      return new UInt128((ulong) low, high);
    }

    public static UInt128 operator *(UInt128 multiplicand, UInt128 multiplier)
    {
      (UInt128 h, UInt128 l) = UInt128.Mult128To256(multiplicand, multiplier);
      if (!(h != (UInt128) 0))
        return l;
      throw new OverflowException();
    }

    private static (ulong h, ulong l) Mult64To128(ulong u, ulong v)
    {
      long num1 = (long) u & (long) uint.MaxValue;
      ulong num2 = v & (ulong) uint.MaxValue;
      ulong num3 = (ulong) num1 * num2;
      ulong num4 = num3 & (ulong) uint.MaxValue;
      ulong num5 = num3 >> 32;
      u >>= 32;
      ulong num6 = u * num2 + num5;
      ulong num7 = num6 & (ulong) uint.MaxValue;
      ulong num8 = num6 >> 32;
      v >>= 32;
      ulong num9 = (ulong) num1 * v + num7;
      ulong num10 = num9 >> 32;
      return (u * v + num8 + num10, (num9 << 32) + num4);
    }

    private static (UInt128 h, UInt128 l) Mult128To256(UInt128 n, UInt128 m)
    {
      (ulong high1, ulong low1) = UInt128.Mult64To128(n.high, m.high);
      (ulong h1, ulong num1) = UInt128.Mult64To128(n.low, m.low);
      (ulong h2, ulong l1) = UInt128.Mult64To128(n.high, m.low);
      ulong num2 = h1 + l1;
      if (num2 < l1)
      {
        UInt128 uint128 = UInt128.Create(low1, high1) + (UInt128) 1;
        high1 = uint128.high;
        low1 = uint128.low;
      }
      ulong low2 = low1 + h2;
      if (low2 < h2)
        ++high1;
      (ulong h3, ulong l2) = UInt128.Mult64To128(n.low, m.high);
      ulong high2 = num2 + l2;
      if (high2 < l2)
      {
        UInt128 uint128 = UInt128.Create(low2, high1) + (UInt128) 1;
        high1 = uint128.high;
        low2 = uint128.low;
      }
      ulong low3 = low2 + h3;
      if (low3 < h3)
        ++high1;
      return (UInt128.Create(low3, high1), UInt128.Create(num1, high2));
    }

    public static UInt128 operator /(UInt128 dividend, UInt128 divisor)
    {
      if (divisor == (UInt128) 0)
        throw new DivideByZeroException();
      uint num1 = !(divisor > (UInt128) uint.MaxValue) ? (uint) divisor.low : throw new ArgumentOutOfRangeException(string.Format("{0} must be less than 32 bits.", (object) divisor));
      if (dividend == (UInt128) 0)
        return UInt128.Create(0UL, 0UL);
      ulong num2 = dividend.high >> 32;
      ulong num3 = dividend.high & (ulong) uint.MaxValue;
      ulong num4 = dividend.low >> 32;
      ulong num5 = dividend.low & (ulong) uint.MaxValue;
      long num6 = (long) num2;
      ulong num7 = (ulong) num6 / (ulong) num1 & (ulong) uint.MaxValue;
      long num8 = ((long) ((ulong) num6 % (ulong) num1) << 32) + (long) num3;
      ulong num9 = (ulong) num8 / (ulong) num1 & (ulong) uint.MaxValue;
      long num10 = ((long) ((ulong) num8 % (ulong) num1) << 32) + (long) num4;
      return UInt128.Create((((ulong) num10 / (ulong) num1 & (ulong) uint.MaxValue) << 32) + ((((ulong) num10 % (ulong) num1 << 32) + num5) / (ulong) num1 & (ulong) uint.MaxValue), (num7 << 32) + num9);
    }

    public static bool operator <(UInt128 left, UInt128 right)
    {
      if (left.high < right.high)
        return true;
      return (long) left.high == (long) right.high && left.low < right.low;
    }

    public static bool operator >(UInt128 left, UInt128 right) => right < left;

    public static bool operator <=(UInt128 left, UInt128 right) => !(right < left);

    public static bool operator >=(UInt128 left, UInt128 right) => !(left < right);

    public static bool operator ==(UInt128 left, UInt128 right) => (long) left.high == (long) right.high && (long) left.low == (long) right.low;

    public static bool operator !=(UInt128 left, UInt128 right) => !(left == right);

    public static UInt128 operator &(UInt128 left, UInt128 right) => new UInt128(left.low & right.low, left.high & right.high);

    public static UInt128 operator |(UInt128 left, UInt128 right) => new UInt128(left.low | right.low, left.high | right.high);

    public static UInt128 operator ^(UInt128 left, UInt128 right) => new UInt128(left.low ^ right.low, left.high ^ right.high);

    public static UInt128 operator ++(UInt128 value) => value + (UInt128) 1;

    public static UInt128 operator --(UInt128 value) => value - (UInt128) 1;

    public static UInt128 Create(ulong low, ulong high) => new UInt128(low, high);

    public static UInt128 FromByteArray(ReadOnlySpan<byte> buffer)
    {
      UInt128 uint128;
      if (!UInt128.TryCreateFromByteArray(buffer, out uint128))
        throw new FormatException("Malformed buffer");
      return uint128;
    }

    public static byte[] ToByteArray(UInt128 uint128)
    {
      byte[] byteArray = new byte[16];
      byte[] bytes1 = BitConverter.GetBytes(uint128.low);
      byte[] bytes2 = BitConverter.GetBytes(uint128.high);
      bytes1.CopyTo((Array) byteArray, 0);
      byte[] numArray = byteArray;
      bytes2.CopyTo((Array) numArray, 8);
      return byteArray;
    }

    public int CompareTo(object value)
    {
      if (value == null)
        return 1;
      if (value is UInt128 other)
        return this.CompareTo(other);
      throw new ArgumentException("Value must be a UInt128.");
    }

    public int CompareTo(UInt128 other)
    {
      if (this < other)
        return -1;
      return this > other ? 1 : 0;
    }

    public override bool Equals(object obj)
    {
      if ((ValueType) this == obj)
        return true;
      return obj is UInt128 other && this.Equals(other);
    }

    public bool Equals(UInt128 other) => this == other;

    public override int GetHashCode()
    {
      ulong num = this.low;
      int hashCode1 = num.GetHashCode();
      num = this.high;
      int hashCode2 = num.GetHashCode();
      return hashCode1 ^ hashCode2;
    }

    public override string ToString()
    {
      byte[] byteArray = UInt128.ToByteArray(this);
      Array.Reverse((Array) byteArray, 0, byteArray.Length);
      return BitConverter.ToString(byteArray);
    }

    public ulong GetHigh() => this.high;

    public ulong GetLow() => this.low;

    public static unsafe bool TryParse(string value, out UInt128 uInt128)
    {
      string[] strArray = !string.IsNullOrEmpty(value) ? ((IEnumerable<string>) value.Split('-')).Take<string>(16).ToArray<string>() : throw new ArgumentException("value can not be null or empty.");
      if (strArray.Length != 16)
      {
        uInt128 = new UInt128();
        return false;
      }
      // ISSUE: untyped stack allocation
      Span<byte> span = new Span<byte>((void*) __untypedstackalloc(new IntPtr(16)), 16);
      for (int index = 0; index < 16; ++index)
      {
        byte result;
        if (!byte.TryParse(strArray[index], NumberStyles.HexNumber, (IFormatProvider) null, out result))
        {
          uInt128 = new UInt128();
          return false;
        }
        span[index] = result;
      }
      span.Reverse<byte>();
      uInt128 = UInt128.FromByteArray((ReadOnlySpan<byte>) span);
      return true;
    }

    public static bool TryCreateFromByteArray(ReadOnlySpan<byte> buffer, out UInt128 value)
    {
      if (buffer.Length < 16)
      {
        value = new UInt128();
        return false;
      }
      ReadOnlySpan<ulong> readOnlySpan = MemoryMarshal.Cast<byte, ulong>(buffer);
      value = new UInt128(readOnlySpan[0], readOnlySpan[1]);
      return true;
    }
  }
}
