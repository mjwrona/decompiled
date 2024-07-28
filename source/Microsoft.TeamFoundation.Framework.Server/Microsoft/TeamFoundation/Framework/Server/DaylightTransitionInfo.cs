// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DaylightTransitionInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class DaylightTransitionInfo
  {
    public DaylightTransitionInfo(
      string timeZoneId,
      DateTime startDate,
      DateTime endDate,
      TimeSpan delta)
    {
      this.TimeZoneId = timeZoneId;
      this.StartDate = startDate;
      this.EndDate = endDate;
      this.Delta = delta;
    }

    public string TimeZoneId { get; private set; }

    public DateTime StartDate { get; private set; }

    public DateTime EndDate { get; private set; }

    public TimeSpan Delta { get; private set; }

    public static List<DaylightTransitionInfo> GetDaylightTransitions(
      TimeZoneInfo tz,
      int startRange,
      int endRange)
    {
      List<DaylightTransitionInfo> daylightTransitions = new List<DaylightTransitionInfo>();
      if (!tz.SupportsDaylightSavingTime || startRange > endRange)
        return daylightTransitions;
      int val1_1 = startRange - 1;
      int val1_2 = endRange + 1;
      DateTime? nullable = new DateTime?();
      foreach (TimeZoneInfo.AdjustmentRule adjustmentRule in tz.GetAdjustmentRules())
      {
        int year = Math.Max(val1_1, adjustmentRule.DateStart.Year);
        for (int index = Math.Min(val1_2, adjustmentRule.DateEnd.Year); year <= index; ++year)
        {
          DateTime dateTime1 = DaylightTransitionInfo.ToDateTime(adjustmentRule.DaylightTransitionStart, year, tz.BaseUtcOffset);
          DateTime dateTime2 = DaylightTransitionInfo.ToDateTime(adjustmentRule.DaylightTransitionEnd, year, tz.BaseUtcOffset + adjustmentRule.DaylightDelta);
          if (dateTime1 < dateTime2)
          {
            if (dateTime1 >= adjustmentRule.DateStart && dateTime2 <= adjustmentRule.DateEnd)
            {
              if (DaylightTransitionInfo.WithinRange(startRange, endRange, dateTime1, dateTime2))
                daylightTransitions.Add(new DaylightTransitionInfo(tz.Id, dateTime1, dateTime2, adjustmentRule.DaylightDelta));
              nullable = new DateTime?();
            }
            else if (dateTime1 >= adjustmentRule.DateStart && dateTime2 > adjustmentRule.DateEnd && adjustmentRule.DateEnd >= dateTime1)
              nullable = new DateTime?(dateTime1);
            else if (dateTime1 < adjustmentRule.DateStart && dateTime2 <= adjustmentRule.DateEnd && adjustmentRule.DateStart <= dateTime2)
            {
              if (nullable.HasValue)
              {
                if (DaylightTransitionInfo.WithinRange(startRange, endRange, nullable.Value, dateTime2))
                  daylightTransitions.Add(new DaylightTransitionInfo(tz.Id, nullable.Value, dateTime2, adjustmentRule.DaylightDelta));
                nullable = new DateTime?();
              }
            }
            else
              nullable = new DateTime?();
          }
          else
          {
            if (nullable.HasValue && dateTime2 <= adjustmentRule.DateEnd && DaylightTransitionInfo.WithinRange(startRange, endRange, nullable.Value, dateTime2))
              daylightTransitions.Add(new DaylightTransitionInfo(tz.Id, nullable.Value, dateTime2, adjustmentRule.DaylightDelta));
            nullable = !(dateTime1 >= adjustmentRule.DateStart) ? new DateTime?() : new DateTime?(dateTime1);
          }
        }
      }
      return daylightTransitions;
    }

    private static bool WithinRange(
      int rangeStartYear,
      int rangeEndYear,
      DateTime daylightStart,
      DateTime daylightEnd)
    {
      return daylightStart.Year <= rangeEndYear && daylightEnd.Year >= rangeStartYear;
    }

    private static DateTime ToDateTime(
      TimeZoneInfo.TransitionTime tt,
      int year,
      TimeSpan utcOffset)
    {
      if (tt.IsFixedDateRule)
        return new DateTime(year, tt.Month, tt.Day, 0, 0, 0, DateTimeKind.Utc).Add(tt.TimeOfDay.TimeOfDay - utcOffset);
      DateTime dateTime = new DateTime(year, tt.Month, 1, 0, 0, 0, DateTimeKind.Utc);
      while (dateTime.DayOfWeek != tt.DayOfWeek)
        dateTime = dateTime.AddDays(1.0);
      if (tt.Week > 1)
        dateTime = dateTime.AddDays((double) (7 * (tt.Week - 1)));
      while (dateTime.Month > tt.Month)
        dateTime = dateTime.AddDays(-7.0);
      return dateTime.Add(tt.TimeOfDay.TimeOfDay - utcOffset);
    }
  }
}
