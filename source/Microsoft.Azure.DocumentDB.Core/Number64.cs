// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Number64
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System;
using System.Globalization;

namespace Microsoft.Azure.Documents
{
  [JsonConverter(typeof (Number64.Number64JsonConverter))]
  internal struct Number64 : IComparable<Number64>, IEquatable<Number64>
  {
    public static readonly Number64 MaxValue = new Number64(double.MaxValue);
    public static readonly Number64 MinValue = new Number64(double.MinValue);
    private readonly double? doubleValue;
    private readonly long? longValue;

    private Number64(double value)
    {
      this.doubleValue = new double?(value);
      this.longValue = new long?();
    }

    private Number64(long value)
    {
      this.longValue = new long?(value);
      this.doubleValue = new double?();
    }

    public bool IsInteger => this.longValue.HasValue;

    public bool IsDouble => this.doubleValue.HasValue;

    public bool IsInfinity => !this.IsInteger && double.IsInfinity(this.doubleValue.Value);

    public bool IsNaN => !this.IsInteger && double.IsNaN(this.doubleValue.Value);

    public override string ToString() => this.ToString((string) null, (IFormatProvider) CultureInfo.CurrentCulture);

    public string ToString(string format) => this.ToString(format, (IFormatProvider) CultureInfo.CurrentCulture);

    public string ToString(IFormatProvider formatProvider) => this.ToString((string) null, formatProvider);

    public string ToString(string format, IFormatProvider formatProvider) => !this.IsDouble ? Number64.ToLong(this).ToString(format, formatProvider) : Number64.ToDouble(this).ToString(format, formatProvider);

    public static bool operator <(Number64 left, Number64 right) => left.CompareTo(right) < 0;

    public static bool operator >(Number64 left, Number64 right) => left.CompareTo(right) > 0;

    public static bool operator <=(Number64 left, Number64 right) => !(right < left);

    public static bool operator >=(Number64 left, Number64 right) => !(left < right);

    public static bool operator ==(Number64 left, Number64 right) => left.Equals(right);

    public static bool operator !=(Number64 left, Number64 right) => !(left == right);

    public static implicit operator Number64(long value) => new Number64(value);

    public static implicit operator Number64(double value) => new Number64(value);

    public static long ToLong(Number64 number64) => !number64.IsInteger ? (long) number64.doubleValue.Value : number64.longValue.Value;

    public static double ToDouble(Number64 number64) => !number64.IsDouble ? (double) number64.longValue.Value : number64.doubleValue.Value;

    public int CompareTo(object value)
    {
      if (value == null)
        return 1;
      if (value is Number64 other)
        return this.CompareTo(other);
      throw new ArgumentException("Value must be a Number64.");
    }

    public int CompareTo(Number64 other) => !this.IsInteger || !other.IsInteger ? (!this.IsDouble || !other.IsDouble ? ((Number64.DoubleEx) (this.IsDouble ? this.doubleValue.Value : (double) this.longValue.Value)).CompareTo((Number64.DoubleEx) (other.IsDouble ? other.doubleValue.Value : (double) other.longValue.Value)) : this.doubleValue.Value.CompareTo(other.doubleValue.Value)) : this.longValue.Value.CompareTo(other.longValue.Value);

    public override bool Equals(object obj)
    {
      if ((ValueType) this == obj)
        return true;
      return obj is Number64 other && this.Equals(other);
    }

    public bool Equals(Number64 other) => this.CompareTo(other) == 0;

    public override int GetHashCode() => (!this.IsDouble ? (Number64.DoubleEx) this.longValue.Value : (Number64.DoubleEx) this.doubleValue.Value).GetHashCode();

    private struct DoubleEx : IEquatable<Number64.DoubleEx>, IComparable<Number64.DoubleEx>
    {
      private DoubleEx(double doubleValue, ushort extraBits)
      {
        this.DoubleValue = doubleValue;
        this.ExtraBits = extraBits;
      }

      public double DoubleValue { get; }

      public ushort ExtraBits { get; }

      public static bool operator ==(Number64.DoubleEx left, Number64.DoubleEx right) => left.Equals(right);

      public static bool operator !=(Number64.DoubleEx left, Number64.DoubleEx right) => !(left == right);

      public static bool operator <(Number64.DoubleEx left, Number64.DoubleEx right) => left.CompareTo(right) < 0;

      public static bool operator >(Number64.DoubleEx left, Number64.DoubleEx right) => left.CompareTo(right) > 0;

      public static bool operator <=(Number64.DoubleEx left, Number64.DoubleEx right) => !(right < left);

      public static bool operator >=(Number64.DoubleEx left, Number64.DoubleEx right) => !(left < right);

      public static implicit operator Number64.DoubleEx(long value)
      {
        if (value == long.MinValue)
          return new Number64.DoubleEx((double) value, (ushort) 0);
        long x = Math.Abs(value);
        int significantBitIndex = BitUtils.GetMostSignificantBitIndex((ulong) x);
        ushort extraBits;
        double doubleValue;
        if (significantBitIndex > 52 && significantBitIndex - BitUtils.GetLeastSignificantBitIndex(x) > 52)
        {
          int num1 = significantBitIndex;
          long num2 = (long) num1 + 1023L << 52;
          long num3 = x << 62 - num1 & 4611686018427387903L;
          extraBits = (ushort) ((num3 & 1023L) << 6);
          long num4 = num3 >> 10;
          long num5 = num2 | num4;
          if (value != x)
            num5 |= long.MinValue;
          doubleValue = BitConverter.Int64BitsToDouble(num5);
        }
        else
        {
          doubleValue = (double) value;
          extraBits = (ushort) 0;
        }
        return new Number64.DoubleEx(doubleValue, extraBits);
      }

      public static implicit operator long(Number64.DoubleEx value)
      {
        long output;
        if (value.ExtraBits != (ushort) 0)
        {
          output = BitConverter.DoubleToInt64Bits(value.DoubleValue);
          int num1 = BitUtils.BitTestAndReset64(output, 63, out output) ? 1 : 0;
          int num2 = (int) ((output >> 52) - 1023L);
          output <<= 10;
          output = (output | 4611686018427387904L) & long.MaxValue;
          output |= (long) value.ExtraBits >> 6;
          output >>= 62 - num2;
          if (num1 != 0)
            output = -output;
        }
        else
          output = (long) value.DoubleValue;
        return output;
      }

      public static implicit operator Number64.DoubleEx(double value) => new Number64.DoubleEx(value, (ushort) 0);

      public override bool Equals(object obj)
      {
        if ((ValueType) this == obj)
          return true;
        return obj is Number64.DoubleEx other && this.Equals(other);
      }

      public bool Equals(Number64.DoubleEx other) => this.DoubleValue == other.DoubleValue && (int) this.ExtraBits == (int) other.ExtraBits;

      public override int GetHashCode() => 0 ^ this.DoubleValue.GetHashCode() ^ this.ExtraBits.GetHashCode();

      public int CompareTo(Number64.DoubleEx other)
      {
        int num = this.DoubleValue.CompareTo(other.DoubleValue);
        if (num == 0)
          num = this.ExtraBits.CompareTo(other.ExtraBits) * Math.Sign(this.DoubleValue);
        return num;
      }
    }

    private sealed class Number64JsonConverter : JsonConverter
    {
      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {
        Number64 number64 = (Number64) value;
        writer.WriteValue(number64.IsDouble ? Number64.ToDouble(number64) : (double) Number64.ToLong(number64));
      }

      public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object existingValue,
        JsonSerializer serializer)
      {
        return (object) (reader.TokenType != JsonToken.Float ? (Number64) (long) reader.Value : (Number64) (double) reader.Value);
      }

      public override bool CanConvert(Type objectType) => (object) objectType == (object) typeof (User);
    }
  }
}
