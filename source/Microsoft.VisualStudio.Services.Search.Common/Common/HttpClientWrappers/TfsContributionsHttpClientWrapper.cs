// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.TfsContributionsHttpClientWrapper
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers
{
  public class TfsContributionsHttpClientWrapper
  {
    private ContributionsHttpClient m_tfsContribtionHttpClient;
    private readonly ExponentialBackoffRetryInvoker m_expRetryInvoker;
    private readonly TraceMetaData m_traceMetaData;
    private readonly IIndexerFaultService m_faultService;
    private readonly int m_waitTimeInMs;
    private readonly int m_retryLimit;
    private static readonly Guid s_tfsServiceInstanceType = ServiceInstanceTypes.TFS;
    internal const string TfsMetricsDataProviderId = "ms.vss-tfs-web.project-overview-data-provider";
    private const string ScopeKey = "scope";
    private const string NumOfDaysKey = "numOfDays";
    private const string ProjectIdKey = "projectId";
    private const string CodeMetricsScope = "codemetrics";

    public TfsContributionsHttpClientWrapper(
      Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext,
      TraceMetaData traceMetadata)
    {
      if (executionContext == null)
        throw new ArgumentNullException(nameof (executionContext));
      this.m_tfsContribtionHttpClient = executionContext.RequestContext.GetRedirectedClientIfNeeded<ContributionsHttpClient>(TfsContributionsHttpClientWrapper.s_tfsServiceInstanceType);
      this.m_faultService = executionContext.FaultService;
      this.m_expRetryInvoker = ExponentialBackoffRetryInvoker.Instance;
      this.m_traceMetaData = traceMetadata;
      this.m_waitTimeInMs = executionContext.ServiceSettings.PipelineSettings.CrawlOperationRetryIntervalInSec * 1000;
      this.m_retryLimit = executionContext.ServiceSettings.PipelineSettings.CrawlOperationRetryLimit;
    }

    internal void SetUpHttpClient(ContributionsHttpClient client) => this.m_tfsContribtionHttpClient = client;

    public virtual CodeActivities GetCodeMetrics(Guid projectId, int numOfDays)
    {
      DataProviderQuery query = new DataProviderQuery()
      {
        ContributionIds = new List<string>()
        {
          "ms.vss-tfs-web.project-overview-data-provider"
        },
        Context = new DataProviderContext()
        {
          Properties = new Dictionary<string, object>()
          {
            ["scope"] = (object) "codemetrics",
            [nameof (numOfDays)] = (object) numOfDays,
            [nameof (projectId)] = (object) projectId
          }
        }
      };
      return TfsContributionsHttpClientWrapper.ParseResult(this.m_expRetryInvoker.InvokeWithFaultCheck<DataProviderResult>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<DataProviderResult>((Func<CancellationTokenSource, Task<DataProviderResult>>) (tokenSource => this.m_tfsContribtionHttpClient.QueryDataProvidersAsync(query)), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData));
    }

    internal static CodeActivities ParseResult(DataProviderResult dataProviderResult)
    {
      if (dataProviderResult?.Data == null || dataProviderResult.Exceptions != null || !dataProviderResult.Data.ContainsKey("ms.vss-tfs-web.project-overview-data-provider") || dataProviderResult.Data["ms.vss-tfs-web.project-overview-data-provider"] == null)
        throw new SearchException(FormattableString.Invariant(FormattableStringFactory.Create("Unable to get the metrics from tfs dataprovider : {0}", (object) "ms.vss-tfs-web.project-overview-data-provider")));
      TfsContributionsHttpClientWrapper.CodeMetrics metrics;
      if (!JsonUtilities.TryDeserialize<TfsContributionsHttpClientWrapper.CodeMetrics>(dataProviderResult.Data["ms.vss-tfs-web.project-overview-data-provider"].ToString(), out metrics))
        throw new SearchException("Unable to deserialise tfs dataprovider result.");
      return new CodeActivities(metrics);
    }

    public class CodeMetrics
    {
      public TfsContributionsHttpClientWrapper.GitMetrics GitMetrics { get; set; }

      public TfsContributionsHttpClientWrapper.TfvcMetrics TfvcMetrics { get; set; }
    }

    public class GitMetrics
    {
      public int CommitsPushedCount { get; set; }

      public int PullRequestsCreatedCount { get; set; }

      public int PullRequestsCompletedCount { get; set; }

      public int AuthorsCount { get; set; }

      public int[] CommitsTrend { get; set; }
    }

    public class TfvcMetrics
    {
      public int Changesets { get; set; }

      public int Authors { get; set; }

      public int[] ChangesetsTrend { get; set; }
    }
  }
}
