// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.SocialHttpClientWrapper
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Social.WebApi;
using Microsoft.VisualStudio.Services.Social.WebApi.Clients;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers
{
  public class SocialHttpClientWrapper
  {
    private readonly SocialHttpClient m_socialHttpClient;
    private readonly ExponentialBackoffRetryInvoker m_expRetryInvoker;
    private readonly TraceMetaData m_traceMetaData;
    private readonly IIndexerFaultService m_faultService;
    private readonly int m_waitTimeInMs;
    private readonly int m_retryLimit;

    protected SocialHttpClientWrapper()
    {
    }

    public SocialHttpClientWrapper(Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext, TraceMetaData traceMetadata)
    {
      if (executionContext == null)
        throw new ArgumentNullException(nameof (executionContext));
      this.m_socialHttpClient = executionContext.RequestContext.GetRedirectedClientIfNeeded<SocialHttpClient>(ServiceInstanceTypes.TFS);
      this.m_faultService = executionContext.FaultService;
      this.m_expRetryInvoker = ExponentialBackoffRetryInvoker.Instance;
      this.m_traceMetaData = traceMetadata;
      this.m_waitTimeInMs = executionContext.ServiceSettings.PipelineSettings.CrawlOperationRetryIntervalInSec * 1000;
      this.m_retryLimit = executionContext.ServiceSettings.PipelineSettings.CrawlOperationRetryLimit;
    }

    public virtual SocialEngagementActivities GetSocialEngagementActivityAsync(Guid artifactId)
    {
      string type = "Project";
      ArtifactScope artifactScope = new ArtifactScope()
      {
        Type = "Project",
        Id = artifactId.ToString()
      };
      return new SocialEngagementActivities(this.m_expRetryInvoker.InvokeWithFaultCheck<SocialEngagementAggregateMetric>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<SocialEngagementAggregateMetric>((Func<CancellationTokenSource, Task<SocialEngagementAggregateMetric>>) (tokenSource => this.m_socialHttpClient.GetSocialEngagementAggregateMetricAsync(type, artifactId.ToString(), SocialEngagementType.Likes, artifactScope.Type, artifactScope.Id, cancellationToken: tokenSource.Token)), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData));
    }
  }
}
