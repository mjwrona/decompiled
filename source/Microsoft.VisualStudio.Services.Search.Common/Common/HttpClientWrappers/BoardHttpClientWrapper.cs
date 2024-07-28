// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.BoardHttpClientWrapper
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers
{
  public class BoardHttpClientWrapper
  {
    private readonly TeamHttpClient m_boardHttpClient;
    private readonly ExponentialBackoffRetryInvoker m_expRetryInvoker;
    private readonly TraceMetaData m_traceMetaData;
    private readonly IIndexerFaultService m_faultService;
    private readonly int m_waitTimeInMs;
    private readonly int m_retryLimit;

    protected BoardHttpClientWrapper()
    {
    }

    internal BoardHttpClientWrapper(
      Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext,
      TraceMetaData traceMetadata,
      TeamHttpClient teamHttpClient)
      : this(executionContext, traceMetadata)
    {
      this.m_boardHttpClient = teamHttpClient;
    }

    public BoardHttpClientWrapper(Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext, TraceMetaData traceMetadata)
    {
      this.m_boardHttpClient = executionContext != null ? executionContext.RequestContext.GetRedirectedClientIfNeeded<TeamHttpClient>() : throw new ArgumentNullException(nameof (executionContext));
      this.m_faultService = executionContext.FaultService;
      this.m_expRetryInvoker = ExponentialBackoffRetryInvoker.Instance;
      this.m_traceMetaData = traceMetadata;
      this.m_waitTimeInMs = executionContext.ServiceSettings.PipelineSettings.CrawlOperationRetryIntervalInSec * 1000;
      this.m_retryLimit = executionContext.ServiceSettings.PipelineSettings.CrawlOperationRetryLimit;
    }

    public virtual IReadOnlyCollection<WebApiTeam> GetAllTeams()
    {
      HashSet<WebApiTeam> allTeams = new HashSet<WebApiTeam>();
      int skip = 0;
      int num = 0;
      try
      {
        for (; num < 100; ++num)
        {
          List<WebApiTeam> other = this.m_expRetryInvoker.InvokeWithFaultCheck<List<WebApiTeam>>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<List<WebApiTeam>>((Func<CancellationTokenSource, Task<List<WebApiTeam>>>) (tokenSource =>
          {
            TeamHttpClient boardHttpClient = this.m_boardHttpClient;
            int? nullable1 = new int?(1000);
            int? nullable2 = new int?(skip);
            bool? mine = new bool?();
            int? top = nullable1;
            int? skip1 = nullable2;
            bool? expandIdentity = new bool?();
            CancellationToken cancellationToken = new CancellationToken();
            return boardHttpClient.GetAllTeamsAsync(mine, top, skip1, expandIdentity, (object) null, cancellationToken);
          }), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);
          if (other != null)
          {
            if (other.Count > 0)
            {
              allTeams.UnionWith((IEnumerable<WebApiTeam>) other);
              skip += other.Count;
            }
            else
              break;
          }
          else
            break;
        }
      }
      finally
      {
        if (num == 100)
          Tracer.TraceError(1083103, "Indexing Pipeline", "Crawl", "Teams api is called more than the threshold number of times");
      }
      return (IReadOnlyCollection<WebApiTeam>) allTeams;
    }
  }
}
