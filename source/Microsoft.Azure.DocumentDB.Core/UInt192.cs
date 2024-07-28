// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.UInt192
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Globalization;

namespace Microsoft.Azure.Documents
{
  internal struct UInt192 : IComparable, IComparable<UInt192>, IEquatable<UInt192>
  {
    public static readonly UInt192 MaxValue = new UInt192(ulong.MaxValue, ulong.MaxValue, ulong.MaxValue);
    public static readonly UInt192 MinValue = (UInt192) 0;
    private const int Length = 24;
    private readonly ulong low;
    private readonly ulong mid;
    private readonly ulong high;

    private UInt192(ulong low, ulong mid, ulong high)
    {
      this.low = low;
      this.mid = mid;
      this.high = high;
    }

    public static UInt192 operator +(UInt192 augend, UInt192 addend)
    {
      long low = (long) augend.low + (long) addend.low;
      ulong mid = augend.mid + addend.mid;
      ulong high = augend.high + addend.high;
      if ((ulong) low < augend.low)
        ++mid;
      if (mid < augend.mid)
        ++high;
      return new UInt192((ulong) low, mid, high);
    }

    public static UInt192 operator -(UInt192 minuend, UInt192 subtrahend)
    {
      long low = (long) minuend.low - (long) subtrahend.low;
      ulong mid = minuend.mid - subtrahend.mid;
      ulong high = minuend.high - subtrahend.high;
      if ((ulong) low > minuend.low)
        --mid;
      if (mid > minuend.mid)
        --high;
      return new UInt192((ulong) low, mid, high);
    }

    public static bool operator <(UInt192 left, UInt192 right)
    {
      if (left.high < right.high || (long) left.high == (long) right.high && left.mid < right.mid)
        return true;
      return (long) left.mid == (long) right.mid && left.low < right.low;
    }

    public static bool operator >(UInt192 left, UInt192 right) => right < left;

    public static bool operator <=(UInt192 left, UInt192 right) => !(right < left);

    public static bool operator >=(UInt192 left, UInt192 right) => !(left < right);

    public static bool operator ==(UInt192 left, UInt192 right) => (long) left.high == (long) right.high && (long) left.mid == (long) right.mid && (long) left.low == (long) right.low;

    public static bool operator !=(UInt192 left, UInt192 right) => !(left == right);

    public static UInt192 operator &(UInt192 left, UInt192 right) => new UInt192(left.low & right.low, left.mid & right.mid, left.high & right.high);

    public static UInt192 operator |(UInt192 left, UInt192 right) => new UInt192(left.low | right.low, left.mid | right.mid, left.high | right.high);

    public static UInt192 operator ^(UInt192 left, UInt192 right) => new UInt192(left.low ^ right.low, left.mid ^ right.mid, left.high ^ right.high);

    public static implicit operator UInt192(int value) => new UInt192((ulong) value, 0UL, 0UL);

    public static implicit operator UInt192(uint value) => new UInt192((ulong) value, 0UL, 0UL);

    public static implicit operator UInt192(ulong value) => new UInt192(value, 0UL, 0UL);

    public static implicit operator UInt192(long value) => new UInt192((ulong) value, 0UL, 0UL);

    public static implicit operator UInt192(UInt128 value) => new UInt192(value.GetLow(), value.GetHigh(), 0UL);

    public static UInt192 Parse(string value)
    {
      string[] strArray = !string.IsNullOrEmpty(value) ? value.Split('-') : throw new ArgumentException("value can not be null or empty.");
      if (strArray.Length != 24)
        throw new ArgumentException("not enough bytes encoded.");
      byte[] bytes = new byte[24];
      for (int index = 0; index < 24; ++index)
        bytes[index] = byte.Parse(strArray[index], NumberStyles.HexNumber, (IFormatProvider) null);
      return UInt192.FromByteArray(bytes);
    }

    public static UInt192 Create(ulong low, ulong mid, ulong high) => new UInt192(low, mid, high);

    public static UInt192 FromByteArray(byte[] bytes, int start = 0)
    {
      long uint64_1 = (long) BitConverter.ToUInt64(bytes, start);
      ulong uint64_2 = BitConverter.ToUInt64(bytes, start + 8);
      ulong uint64_3 = BitConverter.ToUInt64(bytes, start + 16);
      long mid = (long) uint64_2;
      long high = (long) uint64_3;
      return new UInt192((ulong) uint64_1, (ulong) mid, (ulong) high);
    }

    public static byte[] ToByteArray(UInt192 uint192)
    {
      byte[] byteArray = new byte[24];
      BitConverter.GetBytes(uint192.low).CopyTo((Array) byteArray, 0);
      BitConverter.GetBytes(uint192.mid).CopyTo((Array) byteArray, 8);
      BitConverter.GetBytes(uint192.high).CopyTo((Array) byteArray, 16);
      return byteArray;
    }

    public int CompareTo(object value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      if (value is UInt192 other)
        return this.CompareTo(other);
      throw new ArgumentException("Value must be a UInt192.");
    }

    public int CompareTo(UInt192 other)
    {
      if (this < other)
        return -1;
      return this > other ? 1 : 0;
    }

    public override bool Equals(object obj)
    {
      if ((ValueType) this == obj)
        return true;
      return obj is UInt192 other && this.Equals(other);
    }

    public bool Equals(UInt192 other) => this == other;

    public override int GetHashCode()
    {
      ulong num1 = this.low;
      int hashCode1 = num1.GetHashCode();
      num1 = this.mid;
      int hashCode2 = num1.GetHashCode();
      int num2 = hashCode1 ^ hashCode2;
      num1 = this.high;
      int hashCode3 = num1.GetHashCode();
      return num2 ^ hashCode3;
    }

    public override string ToString() => BitConverter.ToString(UInt192.ToByteArray(this));

    public ulong GetHigh() => this.high;

    public ulong GetMid() => this.mid;

    public ulong GetLow() => this.low;
  }
}
