// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.GvfsClientWrapper
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.GvfsHttpClient;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers
{
  public class GvfsClientWrapper : IDisposable
  {
    private bool m_disposedValue;
    private readonly GvfsClient m_gvfsClient;
    private readonly ExponentialBackoffRetryInvoker m_expRetryInvoker;
    private readonly TraceMetaData m_traceMetaData;
    private readonly IIndexerFaultService m_faultService;
    private readonly int m_waitTimeInMs;
    private readonly int m_retryLimit;
    private readonly string m_repoUrl;
    private readonly bool m_isOnPrem;

    internal GvfsClientWrapper()
    {
    }

    internal GvfsClientWrapper(
      Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext,
      string repositoryUrl,
      TraceMetaData traceMetadata)
      : this(executionContext, repositoryUrl, traceMetadata, executionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
    {
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Target = "vssHttpMessageHandler", Justification = "Responsibility of disposing vssHttpMessageHandler has been transferred to m_gvfsClient which is correctly disposed")]
    internal GvfsClientWrapper(
      Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext,
      string repositoryUrl,
      TraceMetaData traceMetadata,
      bool isOnPrem)
    {
      this.m_isOnPrem = isOnPrem;
      if (this.m_isOnPrem)
        return;
      GvfsClientWrapper.ValidateParams(executionContext, repositoryUrl, traceMetadata);
      S2SCommunicator.SetRequestContext(executionContext.RequestContext);
      this.m_gvfsClient = new GvfsClient(new VssHttpMessageHandler(S2SCommunicator.GetS2SCredentials(), new VssHttpRequestSettings()));
      this.m_repoUrl = repositoryUrl;
      this.m_faultService = executionContext.FaultService;
      this.m_expRetryInvoker = ExponentialBackoffRetryInvoker.Instance;
      this.m_traceMetaData = traceMetadata;
      this.m_waitTimeInMs = executionContext.ServiceSettings.PipelineSettings.CrawlOperationRetryIntervalInSec * 1000;
      this.m_retryLimit = executionContext.ServiceSettings.PipelineSettings.CrawlOperationRetryLimit;
    }

    private static void ValidateParams(
      Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext,
      string repositoryUrl,
      TraceMetaData traceMetadata)
    {
      if (executionContext == null)
        throw new ArgumentNullException(nameof (executionContext));
      if (string.IsNullOrWhiteSpace(repositoryUrl))
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("{0} should not be null or only contain whitespaces.", (object) nameof (repositoryUrl))));
      if (traceMetadata == null)
        throw new ArgumentNullException(nameof (traceMetadata));
    }

    internal GitEndPointResponseData GetObjectSizes(IEnumerable<string> objectIds) => this.m_isOnPrem ? (GitEndPointResponseData) null : this.m_expRetryInvoker.InvokeWithFaultCheck<GitEndPointResponseData>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<GitEndPointResponseData>((Func<CancellationTokenSource, Task<GitEndPointResponseData>>) (tokenSource => this.m_gvfsClient.GetObjectSizes(objectIds, this.m_repoUrl)), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);

    [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "Calling Dispose multiple times for Stream object does not break anything")]
    internal virtual Dictionary<string, long> QueryForFileSizes(IEnumerable<string> objectIds)
    {
      GitEndPointResponseData objectSizes = this.GetObjectSizes(objectIds);
      Dictionary<string, long> dictionary = new Dictionary<string, long>();
      if (objectSizes != null)
      {
        using (Stream stream = objectSizes.Stream)
        {
          using (StreamReader streamReader = new StreamReader(stream))
            dictionary = JsonConvert.DeserializeObject<List<GitObjectSize>>(streamReader.ReadToEndAsync().Result).ToDictionary<GitObjectSize, string, long>((Func<GitObjectSize, string>) (x => x.Id), (Func<GitObjectSize, long>) (x => x.Size));
        }
      }
      return dictionary;
    }

    internal GitEndPointResponseData DownloadObjects(IEnumerable<string> objectIds) => this.m_isOnPrem ? (GitEndPointResponseData) null : this.m_expRetryInvoker.InvokeWithFaultCheck<GitEndPointResponseData>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<GitEndPointResponseData>((Func<CancellationTokenSource, Task<GitEndPointResponseData>>) (tokenSource => this.m_gvfsClient.DownloadObjects(objectIds, this.m_repoUrl)), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);

    protected virtual void Dispose(bool disposing)
    {
      if (this.m_disposedValue)
        return;
      if (disposing && this.m_gvfsClient != null)
        this.m_gvfsClient.Dispose();
      this.m_disposedValue = true;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }
  }
}
