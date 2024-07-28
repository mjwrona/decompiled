// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts.FeedRequest`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts
{
  public class FeedRequest<TData> : 
    FeedRequest,
    IFeedRequest<TData>,
    IFeedRequest,
    IProtocolAgnosticFeedRequest
  {
    public FeedRequest(IFeedRequest feedRequest, TData data)
      : base(feedRequest)
    {
      this.AdditionalData = data;
    }

    public FeedRequest(FeedCore feed, IProtocol protocol, TData data)
      : base(feed, protocol)
    {
      this.AdditionalData = data;
    }

    public TData AdditionalData { get; }
  }
}
