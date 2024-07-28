// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.UInt128
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;

namespace Microsoft.Azure.Documents
{
  internal struct UInt128 : IComparable, IComparable<UInt128>, IEquatable<UInt128>
  {
    public static readonly UInt128 MaxValue = new UInt128(ulong.MaxValue, ulong.MaxValue);
    public static readonly UInt128 MinValue = (UInt128) 0;
    private const int Length = 16;
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

    public static UInt128 Create(ulong low, ulong high) => new UInt128(low, high);

    public static UInt128 FromByteArray(byte[] bytes, int start = 0) => new UInt128(BitConverter.ToUInt64(bytes, start), BitConverter.ToUInt64(bytes, start + 8));

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

    public override string ToString() => BitConverter.ToString(UInt128.ToByteArray(this));

    public ulong GetHigh() => this.high;

    public ulong GetLow() => this.low;
  }
}
