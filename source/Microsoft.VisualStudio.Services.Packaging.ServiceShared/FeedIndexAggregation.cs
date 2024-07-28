// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.FeedIndexAggregation
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public abstract class FeedIndexAggregation : 
    IAggregation<FeedIndexAggregation, IFeedIndexAggregationAccessor>,
    IAggregation,
    IAggregationAccessorBootstrapper
  {
    protected readonly IProtocol protocol;

    protected FeedIndexAggregation(
      IProtocol protocol,
      string name,
      AggregationDefinition definition)
    {
      this.protocol = protocol;
      this.VersionName = name;
      this.Definition = definition;
    }

    public AggregationDefinition Definition { get; }

    public string VersionName { get; }

    public abstract IAggregationAccessor Bootstrap(IVssRequestContext requestContext);

    protected IFeedIndexClient GetFeedIndexClient(IVssRequestContext requestContext) => (IFeedIndexClient) new FeedIndexClientFacade(requestContext.Elevate());
  }
}
