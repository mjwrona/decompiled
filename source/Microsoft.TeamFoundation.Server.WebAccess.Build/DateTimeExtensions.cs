// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.DateTimeExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class DateTimeExtensions
  {
    private static readonly double s_minuteInSeconds = 60.0;
    private static readonly double s_hourInSeconds = DateTimeExtensions.s_minuteInSeconds * 60.0;
    private static readonly double s_dayInSeconds = DateTimeExtensions.s_hourInSeconds * 24.0;
    private static readonly double s_weekInSeconds = DateTimeExtensions.s_dayInSeconds * 7.0;
    private static readonly double s_monthInSeconds = DateTimeExtensions.s_dayInSeconds * 365.0 / 12.0;
    private static readonly double s_yearInSeconds = DateTimeExtensions.s_dayInSeconds * 365.0;
    private static readonly DateTimeExtensions.AgoFormatSpec[] s_agoFormatSpecs = new DateTimeExtensions.AgoFormatSpec[13]
    {
      new DateTimeExtensions.AgoFormatSpec()
      {
        Limit = DateTimeExtensions.s_minuteInSeconds,
        Format = WACommonResources.AgoLessThanAMinute,
        DivisibleArgument = 0.0
      },
      new DateTimeExtensions.AgoFormatSpec()
      {
        Limit = DateTimeExtensions.s_minuteInSeconds * 1.5,
        Format = WACommonResources.AgoAMinute,
        DivisibleArgument = 0.0
      },
      new DateTimeExtensions.AgoFormatSpec()
      {
        Limit = DateTimeExtensions.s_hourInSeconds,
        Format = WACommonResources.AgoMinutes,
        DivisibleArgument = DateTimeExtensions.s_minuteInSeconds
      },
      new DateTimeExtensions.AgoFormatSpec()
      {
        Limit = DateTimeExtensions.s_hourInSeconds * 1.5,
        Format = WACommonResources.AgoAnHour,
        DivisibleArgument = 0.0
      },
      new DateTimeExtensions.AgoFormatSpec()
      {
        Limit = DateTimeExtensions.s_dayInSeconds,
        Format = WACommonResources.AgoHours,
        DivisibleArgument = DateTimeExtensions.s_hourInSeconds
      },
      new DateTimeExtensions.AgoFormatSpec()
      {
        Limit = DateTimeExtensions.s_dayInSeconds * 1.5,
        Format = WACommonResources.AgoADay,
        DivisibleArgument = 0.0
      },
      new DateTimeExtensions.AgoFormatSpec()
      {
        Limit = DateTimeExtensions.s_weekInSeconds,
        Format = WACommonResources.AgoDays,
        DivisibleArgument = DateTimeExtensions.s_dayInSeconds
      },
      new DateTimeExtensions.AgoFormatSpec()
      {
        Limit = DateTimeExtensions.s_weekInSeconds * 1.5,
        Format = WACommonResources.AgoAWeek,
        DivisibleArgument = 0.0
      },
      new DateTimeExtensions.AgoFormatSpec()
      {
        Limit = DateTimeExtensions.s_monthInSeconds,
        Format = WACommonResources.AgoWeeks,
        DivisibleArgument = DateTimeExtensions.s_weekInSeconds
      },
      new DateTimeExtensions.AgoFormatSpec()
      {
        Limit = DateTimeExtensions.s_monthInSeconds * 1.5,
        Format = WACommonResources.AgoAMonth,
        DivisibleArgument = 0.0
      },
      new DateTimeExtensions.AgoFormatSpec()
      {
        Limit = DateTimeExtensions.s_yearInSeconds,
        Format = WACommonResources.AgoMonths,
        DivisibleArgument = DateTimeExtensions.s_monthInSeconds
      },
      new DateTimeExtensions.AgoFormatSpec()
      {
        Limit = DateTimeExtensions.s_yearInSeconds * 1.5,
        Format = WACommonResources.AgoAYear,
        DivisibleArgument = 0.0
      },
      new DateTimeExtensions.AgoFormatSpec()
      {
        Limit = double.MaxValue,
        Format = WACommonResources.AgoYears,
        DivisibleArgument = DateTimeExtensions.s_yearInSeconds
      }
    };

    public static string Ago(this DateTime date)
    {
      string str = DateTimeExtensions.FormatTimeSpan(DateTime.UtcNow - date.ToUniversalTime());
      if (string.IsNullOrEmpty(str))
        str = date.ToString((IFormatProvider) CultureInfo.CurrentCulture);
      return str;
    }

    public static string Friendly(this DateTime date) => date.Friendly(DateTime.Now);

    public static string Friendly(this DateTime date, DateTime now)
    {
      DateTime localTime1 = now.ToLocalTime();
      DateTime localTime2 = date.ToLocalTime();
      TimeSpan difference = localTime1 - localTime2;
      string str1 = (string) null;
      List<KeyValuePair<double, string>> keyValuePairList = new List<KeyValuePair<double, string>>();
      keyValuePairList.Add(new KeyValuePair<double, string>(DateTimeExtensions.s_dayInSeconds, DateTimeExtensions.FormatTimeSpan(difference)));
      DateTime dateTime = new DateTime(localTime1.Year, localTime1.Month, localTime1.Day).Subtract(TimeSpan.FromDays((double) ((localTime1.DayOfWeek - CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek + 7) % 7)));
      if (localTime2 >= dateTime)
        keyValuePairList.Add(new KeyValuePair<double, string>((localTime1 - dateTime).TotalSeconds, localTime2.ToString("dddd")));
      keyValuePairList.Add(new KeyValuePair<double, string>(double.MaxValue, localTime2.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern)));
      foreach (KeyValuePair<double, string> keyValuePair in keyValuePairList)
      {
        double key = keyValuePair.Key;
        string str2 = keyValuePair.Value;
        if (difference.TotalSeconds <= key)
        {
          str1 = str2;
          break;
        }
      }
      if (string.IsNullOrEmpty(str1))
        str1 = date.ToString((IFormatProvider) CultureInfo.CurrentCulture);
      return str1;
    }

    public static string RelateToPresent(this DateTime startDate)
    {
      DateTime now = DateTime.Now;
      DateTime localTime = startDate.ToLocalTime();
      TimeSpan difference = now - localTime;
      if (difference.TotalHours < 7.0)
        return DateTimeExtensions.FormatTimeSpan(difference);
      if (localTime.Date == now.Date)
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, WACommonResources.TimeStampTodayAt, (object) localTime.ToShortTimeString());
      if ((localTime + TimeSpan.FromDays(1.0)).Date == now.Date)
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, WACommonResources.TimeStampYesterdayAt, (object) localTime.ToShortTimeString());
      return difference.TotalDays <= 5.0 ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, WACommonResources.TimeStampYesterdayAt, (object) localTime.ToString("dddd", (IFormatProvider) CultureInfo.CurrentCulture), (object) localTime.ToShortTimeString()) : string.Format((IFormatProvider) CultureInfo.CurrentCulture, WACommonResources.TimeStampYesterdayAt, (object) localTime.ToString("dddd", (IFormatProvider) CultureInfo.CurrentCulture), (object) localTime.ToString("g", (IFormatProvider) CultureInfo.CurrentCulture));
    }

    private static string FormatTimeSpan(TimeSpan difference)
    {
      double totalSeconds = difference.TotalSeconds;
      foreach (DateTimeExtensions.AgoFormatSpec agoFormatSpec in DateTimeExtensions.s_agoFormatSpecs)
      {
        double num1 = totalSeconds;
        double num2 = agoFormatSpec.Limit;
        if (agoFormatSpec.DivisibleArgument > 0.0)
        {
          num1 = Math.Round(num1 / agoFormatSpec.DivisibleArgument);
          num2 = Math.Round(num2 / agoFormatSpec.DivisibleArgument);
        }
        if (num1 < num2)
          return agoFormatSpec.DivisibleArgument > 0.0 ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, agoFormatSpec.Format, (object) num1) : agoFormatSpec.Format;
      }
      return string.Empty;
    }

    private class AgoFormatSpec
    {
      public double Limit { get; set; }

      public string Format { get; set; }

      public double DivisibleArgument { get; set; }
    }
  }
}
