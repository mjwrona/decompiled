// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.TrendDateTimeHelper
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Reporting.DataServices.FeatureUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Reporting.DataServices
{
  public static class TrendDateTimeHelper
  {
    public static string SampleTimeKey = "TrendDateTimeHelper.SampleTime";

    public static TimeZoneInfo GetCollectionTimeZoneInfo(IVssRequestContext RequestContext) => RequestContext.GetService<ICollectionPreferencesService>().GetCollectionTimeZone(RequestContext);

    public static DateTime SampleTime(IVssRequestContext requestContext)
    {
      DateTime dateTime;
      if (!requestContext.Items.TryGetValue<DateTime>(TrendDateTimeHelper.SampleTimeKey, out dateTime))
      {
        dateTime = DateTime.UtcNow;
        dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0, 0, dateTime.Kind);
        requestContext.Items.Add(TrendDateTimeHelper.SampleTimeKey, (object) dateTime);
      }
      return dateTime;
    }

    public static IEnumerable<DateTime> InstantiateRange(
      IEnumerable<FixedIntervalDateRange> ranges,
      string rangeName,
      DateTime anchorTime)
    {
      return (!string.IsNullOrEmpty(rangeName) ? (DateRangeBase) HistoryRange.FindNamedRange<FixedIntervalDateRange>(ranges, rangeName) : (DateRangeBase) DefaultDateRanges.Snapshot()).InstantiateFrom(anchorTime);
    }

    public static IEnumerable<DateTime> DatesToUtc(
      IEnumerable<DateTime> historyRangeInstance,
      TimeZoneInfo timeZone)
    {
      return historyRangeInstance.Select<DateTime, DateTime>((Func<DateTime, DateTime>) (o => o.ToUtc(timeZone)));
    }
  }
}
