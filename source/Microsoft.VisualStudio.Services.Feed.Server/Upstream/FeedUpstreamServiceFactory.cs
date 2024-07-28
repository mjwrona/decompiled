// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.Upstream.FeedUpstreamServiceFactory
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Feed.Server.Upstream
{
  public class FeedUpstreamServiceFactory
  {
    private readonly IVssRequestContext requestContext;

    public FeedUpstreamServiceFactory(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IFeedUpstreamService Get() => this.requestContext.IsFeatureEnabled("Packaging.Feed.UseFeedUpstreamServiceFeedCache") ? (IFeedUpstreamService) this.requestContext.GetService<IFeedUpstreamCacheService>() : (IFeedUpstreamService) this.requestContext.GetService<IFeedUpstreamSQLService>();
  }
}
