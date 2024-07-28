// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.ProjectAnalysisHttpClientWrapper
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.ProjectAnalysis.WebApi;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers
{
  public class ProjectAnalysisHttpClientWrapper
  {
    private ProjectAnalysisHttpClient m_projectAnalysisHttpClient;
    private readonly ExponentialBackoffRetryInvoker m_expRetryInvoker;
    private readonly TraceMetaData m_traceMetaData;
    private readonly IIndexerFaultService m_faultService;
    private readonly int m_waitTimeInMs;
    private readonly int m_retryLimit;

    public ProjectAnalysisHttpClientWrapper(
      Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext,
      TraceMetaData traceMetadata)
    {
      this.m_projectAnalysisHttpClient = executionContext != null ? executionContext.RequestContext.GetRedirectedClientIfNeeded<ProjectAnalysisHttpClient>() : throw new ArgumentNullException(nameof (executionContext));
      this.m_faultService = executionContext.FaultService;
      this.m_expRetryInvoker = ExponentialBackoffRetryInvoker.Instance;
      this.m_traceMetaData = traceMetadata;
      this.m_waitTimeInMs = executionContext.ServiceSettings.PipelineSettings.CrawlOperationRetryIntervalInSec * 1000;
      this.m_retryLimit = executionContext.ServiceSettings.PipelineSettings.CrawlOperationRetryLimit;
    }

    internal void SetUpHttpClient(ProjectAnalysisHttpClient client) => this.m_projectAnalysisHttpClient = client;

    public virtual LanguageMetrics GetLanguageMetrics(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      ProjectLanguageAnalytics projectLanguageAnalytics = this.m_expRetryInvoker.InvokeWithFaultCheck<ProjectLanguageAnalytics>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<ProjectLanguageAnalytics>((Func<CancellationTokenSource, Task<ProjectLanguageAnalytics>>) (tokenSource => this.m_projectAnalysisHttpClient.GetProjectLanguageAnalyticsAsync(projectId, cancellationToken: tokenSource.Token)), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);
      return new LanguageMetrics(requestContext, projectLanguageAnalytics);
    }

    public virtual CodeActivities GetCodeMetrics(
      Guid projectId,
      int numOfDays,
      DateTime tillTime,
      AggregationType aggregationType = AggregationType.Daily)
    {
      return new CodeActivities(this.m_expRetryInvoker.InvokeWithFaultCheck<ProjectActivityMetrics>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<ProjectActivityMetrics>((Func<CancellationTokenSource, Task<ProjectActivityMetrics>>) (tokenSource => this.m_projectAnalysisHttpClient.GetProjectActivityMetricsAsync(projectId, tillTime.AddDays((double) (-1 * numOfDays)), aggregationType, cancellationToken: tokenSource.Token)), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData), tillTime);
    }

    public virtual CodeActivities GetRepositoryCodeMetrics(
      Guid projectId,
      Guid repositoryId,
      int numOfDays,
      DateTime tillTime,
      AggregationType aggregationType = AggregationType.Daily)
    {
      return new CodeActivities(this.m_expRetryInvoker.InvokeWithFaultCheck<RepositoryActivityMetrics>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<RepositoryActivityMetrics>((Func<CancellationTokenSource, Task<RepositoryActivityMetrics>>) (tokenSource => this.m_projectAnalysisHttpClient.GetRepositoryActivityMetricsAsync(projectId, repositoryId, tillTime.AddDays((double) (-1 * numOfDays)), aggregationType, cancellationToken: tokenSource.Token)), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData), tillTime);
    }

    public virtual IDictionary<Guid, CodeActivities> GetAllGitRepositoriesCodeActivities(
      Guid projectId,
      int numberOfDays,
      DateTime tillTime,
      AggregationType aggregationType)
    {
      int skip = 0;
      int top = 100;
      List<RepositoryActivityMetrics> source = new List<RepositoryActivityMetrics>();
      List<RepositoryActivityMetrics> repositoriesActivityMetrics;
      do
      {
        int num = numberOfDays != int.MinValue ? -numberOfDays : int.MaxValue;
        repositoriesActivityMetrics = this.GetGitRepositoriesActivityMetrics(projectId, tillTime.AddDays((double) num), aggregationType, skip, top);
        source.AddRange((IEnumerable<RepositoryActivityMetrics>) repositoriesActivityMetrics);
        skip += top;
      }
      while (repositoriesActivityMetrics.Count == top);
      return (IDictionary<Guid, CodeActivities>) source.Distinct<RepositoryActivityMetrics>().ToDictionary<RepositoryActivityMetrics, Guid, CodeActivities>((Func<RepositoryActivityMetrics, Guid>) (metrics => metrics.RepositoryId), (Func<RepositoryActivityMetrics, CodeActivities>) (metrics => new CodeActivities(metrics, tillTime)));
    }

    internal virtual List<RepositoryActivityMetrics> GetGitRepositoriesActivityMetrics(
      Guid projectId,
      DateTime fromDate,
      AggregationType aggregationType,
      int skip,
      int top)
    {
      return this.m_expRetryInvoker.InvokeWithFaultCheck<List<RepositoryActivityMetrics>>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<List<RepositoryActivityMetrics>>((Func<CancellationTokenSource, Task<List<RepositoryActivityMetrics>>>) (tokenSource => this.m_projectAnalysisHttpClient.GetGitRepositoriesActivityMetricsAsync(projectId, fromDate, aggregationType, skip, top, cancellationToken: tokenSource.Token)), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);
    }
  }
}
