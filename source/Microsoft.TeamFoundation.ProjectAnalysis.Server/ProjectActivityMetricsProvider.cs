// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.Server.ProjectActivityMetricsProvider
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 076482BC-74A4-4A35-9427-1E61C33D1FA6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.ProjectAnalysis.WebApi;
using Microsoft.TeamFoundation.VersionControl.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.ProjectAnalysis.Server
{
  public class ProjectActivityMetricsProvider
  {
    public ProjectActivityMetrics GetProjectActivityMetrics(
      IVssRequestContext tfsRequestContext,
      Guid projectId,
      DateTime fromDate,
      AggregationType aggregationType)
    {
      DateTime utcNow = DateTime.UtcNow;
      this.ValidateArgumentsAndThrow(fromDate.ToUniversalTime(), aggregationType, utcNow);
      DateTime normalizedFromDateTime = ActivityMetricsProviderHelper.GetNormalizedFromDateTime(fromDate, aggregationType);
      int bucket1 = CodeMetricsUtil.ConvertToBucket(normalizedFromDateTime);
      int bucket2 = CodeMetricsUtil.ConvertToBucket(utcNow);
      return this.MergeGitAndTfvcMetrics(this.GetGitProjectMetrics(tfsRequestContext, projectId, bucket1, bucket2), ActivityMetricsProviderHelper.GetTfvcProjectMetrics(tfsRequestContext, projectId, bucket1, bucket2), aggregationType, normalizedFromDateTime.ToUniversalTime(), utcNow, projectId);
    }

    internal ProjectActivityMetrics MergeGitAndTfvcMetrics(
      GitActivityMetrics gitMetrics,
      CodeMetrics tfvcMetrics,
      AggregationType aggregationType,
      DateTime normalizedFromDate,
      DateTime end,
      Guid projectId)
    {
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      int num4 = 0;
      if (gitMetrics != null)
      {
        num1 += gitMetrics.PullRequestsCompletedCount;
        num2 += gitMetrics.PullRequestsCreatedCount;
        num3 += gitMetrics.AuthorsCount;
        num4 += gitMetrics.CommitsPushedCount;
      }
      if (tfvcMetrics != null)
      {
        num4 += tfvcMetrics.Changesets;
        num3 += tfvcMetrics.Authors;
      }
      return new ProjectActivityMetrics()
      {
        ProjectId = projectId,
        PullRequestsCompletedCount = num1,
        PullRequestsCreatedCount = num2,
        AuthorsCount = num3,
        CodeChangesCount = num4,
        CodeChangesTrend = (IList<CodeChangeTrendItem>) ActivityMetricsProviderHelper.GetCodeChangeTrendItem(gitMetrics, tfvcMetrics, aggregationType, normalizedFromDate, end)
      };
    }

    private GitActivityMetrics GetGitProjectMetrics(
      IVssRequestContext tfsRequestContext,
      Guid projectId,
      int startBucket,
      int endBucket)
    {
      using (PerformanceTimer.StartMeasure(tfsRequestContext, nameof (GetGitProjectMetrics)))
        return new GitMetricsProvider(tfsRequestContext).GetProjectMetrics(projectId, startBucket, endBucket);
    }

    private void ValidateArgumentsAndThrow(
      DateTime from,
      AggregationType aggregationType,
      DateTime currentDateTime)
    {
      if (from.Date < currentDateTime.AddDays(-31.0))
        throw new ArgumentOutOfRangeException("from time can be only within last 30 days");
      if (from.Date > currentDateTime)
        throw new ArgumentOutOfRangeException("from time cannot be in future");
      if (aggregationType != AggregationType.Hourly && aggregationType != AggregationType.Daily)
        throw new ArgumentOutOfRangeException("Unsupported Aggregation Type");
    }
  }
}
