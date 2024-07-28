// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.Server.RepositoryActivityMetricsProvider
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 076482BC-74A4-4A35-9427-1E61C33D1FA6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.ProjectAnalysis.WebApi;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.ProjectAnalysis.Server
{
  public class RepositoryActivityMetricsProvider
  {
    public RepositoryActivityMetrics GetRepositoryActivityMetrics(
      IVssRequestContext tfsRequestContext,
      Guid projectId,
      Guid repositoryId,
      DateTime fromDate,
      AggregationType aggregationType)
    {
      DateTime utcNow = DateTime.UtcNow;
      this.ValidateArgumentsAndThrow(fromDate.ToUniversalTime(), aggregationType, utcNow);
      DateTime normalizedFromDateTime = ActivityMetricsProviderHelper.GetNormalizedFromDateTime(fromDate, aggregationType);
      int bucket1 = CodeMetricsUtil.ConvertToBucket(normalizedFromDateTime);
      int bucket2 = CodeMetricsUtil.ConvertToBucket(utcNow);
      if (projectId == repositoryId)
      {
        CodeMetrics tfvcProjectMetrics = ActivityMetricsProviderHelper.GetTfvcProjectMetrics(tfsRequestContext, projectId, bucket1, bucket2);
        return new RepositoryActivityMetrics()
        {
          CodeChangesCount = tfvcProjectMetrics.Changesets,
          CodeChangesTrend = (IList<CodeChangeTrendItem>) ActivityMetricsProviderHelper.GetCodeChangeTrendItem(tfvcProjectMetrics, aggregationType, normalizedFromDateTime.ToUniversalTime(), utcNow),
          RepositoryId = repositoryId
        };
      }
      GitActivityMetrics repositoryMetrics = this.GetGitRepositoryMetrics(tfsRequestContext, projectId, repositoryId, bucket1, bucket2);
      return new RepositoryActivityMetrics()
      {
        CodeChangesCount = repositoryMetrics.CommitsPushedCount,
        CodeChangesTrend = (IList<CodeChangeTrendItem>) ActivityMetricsProviderHelper.GetCodeChangeTrendItem(repositoryMetrics, aggregationType, normalizedFromDateTime.ToUniversalTime(), utcNow),
        RepositoryId = repositoryId
      };
    }

    public IList<RepositoryActivityMetrics> GetGitRepositoriesActivityMetrics(
      IVssRequestContext tfsRequestContext,
      Guid projectId,
      DateTime fromDate,
      AggregationType aggregationType,
      int skip,
      int take)
    {
      DateTime utcNow = DateTime.UtcNow;
      this.ValidateArgumentsAndThrow(fromDate.ToUniversalTime(), aggregationType, utcNow, skip, take);
      DateTime normalizedFromDateTime = ActivityMetricsProviderHelper.GetNormalizedFromDateTime(fromDate, aggregationType);
      int bucket1 = CodeMetricsUtil.ConvertToBucket(normalizedFromDateTime);
      int bucket2 = CodeMetricsUtil.ConvertToBucket(utcNow);
      IList<GitActivityMetrics> repositoriesMetrics;
      using (PerformanceTimer.StartMeasure(tfsRequestContext, "GetGitRepositoriesMetrics"))
        repositoriesMetrics = new GitMetricsProvider(tfsRequestContext).GetRepositoriesMetrics(projectId, bucket1, bucket2, skip, take);
      List<RepositoryActivityMetrics> repositoriesActivityMetrics = new List<RepositoryActivityMetrics>();
      foreach (GitActivityMetrics gitMetrics in (IEnumerable<GitActivityMetrics>) repositoriesMetrics)
        repositoriesActivityMetrics.Add(new RepositoryActivityMetrics()
        {
          CodeChangesCount = gitMetrics.CommitsPushedCount,
          CodeChangesTrend = (IList<CodeChangeTrendItem>) ActivityMetricsProviderHelper.GetCodeChangeTrendItem(gitMetrics, aggregationType, normalizedFromDateTime.ToUniversalTime(), utcNow),
          RepositoryId = gitMetrics.RepoScope.RepoId
        });
      return (IList<RepositoryActivityMetrics>) repositoriesActivityMetrics;
    }

    private GitActivityMetrics GetGitRepositoryMetrics(
      IVssRequestContext tfsRequestContext,
      Guid projectId,
      Guid repositoryId,
      int startBucket,
      int endBucket)
    {
      using (PerformanceTimer.StartMeasure(tfsRequestContext, nameof (GetGitRepositoryMetrics)))
        return new GitMetricsProvider(tfsRequestContext).GetRepositoryMetrics(projectId, repositoryId, startBucket, endBucket);
    }

    private void ValidateArgumentsAndThrow(
      DateTime from,
      AggregationType aggregationType,
      DateTime currentDateTime,
      int skip,
      int take)
    {
      ArgumentUtility.CheckForNonnegativeInt(skip, nameof (skip));
      ArgumentUtility.CheckBoundsInclusive(take, 1, 100, nameof (take));
      this.ValidateArgumentsAndThrow(from, aggregationType, currentDateTime);
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
