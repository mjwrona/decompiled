// Decompiled with JetBrains decompiler
// Type: Tomlyn.TomlDateTime
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;


#nullable enable
namespace Tomlyn
{
  public record struct TomlDateTime(
    DateTimeOffset DateTime,
    int SecondPrecision,
    TomlDateTimeKind Kind) : IConvertible
  {
    public TomlDateTime(int year, int month, int day)
      : this(new DateTimeOffset(new System.DateTime(year, month, day)), 0, TomlDateTimeKind.LocalDate)
    {
    }

    public TomlDateTime(System.DateTime datetime)
      : this(new DateTimeOffset(datetime), 0, TomlDateTimeKind.LocalDateTime)
    {
    }

    public override string ToString() => ((IConvertible) this).ToString((IFormatProvider) CultureInfo.InvariantCulture);

    [ExcludeFromCodeCoverage]
    TypeCode IConvertible.GetTypeCode() => TypeCode.Object;

    [ExcludeFromCodeCoverage]
    bool IConvertible.ToBoolean(IFormatProvider? provider) => throw new NotSupportedException();

    [ExcludeFromCodeCoverage]
    byte IConvertible.ToByte(IFormatProvider? provider) => throw new NotSupportedException();

    [ExcludeFromCodeCoverage]
    char IConvertible.ToChar(IFormatProvider? provider) => throw new NotSupportedException();

    [ExcludeFromCodeCoverage]
    System.DateTime IConvertible.ToDateTime(IFormatProvider? provider) => this.DateTime.DateTime;

    [ExcludeFromCodeCoverage]
    Decimal IConvertible.ToDecimal(IFormatProvider? provider) => throw new NotSupportedException();

    [ExcludeFromCodeCoverage]
    double IConvertible.ToDouble(IFormatProvider? provider) => throw new NotSupportedException();

    [ExcludeFromCodeCoverage]
    short IConvertible.ToInt16(IFormatProvider? provider) => throw new NotSupportedException();

    [ExcludeFromCodeCoverage]
    int IConvertible.ToInt32(IFormatProvider? provider) => throw new NotSupportedException();

    [ExcludeFromCodeCoverage]
    long IConvertible.ToInt64(IFormatProvider? provider) => throw new NotSupportedException();

    [ExcludeFromCodeCoverage]
    sbyte IConvertible.ToSByte(IFormatProvider? provider) => throw new NotSupportedException();

    [ExcludeFromCodeCoverage]
    float IConvertible.ToSingle(IFormatProvider? provider) => throw new NotSupportedException();

    string IConvertible.ToString(IFormatProvider? provider)
    {
      switch (this.Kind)
      {
        case TomlDateTimeKind.OffsetDateTimeByNumber:
          DateTimeOffset localTime = this.DateTime.ToLocalTime();
          return localTime.Millisecond == 0 ? localTime.ToString("yyyy-MM-dd'T'HH:mm:sszzz", provider) : localTime.ToString("yyyy-MM-dd'T'HH:mm:ss." + TomlDateTime.GetFormatPrecision(this.SecondPrecision) + "zzz", provider);
        case TomlDateTimeKind.LocalDateTime:
          return this.DateTime.Millisecond == 0 ? this.DateTime.ToString("yyyy-MM-dd'T'HH:mm:ss", provider) : this.DateTime.ToString("yyyy-MM-dd'T'HH:mm:ss." + TomlDateTime.GetFormatPrecision(this.SecondPrecision), provider);
        case TomlDateTimeKind.LocalDate:
          return this.DateTime.ToString("yyyy-MM-dd", provider);
        case TomlDateTimeKind.LocalTime:
          return this.DateTime.Millisecond == 0 ? this.DateTime.ToString("HH:mm:ss", provider) : this.DateTime.ToString("HH:mm:ss." + TomlDateTime.GetFormatPrecision(this.SecondPrecision), provider);
        default:
          DateTimeOffset universalTime = this.DateTime.ToUniversalTime();
          return universalTime.Millisecond == 0 ? universalTime.ToString("yyyy-MM-dd'T'HH:mm:ssZ", provider) : universalTime.ToString("yyyy-MM-dd'T'HH:mm:ss." + TomlDateTime.GetFormatPrecision(this.SecondPrecision) + "Z", provider);
      }
    }

    object IConvertible.ToType(Type conversionType, IFormatProvider? provider)
    {
      if (conversionType == typeof (System.DateTime))
        return (object) this.DateTime.DateTime;
      if (conversionType == typeof (DateTimeOffset))
        return (object) this.DateTime;
      throw new InvalidCastException("Unable to convert TomlDateTime to destination type " + conversionType.FullName);
    }

    [ExcludeFromCodeCoverage]
    ushort IConvertible.ToUInt16(IFormatProvider? provider) => throw new NotSupportedException();

    [ExcludeFromCodeCoverage]
    uint IConvertible.ToUInt32(IFormatProvider? provider) => throw new NotSupportedException();

    [ExcludeFromCodeCoverage]
    ulong IConvertible.ToUInt64(IFormatProvider? provider) => throw new NotSupportedException();

    public static string GetFormatPrecision(int precision)
    {
      switch (precision)
      {
        case 1:
          return "f";
        case 2:
          return "ff";
        case 3:
          return "fff";
        case 4:
          return "ffff";
        case 5:
          return "fffff";
        case 6:
          return "ffffff";
        case 7:
          return "fffffff";
        default:
          return "fff";
      }
    }

    public static implicit operator TomlDateTime(System.DateTime dateTime) => new TomlDateTime((DateTimeOffset) dateTime, 0, TomlDateTimeKind.LocalDateTime);
  }
}
