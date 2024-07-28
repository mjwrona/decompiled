// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Charts.DateTimeExtensionMethods
// Assembly: Microsoft.Azure.Boards.Charts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EABADF19-3537-403E-8E3C-4185CE6D1F3B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.Charts.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.Boards.Charts
{
  public static class DateTimeExtensionMethods
  {
    public static DateTime GetEndOfDayUtc(this DateTime dateTime, TimeZoneInfo timeZoneInfo)
    {
      DateTime dateOnly = dateTime.Date;
      DateTime dateTime1 = dateOnly.AddDays(1.0).AddMilliseconds(-1.0);
      if (timeZoneInfo.IsInvalidTime(dateTime1))
      {
        TimeZoneInfo.AdjustmentRule adjustmentRule = ((IEnumerable<TimeZoneInfo.AdjustmentRule>) timeZoneInfo.GetAdjustmentRules()).FirstOrDefault<TimeZoneInfo.AdjustmentRule>((Func<TimeZoneInfo.AdjustmentRule, bool>) (rule => rule.DateStart <= dateOnly && dateOnly <= rule.DateEnd));
        if (adjustmentRule != null)
          dateTime1 = new DateTime(dateOnly.Ticks + adjustmentRule.DaylightTransitionStart.TimeOfDay.Ticks - 10000L);
        if (timeZoneInfo.IsInvalidTime(dateTime1))
          throw new TeamFoundationServiceException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The calculated end of day was invalid. Start DateTime: {0}, TimeZoneInfo: {1}.", (object) dateTime.ToString("u"), (object) timeZoneInfo.ToString()));
      }
      return TimeZoneInfo.ConvertTime(dateTime1, timeZoneInfo, TimeZoneInfo.Utc);
    }
  }
}
