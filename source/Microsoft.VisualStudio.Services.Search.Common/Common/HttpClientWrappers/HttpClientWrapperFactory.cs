// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.HttpClientWrapperFactory
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;

namespace Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers
{
  public class HttpClientWrapperFactory : IHttpClientWrapperFactory
  {
    [StaticSafe]
    private static readonly HttpClientWrapperFactory s_instance = new HttpClientWrapperFactory();

    private HttpClientWrapperFactory()
    {
    }

    public static IHttpClientWrapperFactory GetInstance() => (IHttpClientWrapperFactory) HttpClientWrapperFactory.s_instance;

    public virtual ProjectHttpClientWrapper GetProjectHttpClient(ExecutionContext executionContext) => new ProjectHttpClientWrapper(executionContext, new TraceMetaData(1080102, "Indexing Pipeline", "Crawl"));

    public virtual GitHttpClientWrapper GetGitHttpClient(ExecutionContext executionContext) => new GitHttpClientWrapper(executionContext, Guid.Empty, Guid.Empty, new TraceMetaData(1080102, "Indexing Pipeline", "Crawl"));

    public virtual TfvcHttpClientWrapper GetTfvcHttpClient(ExecutionContext executionContext) => new TfvcHttpClientWrapper(executionContext, new TraceMetaData(1080102, "Indexing Pipeline", "Crawl"));

    public virtual WorkItemHttpClientWrapper GetWorkItemHttpClient(ExecutionContext executionContext)
    {
      if (executionContext == null)
        throw new ArgumentNullException(nameof (executionContext));
      return executionContext.RequestContext.IsFeatureEnabled("Search.Server.WorkItem.UseDiscussionEditDeleteEndpoint") ? (WorkItemHttpClientWrapper) new WorkItemHttpClientWrapper2(executionContext, new TraceMetaData(1080102, "Indexing Pipeline", "Crawl")) : new WorkItemHttpClientWrapper(executionContext, new TraceMetaData(1080102, "Indexing Pipeline", "Crawl"));
    }

    public virtual FeedHttpClientWrapper GetFeedHttpClient(ExecutionContext executionContext) => new FeedHttpClientWrapper(executionContext, new TraceMetaData(1080102, "Indexing Pipeline", "Crawl"));

    public virtual BoardHttpClientWrapper GetBoardHttpClient(ExecutionContext executionContext) => new BoardHttpClientWrapper(executionContext, new TraceMetaData(1080102, "Indexing Pipeline", "Crawl"));

    public virtual TfsContributionsHttpClientWrapper GetTfsContributionsHttpClient(
      ExecutionContext executionContext)
    {
      return new TfsContributionsHttpClientWrapper(executionContext, new TraceMetaData(1080102, "Indexing Pipeline", "Crawl"));
    }

    public virtual ProjectAnalysisHttpClientWrapper GetProjectAnalysisHttpClient(
      ExecutionContext executionContext)
    {
      return new ProjectAnalysisHttpClientWrapper(executionContext, new TraceMetaData(1080102, "Indexing Pipeline", "Crawl"));
    }

    public virtual SocialHttpClientWrapper GetSocialHttpClient(ExecutionContext executionContext) => new SocialHttpClientWrapper(executionContext, new TraceMetaData(1080102, "Indexing Pipeline", "Crawl"));
  }
}
