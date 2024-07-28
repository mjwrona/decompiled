// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Billing.UsageInfoServiceExtensions
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Billing
{
  public static class UsageInfoServiceExtensions
  {
    public const short DefaultLookBackDurationInDays = 7;
    public const short MaximumFeedsToElect = 100;

    public static IEnumerable<Tuple<DateTimeOffset, string>> GetJobResults(
      IVssRequestContext requestContext,
      Guid jobId,
      DateTimeOffset maxTime,
      string jobResultMessageMetric,
      short lookBackWindowInDays = 7)
    {
      return requestContext.GetService<ITeamFoundationJobService>().QueryJobHistory(requestContext, jobId).Where<TeamFoundationJobHistoryEntry>((Func<TeamFoundationJobHistoryEntry, bool>) (job => job.Result == TeamFoundationJobResult.Succeeded)).Where<TeamFoundationJobHistoryEntry>((Func<TeamFoundationJobHistoryEntry, bool>) (job => job.ResultMessage.Contains(jobResultMessageMetric))).Select(job => new
      {
        job = job,
        jobResult = Tuple.Create<DateTimeOffset, string>(job.ExecutionStartTime.ToDateTimeOffset(), job.ResultMessage)
      }).Where(_param1 => _param1.jobResult.Item1 > maxTime.Subtract(TimeSpan.FromDays((double) lookBackWindowInDays))).Where(_param1 => _param1.jobResult.Item1 <= maxTime).Select(_param1 => _param1.jobResult);
    }

    public static ConcurrentDictionary<string, ulong> GetTopLogicalSizeByFeed(
      ConcurrentDictionary<string, ulong> logicalSizeByFeed)
    {
      short count = 100;
      IOrderedEnumerable<KeyValuePair<string, ulong>> source = logicalSizeByFeed.OrderByDescending<KeyValuePair<string, ulong>, ulong>((Func<KeyValuePair<string, ulong>, ulong>) (x => x.Value));
      ConcurrentDictionary<string, ulong> logicalSizeByFeed1 = new ConcurrentDictionary<string, ulong>(source.Take<KeyValuePair<string, ulong>>((int) count));
      if (logicalSizeByFeed.Count<KeyValuePair<string, ulong>>() > (int) count)
      {
        ulong sumOfSmallFeeds = source.Skip<KeyValuePair<string, ulong>>((int) count).Sum<KeyValuePair<string, ulong>>((Func<KeyValuePair<string, ulong>, ulong>) (x => x.Value));
        long num = (long) logicalSizeByFeed1.AddOrUpdate("Others", sumOfSmallFeeds, (Func<string, ulong, ulong>) ((key, existing) => existing + sumOfSmallFeeds));
      }
      return logicalSizeByFeed1;
    }

    public static bool IsBillingEnabledForDomainsAsync(IVssRequestContext requestContext) => !requestContext.ExecutionEnvironment.IsOnPremisesDeployment && requestContext.IsFeatureEnabled("BlobStore.Features.EnableMultiDomainBilling");
  }
}
