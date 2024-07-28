// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.Server.ActivityMetricsProviderHelper
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 076482BC-74A4-4A35-9427-1E61C33D1FA6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.ProjectAnalysis.WebApi;
using Microsoft.TeamFoundation.VersionControl.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.ProjectAnalysis.Server
{
  internal static class ActivityMetricsProviderHelper
  {
    internal static CodeMetrics GetTfvcProjectMetrics(
      IVssRequestContext tfsRequestContext,
      Guid projectId,
      int startBucket,
      int endBucket)
    {
      TeamFoundationVersionControlService service = tfsRequestContext.GetService<TeamFoundationVersionControlService>();
      using (PerformanceTimer.StartMeasure(tfsRequestContext, nameof (GetTfvcProjectMetrics)))
        return service.QueryCodeMetrics(tfsRequestContext, projectId, startBucket, endBucket);
    }

    internal static List<CodeChangeTrendItem> GetCodeChangeTrendItem(
      GitActivityMetrics gitMetrics,
      AggregationType aggregationType,
      DateTime normalizedFromDate,
      DateTime end)
    {
      return ActivityMetricsProviderHelper.GetCodeChangeTrendItem(gitMetrics, (CodeMetrics) null, aggregationType, normalizedFromDate, end);
    }

    internal static List<CodeChangeTrendItem> GetCodeChangeTrendItem(
      CodeMetrics tfvcMetrics,
      AggregationType aggregationType,
      DateTime normalizedFromDate,
      DateTime end)
    {
      return ActivityMetricsProviderHelper.GetCodeChangeTrendItem((GitActivityMetrics) null, tfvcMetrics, aggregationType, normalizedFromDate, end);
    }

    internal static List<CodeChangeTrendItem> GetCodeChangeTrendItem(
      GitActivityMetrics gitMetrics,
      CodeMetrics tfvcMetrics,
      AggregationType aggregationType,
      DateTime normalizedFromDate,
      DateTime end)
    {
      List<CommitsTrendItem> commitsTrendItemList = new List<CommitsTrendItem>();
      if (gitMetrics != null && gitMetrics.CommitsTrend != null)
        commitsTrendItemList = gitMetrics.CommitsTrend;
      List<ChangesetsTrendItem> changesetsTrendItemList = new List<ChangesetsTrendItem>();
      if (tfvcMetrics != null)
        changesetsTrendItemList = tfvcMetrics.ChangesetsTrend;
      Dictionary<DateTime, int> source = new Dictionary<DateTime, int>();
      if (AggregationType.Hourly.Equals((object) aggregationType))
      {
        for (DateTime key = normalizedFromDate; key <= end; key = key.AddHours(1.0))
          source[key] = 0;
        foreach (CommitsTrendItem commitsTrendItem in commitsTrendItemList)
          source[commitsTrendItem.TimeStamp] += commitsTrendItem.CommitsCount;
        foreach (ChangesetsTrendItem changesetsTrendItem in changesetsTrendItemList)
          source[changesetsTrendItem.TimeStamp] += changesetsTrendItem.ChangesetsCount;
      }
      else
      {
        for (DateTime key = normalizedFromDate; key <= end; key = key.AddDays(1.0))
          source[key] = 0;
        foreach (CommitsTrendItem commitsTrendItem in commitsTrendItemList)
          source[commitsTrendItem.TimeStamp.Date] += commitsTrendItem.CommitsCount;
        foreach (ChangesetsTrendItem changesetsTrendItem in changesetsTrendItemList)
          source[changesetsTrendItem.TimeStamp.Date] += changesetsTrendItem.ChangesetsCount;
      }
      return source.Select<KeyValuePair<DateTime, int>, CodeChangeTrendItem>((Func<KeyValuePair<DateTime, int>, CodeChangeTrendItem>) (x => new CodeChangeTrendItem()
      {
        Time = x.Key,
        Value = x.Value
      })).ToList<CodeChangeTrendItem>();
    }

    internal static DateTime GetNormalizedFromDateTime(
      DateTime from,
      AggregationType aggregationType)
    {
      if (AggregationType.Daily.Equals((object) aggregationType))
        return from.ToUniversalTime().Date;
      DateTime dateTime = from.ToUniversalTime();
      dateTime = dateTime.Date;
      return dateTime.AddHours((double) from.Hour);
    }
  }
}
