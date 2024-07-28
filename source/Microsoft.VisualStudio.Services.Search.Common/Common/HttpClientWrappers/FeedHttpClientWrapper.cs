// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.FeedHttpClientWrapper
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers
{
  public class FeedHttpClientWrapper
  {
    private readonly FeedHttpClient m_feedHttpClient;
    private readonly ExponentialBackoffRetryInvoker m_expRetryInvoker;
    private readonly TraceMetaData m_traceMetaData;
    private readonly IIndexerFaultService m_faultService;
    private readonly int m_waitTimeInMs;
    private readonly int m_retryLimit;

    protected FeedHttpClientWrapper()
    {
    }

    public FeedHttpClientWrapper(Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext, TraceMetaData traceMetadata)
    {
      this.m_feedHttpClient = executionContext != null ? FeedHttpClientWrapper.GetFeedClient(executionContext) : throw new ArgumentNullException(nameof (executionContext));
      this.m_faultService = executionContext.FaultService;
      this.m_expRetryInvoker = ExponentialBackoffRetryInvoker.Instance;
      this.m_traceMetaData = traceMetadata;
      this.m_waitTimeInMs = executionContext.ServiceSettings.PipelineSettings.CrawlOperationRetryIntervalInSec * 1000;
      this.m_retryLimit = executionContext.ServiceSettings.PipelineSettings.CrawlOperationRetryLimit;
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Target = "vssConnection", Justification = "#1575855")]
    private static FeedHttpClient GetFeedClient(Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext)
    {
      bool flag = false;
      if (executionContext.RequestContext.ExecutionEnvironment.IsDevFabricDeployment)
        flag = string.Equals(executionContext.RequestContext.GetConfigValue<string>("/Service/ALMSearch/Settings/FakeFeedClient", TeamFoundationHostType.ProjectCollection, "0"), "1");
      if (flag)
      {
        string configValue1 = executionContext.RequestContext.GetConfigValue<string>("/Service/ALMSearch/Settings/FakeFeedAccount", TeamFoundationHostType.ProjectCollection, "https://mseng.visualstudio.com");
        string configValue2 = executionContext.RequestContext.GetConfigValue<string>("/Service/ALMSearch/Settings/FakeFeedAccountAccessToken", TeamFoundationHostType.ProjectCollection, "");
        return !string.IsNullOrWhiteSpace(configValue2) ? new VssConnection(new Uri(configValue1), (VssCredentials) (FederatedCredential) new VssBasicCredential("AuthToken", configValue2)).GetClient<FeedHttpClient>() : throw new SearchServiceException("Registry key /Service/ALMSearch/Settings/FakeFeedAccountAccessToken not set");
      }
      IVssRequestContext requestContext = executionContext.RequestContext.Elevate();
      return executionContext.RequestContext.GetService<ICollectionRedirectionService>().GetFeedClient<FeedHttpClient>(requestContext);
    }

    public virtual IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> GetFeeds() => this.GetFeedsInternal();

    private IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> GetFeedsInternal() => this.m_expRetryInvoker.InvokeWithFaultCheck<IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>>((Func<CancellationTokenSource, Task<List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>>>) (tokenSource => this.m_feedHttpClient.GetFeedsAsync(new FeedRole?(), (object) null, new CancellationToken())), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);

    public virtual Microsoft.VisualStudio.Services.Feed.WebApi.Feed GetFeed(Guid feedId) => this.m_expRetryInvoker.InvokeWithFaultCheck<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((Func<CancellationTokenSource, Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>>) (tokenSource => this.m_feedHttpClient.GetFeedAsync(feedId.ToString(), (object) null, new CancellationToken())), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);

    public virtual Microsoft.VisualStudio.Services.Feed.WebApi.Feed GetFeed(
      Guid feedId,
      Guid projectId)
    {
      return this.m_expRetryInvoker.InvokeWithFaultCheck<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((Func<CancellationTokenSource, Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>>) (tokenSource => this.m_feedHttpClient.GetFeedAsync(projectId, feedId.ToString())), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);
    }

    public virtual List<FeedView> GetFeedViews(Guid feedId) => this.m_expRetryInvoker.InvokeWithFaultCheck<List<FeedView>>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<List<FeedView>>((Func<CancellationTokenSource, Task<List<FeedView>>>) (tokenSource => this.m_feedHttpClient.GetFeedViewsAsync(feedId.ToString())), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);

    public virtual List<FeedView> GetFeedViews(Guid projectId, Guid feedId) => this.m_expRetryInvoker.InvokeWithFaultCheck<List<FeedView>>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<List<FeedView>>((Func<CancellationTokenSource, Task<List<FeedView>>>) (tokenSource => this.m_feedHttpClient.GetFeedViewsAsync(projectId, feedId.ToString())), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);

    public virtual PackageChangesResponse GetPackageChanges(
      string feedId,
      long continuationToken,
      out bool isLastBatch)
    {
      PackageChangesResponse packageChanges = this.m_expRetryInvoker.InvokeWithFaultCheck<PackageChangesResponse>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<PackageChangesResponse>((Func<CancellationTokenSource, Task<PackageChangesResponse>>) (tokenSource =>
      {
        FeedHttpClient feedHttpClient = this.m_feedHttpClient;
        string feedId1 = feedId;
        long? continuationToken1 = new long?(continuationToken);
        CancellationToken token = tokenSource.Token;
        int? batchSize = new int?();
        CancellationToken cancellationToken = token;
        return feedHttpClient.GetPackageChangesAsync(feedId1, continuationToken1, batchSize, cancellationToken: cancellationToken);
      }), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);
      isLastBatch = this.m_feedHttpClient.LastResponseContext.HttpStatusCode == HttpStatusCode.OK && packageChanges.Count == 0;
      return packageChanges;
    }

    public virtual PackageChangesResponse GetPackageChanges(
      string feedId,
      Guid projectId,
      long continuationToken,
      out bool isLastBatch)
    {
      PackageChangesResponse packageChanges = this.m_expRetryInvoker.InvokeWithFaultCheck<PackageChangesResponse>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<PackageChangesResponse>((Func<CancellationTokenSource, Task<PackageChangesResponse>>) (tokenSource =>
      {
        FeedHttpClient feedHttpClient = this.m_feedHttpClient;
        string str = feedId;
        Guid project = projectId;
        string feedId1 = str;
        long? continuationToken1 = new long?(continuationToken);
        CancellationToken token = tokenSource.Token;
        int? batchSize = new int?();
        CancellationToken cancellationToken = token;
        return feedHttpClient.GetPackageChangesAsync(project, feedId1, continuationToken1, batchSize, cancellationToken: cancellationToken);
      }), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);
      isLastBatch = this.m_feedHttpClient.LastResponseContext.HttpStatusCode == HttpStatusCode.OK && packageChanges.Count == 0;
      return packageChanges;
    }

    public virtual FeedChangesResponse GetFeedChanges(
      long continuationToken,
      out bool isLastBatch,
      bool includeDeleted = true)
    {
      FeedChangesResponse feedChanges = this.m_expRetryInvoker.InvokeWithFaultCheck<FeedChangesResponse>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<FeedChangesResponse>((Func<CancellationTokenSource, Task<FeedChangesResponse>>) (tokenSource =>
      {
        FeedHttpClient feedHttpClient = this.m_feedHttpClient;
        bool? includeDeleted1 = new bool?(includeDeleted);
        long? continuationToken1 = new long?(continuationToken);
        CancellationToken token = tokenSource.Token;
        int? batchSize = new int?();
        CancellationToken cancellationToken = token;
        return feedHttpClient.GetFeedChangesAsync(includeDeleted1, continuationToken1, batchSize, cancellationToken: cancellationToken);
      }), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);
      isLastBatch = this.m_feedHttpClient.LastResponseContext.HttpStatusCode == HttpStatusCode.OK && feedChanges.Count == 0;
      return feedChanges;
    }

    public virtual FeedChange GetFeedChange(string feedId) => this.m_expRetryInvoker.InvokeWithFaultCheck<FeedChange>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<FeedChange>((Func<CancellationTokenSource, Task<FeedChange>>) (tokenSource => this.m_feedHttpClient.GetFeedChangeAsync(feedId, cancellationToken: tokenSource.Token)), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);

    public virtual FeedChange GetFeedChange(string feedId, Guid projectId) => this.m_expRetryInvoker.InvokeWithFaultCheck<FeedChange>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<FeedChange>((Func<CancellationTokenSource, Task<FeedChange>>) (tokenSource => this.m_feedHttpClient.GetFeedChangeAsync(projectId, feedId, cancellationToken: tokenSource.Token)), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);

    public virtual List<FeedPermission> GetFeedPermissions(string feedId)
    {
      try
      {
        return this.m_expRetryInvoker.InvokeWithFaultCheck<List<FeedPermission>>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<List<FeedPermission>>((Func<CancellationTokenSource, Task<List<FeedPermission>>>) (tokenSource => this.m_feedHttpClient.GetFeedPermissionsAsync(feedId, new bool?(true), (object) null, tokenSource.Token)), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);
      }
      catch (Exception ex)
      {
        return new List<FeedPermission>();
      }
    }
  }
}
