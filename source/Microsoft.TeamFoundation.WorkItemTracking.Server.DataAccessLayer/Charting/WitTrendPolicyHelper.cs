// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Charting.WitTrendPolicyHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Reporting.DataServices;
using Microsoft.TeamFoundation.Reporting.DataServices.FeatureUtilities;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Charting
{
  internal class WitTrendPolicyHelper
  {
    internal static string SampleTimeKey = "WitTrendPolicyHelper.SampleTime";

    public static string TrendFieldRefName => "System.RevisedDate";

    public static int TrendFieldId => -5;

    public static bool IsHistoryInGroupBy(TransformOptions option) => option.GroupBy == WitTrendPolicyHelper.TrendFieldRefName;

    internal static IEnumerable<DateTime> DatesToUtc(
      IEnumerable<DateTime> historyRangeInstance,
      TimeZoneInfo timeZone)
    {
      return historyRangeInstance.Select<DateTime, DateTime>((Func<DateTime, DateTime>) (o => o.ToUtc(timeZone)));
    }

    public static bool IsHistoryInSeries(TransformOptions option) => option.GroupBy != WitTrendPolicyHelper.TrendFieldRefName && option.Series == WitTrendPolicyHelper.TrendFieldRefName;

    public static void ApplyRevisedDateForHistory(TransformOptions option)
    {
      if (string.IsNullOrWhiteSpace(option.GroupBy))
        option.GroupBy = WitTrendPolicyHelper.TrendFieldRefName;
      else if (option.GroupBy != WitTrendPolicyHelper.TrendFieldRefName && string.IsNullOrWhiteSpace(option.Series))
        option.Series = WitTrendPolicyHelper.TrendFieldRefName;
      else if (option.Series != WitTrendPolicyHelper.TrendFieldRefName && option.GroupBy != WitTrendPolicyHelper.TrendFieldRefName)
        throw new InvalidTransformOptionsException("WrongSeriesForTrendData");
    }

    internal static IEnumerable<DateTime> InstantiateRange(
      IEnumerable<FixedIntervalDateRange> ranges,
      string rangeName,
      DateTime anchorTime)
    {
      return (!string.IsNullOrEmpty(rangeName) ? (DateRangeBase) HistoryRange.FindNamedRange<FixedIntervalDateRange>(ranges, rangeName) : (DateRangeBase) DefaultDateRanges.Snapshot()).InstantiateFrom(anchorTime);
    }

    internal static IEnumerable<AggregatedRecord> SortResults(
      TransformOptions options,
      IComparer<AggregatedRecord> comparer,
      OrderBy orderBy,
      IDictionary<object, AggregatedRecord> results)
    {
      bool useGroup = WitTrendPolicyHelper.IsHistoryInGroupBy(options);
      IOrderedEnumerable<AggregatedRecord> source = results.Values.OrderBy<AggregatedRecord, object>((Func<AggregatedRecord, object>) (x => !useGroup ? x.Series : x.Group));
      if (orderBy == null)
        return (IEnumerable<AggregatedRecord>) source;
      return !(options.OrderBy.Direction == "descending") ? (IEnumerable<AggregatedRecord>) source.ThenBy<AggregatedRecord, AggregatedRecord>((Func<AggregatedRecord, AggregatedRecord>) (x => x), comparer) : (IEnumerable<AggregatedRecord>) source.ThenByDescending<AggregatedRecord, AggregatedRecord>((Func<AggregatedRecord, AggregatedRecord>) (x => x), comparer);
    }

    internal static DateTime SampleTime(IVssRequestContext requestContext)
    {
      DateTime dateTime;
      if (!requestContext.Items.TryGetValue<string, DateTime>(WitTrendPolicyHelper.SampleTimeKey, out dateTime))
      {
        dateTime = DateTime.UtcNow;
        dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0, 0, dateTime.Kind);
        requestContext.Items.Add(WitTrendPolicyHelper.SampleTimeKey, (object) dateTime);
      }
      return dateTime;
    }

    public static void EnforceTrendSettings(
      IEnumerable<TransformOptions> options,
      bool uselegacyRules)
    {
      IEnumerable<string> source = WitDataServiceCapabilityProvider.GetRangeOptions().Select<FixedIntervalDateRange, string>((Func<FixedIntervalDateRange, string>) (y => y.Name.ToLower()));
      foreach (TransformOptions option in options)
      {
        if (uselegacyRules)
        {
          WitTrendPolicyHelper.ApplyRevisedDateForHistory(option);
          if (option.HistoryRange == null || !source.Contains<string>(option.HistoryRange.ToLower()))
            throw new InvalidTransformOptionsException("InvalidRange");
        }
        else if (!string.IsNullOrEmpty(option.HistoryRange) && !source.Contains<string>(option.HistoryRange.ToLower()))
          throw new InvalidTransformOptionsException("InvalidRange");
      }
    }

    public static TimeZoneInfo GetUserTimeZoneInfo(IVssRequestContext RequestContext) => RequestContext.GetService<IUserPreferencesService>().GetUserPreferences(RequestContext).TimeZone;

    public static TimeZoneInfo GetCollectionTimeZoneInfo(IVssRequestContext RequestContext) => RequestContext.GetService<ICollectionPreferencesService>().GetCollectionTimeZone(RequestContext);
  }
}
