// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.WikiHttpClientWrapper
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers
{
  public class WikiHttpClientWrapper
  {
    private readonly WikiHttpClient m_wikiHttpClient;
    private readonly ExponentialBackoffRetryInvoker m_expRetryInvoker;
    private readonly TraceMetaData m_traceMetaData;
    private readonly IIndexerFaultService m_faultService;
    private readonly int m_waitTimeInMs;
    private readonly int m_retryLimit;

    protected WikiHttpClientWrapper()
    {
    }

    public WikiHttpClientWrapper(Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext, TraceMetaData traceMetadata)
    {
      this.m_wikiHttpClient = executionContext != null ? executionContext.RequestContext.GetRedirectedClientIfNeeded<WikiHttpClient>(executionContext.RequestContext.GetHttpClientOptionsForEventualReadConsistencyLevel("Search.Server.Wiki.EnableReadReplica")) : throw new ArgumentNullException(nameof (executionContext));
      this.m_faultService = executionContext.FaultService;
      this.m_expRetryInvoker = ExponentialBackoffRetryInvoker.Instance;
      this.m_traceMetaData = traceMetadata;
      this.m_waitTimeInMs = executionContext.ServiceSettings.PipelineSettings.CrawlOperationRetryIntervalInSec * 1000;
      this.m_retryLimit = executionContext.ServiceSettings.PipelineSettings.CrawlOperationRetryLimit;
    }

    public virtual IEnumerable<GitRepository> GetWikiRepositoriesAsync()
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      try
      {
        return this.m_expRetryInvoker.InvokeWithFaultCheck<IEnumerable<GitRepository>>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<List<Microsoft.TeamFoundation.Wiki.WebApi.Wiki>>((Func<CancellationTokenSource, Task<List<Microsoft.TeamFoundation.Wiki.WebApi.Wiki>>>) (tokenSource => this.m_wikiHttpClient.GetWikisAsync(cancellationToken: tokenSource.Token)), this.m_waitTimeInMs, this.m_traceMetaData).Select<Microsoft.TeamFoundation.Wiki.WebApi.Wiki, GitRepository>((Func<Microsoft.TeamFoundation.Wiki.WebApi.Wiki, GitRepository>) (wikiRepo => wikiRepo.Repository))), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);
      }
      finally
      {
        stopwatch.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("GetWikiRepositoriesCall", "Indexing Pipeline", (double) stopwatch.ElapsedMilliseconds);
      }
    }

    public virtual IEnumerable<WikiV2> GetAllWikisAsync(string projectId = null)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      try
      {
        return this.m_expRetryInvoker.InvokeWithFaultCheck<IEnumerable<WikiV2>>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<List<WikiV2>>((Func<CancellationTokenSource, Task<List<WikiV2>>>) (tokenSource => this.m_wikiHttpClient.GetAllWikisAsync(projectId, (object) null, tokenSource.Token)), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);
      }
      finally
      {
        stopwatch.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("GetWikisCall", "Indexing Pipeline", (double) stopwatch.ElapsedMilliseconds);
      }
    }
  }
}
