// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.ProjectCollectionHttpClientWrapper
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers
{
  public class ProjectCollectionHttpClientWrapper
  {
    private ExponentialBackoffRetryInvoker m_expRetryInvoker;
    private TraceMetaData m_traceMetaData;
    private IIndexerFaultService m_faultService;
    private int m_waitTimeInMs;
    private int m_retryLimit;

    public ProjectCollectionHttpClientWrapper(
      IVssRequestContext requestContext,
      TraceMetaData traceMetadata)
    {
      this.m_expRetryInvoker = ExponentialBackoffRetryInvoker.Instance;
      this.m_traceMetaData = traceMetadata;
      this.m_faultService = (IIndexerFaultService) new IndexerV1FaultService(requestContext, (IFaultStore) new RegistryServiceFaultStore(requestContext));
      this.m_waitTimeInMs = 60000;
      this.m_retryLimit = 3;
    }

    public virtual TeamProjectCollection GetProjectCollection(
      ProjectCollectionHttpClient projectCollectionHttpClient,
      string collectionId)
    {
      return this.m_expRetryInvoker.InvokeWithFaultCheck<TeamProjectCollection>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<TeamProjectCollection>((Func<CancellationTokenSource, Task<TeamProjectCollection>>) (tokenSource => projectCollectionHttpClient.GetProjectCollection(collectionId)), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);
    }
  }
}
