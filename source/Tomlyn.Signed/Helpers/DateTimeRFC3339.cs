// Decompiled with JetBrains decompiler
// Type: Tomlyn.Helpers.DateTimeRFC3339
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using System.Globalization;


#nullable enable
namespace Tomlyn.Helpers
{
  internal static class DateTimeRFC3339
  {
    private static readonly string[] OffsetDateTimeFormatsByZ = new string[16]
    {
      "yyyy-MM-ddTHH:mm:ssZ",
      "yyyy-MM-ddTHH:mm:ss.fZ",
      "yyyy-MM-ddTHH:mm:ss.ffZ",
      "yyyy-MM-ddTHH:mm:ss.fffZ",
      "yyyy-MM-ddTHH:mm:ss.ffffZ",
      "yyyy-MM-ddTHH:mm:ss.fffffZ",
      "yyyy-MM-ddTHH:mm:ss.ffffffZ",
      "yyyy-MM-ddTHH:mm:ss.fffffffZ",
      "yyyy-MM-dd HH:mm:ssZ",
      "yyyy-MM-dd HH:mm:ss.fZ",
      "yyyy-MM-dd HH:mm:ss.ffZ",
      "yyyy-MM-dd HH:mm:ss.fffZ",
      "yyyy-MM-dd HH:mm:ss.ffffZ",
      "yyyy-MM-dd HH:mm:ss.fffffZ",
      "yyyy-MM-dd HH:mm:ss.ffffffZ",
      "yyyy-MM-dd HH:mm:ss.fffffffZ"
    };
    private static readonly string[] OffsetDateTimeFormatsByNumber = new string[16]
    {
      "yyyy-MM-ddTHH:mm:sszzz",
      "yyyy-MM-ddTHH:mm:ss.fzzz",
      "yyyy-MM-ddTHH:mm:ss.ffzzz",
      "yyyy-MM-ddTHH:mm:ss.fffzzz",
      "yyyy-MM-ddTHH:mm:ss.ffffzzz",
      "yyyy-MM-ddTHH:mm:ss.fffffzzz",
      "yyyy-MM-ddTHH:mm:ss.ffffffzzz",
      "yyyy-MM-ddTHH:mm:ss.fffffffzzz",
      "yyyy-MM-dd HH:mm:sszzz",
      "yyyy-MM-dd HH:mm:ss.fzzz",
      "yyyy-MM-dd HH:mm:ss.ffzzz",
      "yyyy-MM-dd HH:mm:ss.fffzzz",
      "yyyy-MM-dd HH:mm:ss.ffffzzz",
      "yyyy-MM-dd HH:mm:ss.fffffzzz",
      "yyyy-MM-dd HH:mm:ss.ffffffzzz",
      "yyyy-MM-dd HH:mm:ss.fffffffzzz"
    };
    private static readonly string[] LocalDateTimeFormats = new string[16]
    {
      "yyyy-MM-ddTHH:mm:ss",
      "yyyy-MM-ddTHH:mm:ss.f",
      "yyyy-MM-ddTHH:mm:ss.ff",
      "yyyy-MM-ddTHH:mm:ss.fff",
      "yyyy-MM-ddTHH:mm:ss.ffff",
      "yyyy-MM-ddTHH:mm:ss.fffff",
      "yyyy-MM-ddTHH:mm:ss.ffffff",
      "yyyy-MM-ddTHH:mm:ss.fffffff",
      "yyyy-MM-dd HH:mm:ss",
      "yyyy-MM-dd HH:mm:ss.f",
      "yyyy-MM-dd HH:mm:ss.ff",
      "yyyy-MM-dd HH:mm:ss.fff",
      "yyyy-MM-dd HH:mm:ss.ffff",
      "yyyy-MM-dd HH:mm:ss.fffff",
      "yyyy-MM-dd HH:mm:ss.ffffff",
      "yyyy-MM-dd HH:mm:ss.fffffff"
    };
    private static readonly string[] LocalTimeFormats = new string[8]
    {
      "HH:mm:ss",
      "HH:mm:ss.f",
      "HH:mm:ss.ff",
      "HH:mm:ss.fff",
      "HH:mm:ss.ffff",
      "HH:mm:ss.fffff",
      "HH:mm:ss.ffffff",
      "HH:mm:ss.fffffff"
    };
    private static readonly DateTimeRFC3339.ParseDelegate TryParseDateTime = (DateTimeRFC3339.ParseDelegate) ((string text, string format, CultureInfo culture, DateTimeStyles style, out DateTimeOffset time) =>
    {
      time = new DateTimeOffset();
      DateTime result;
      if (!DateTime.TryParseExact(text, format, (IFormatProvider) culture, style, out result))
        return false;
      time = new DateTimeOffset(result);
      return true;
    });
    private static readonly DateTimeRFC3339.ParseDelegate TryParseDateTimeOffset = new DateTimeRFC3339.ParseDelegate(DateTimeOffset.TryParseExact);

    public static bool TryParseOffsetDateTime(string str, out TomlDateTime time) => DateTimeRFC3339.TryParseExactWithPrecision(str.ToUpperInvariant(), DateTimeRFC3339.OffsetDateTimeFormatsByZ, DateTimeRFC3339.TryParseDateTimeOffset, DateTimeStyles.None, TomlDateTimeKind.OffsetDateTimeByZ, out time) || DateTimeRFC3339.TryParseExactWithPrecision(str.ToUpperInvariant(), DateTimeRFC3339.OffsetDateTimeFormatsByNumber, DateTimeRFC3339.TryParseDateTimeOffset, DateTimeStyles.None, TomlDateTimeKind.OffsetDateTimeByNumber, out time);

    public static bool TryParseLocalDateTime(string str, out TomlDateTime time) => DateTimeRFC3339.TryParseExactWithPrecision(str.ToUpperInvariant(), DateTimeRFC3339.LocalDateTimeFormats, DateTimeRFC3339.TryParseDateTime, DateTimeStyles.AssumeLocal, TomlDateTimeKind.LocalDateTime, out time);

    public static bool TryParseLocalDate(string str, out TomlDateTime time)
    {
      DateTime result;
      if (DateTime.TryParseExact(str.ToUpperInvariant(), "yyyy-MM-dd", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result))
      {
        time = new TomlDateTime((DateTimeOffset) result, 0, TomlDateTimeKind.LocalDate);
        return true;
      }
      time = new TomlDateTime();
      return false;
    }

    public static bool TryParseLocalTime(string str, out TomlDateTime time) => DateTimeRFC3339.TryParseExactWithPrecision(str.ToUpperInvariant(), DateTimeRFC3339.LocalTimeFormats, DateTimeRFC3339.TryParseDateTime, DateTimeStyles.None, TomlDateTimeKind.LocalTime, out time);

    private static bool TryParseExactWithPrecision(
      string str,
      string[] formats,
      DateTimeRFC3339.ParseDelegate parser,
      DateTimeStyles style,
      TomlDateTimeKind kind,
      out TomlDateTime time)
    {
      time = new TomlDateTime();
      for (int index = 0; index < formats.Length; ++index)
      {
        string format = formats[index];
        DateTimeOffset time1;
        if (parser(str, format, CultureInfo.InvariantCulture, style, out time1))
        {
          int SecondPrecision = index;
          if (SecondPrecision >= DateTimeRFC3339.LocalTimeFormats.Length)
            SecondPrecision -= DateTimeRFC3339.LocalTimeFormats.Length;
          time = new TomlDateTime(time1, SecondPrecision, kind);
          return true;
        }
      }
      return false;
    }

    private delegate bool ParseDelegate(
      string text,
      string format,
      CultureInfo culture,
      DateTimeStyles style,
      out DateTimeOffset time);
  }
}
