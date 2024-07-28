// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts.FeedRequest
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts
{
  public class FeedRequest : ProtocolAgnosticFeedRequest, IFeedRequest, IProtocolAgnosticFeedRequest
  {
    public FeedRequest(FeedCore feed, IProtocol protocol)
      : this((string) null, (string) null, feed, protocol)
    {
    }

    public FeedRequest(
      string userSuppliedProjectNameOrId,
      string userSuppliedFeedNameOrId,
      FeedCore feed,
      IProtocol protocol)
      : base(userSuppliedProjectNameOrId, userSuppliedFeedNameOrId, feed)
    {
      this.Protocol = protocol;
    }

    public FeedRequest(IFeedRequest feedRequest)
      : this(feedRequest.UserSuppliedProjectNameOrId, feedRequest.UserSuppliedFeedNameOrId, feedRequest.Feed, feedRequest.Protocol)
    {
    }

    public FeedRequest(
      IProtocolAgnosticFeedRequest protocolAgnosticRequest,
      IProtocol protocol)
      : this(protocolAgnosticRequest.UserSuppliedProjectNameOrId, protocolAgnosticRequest.UserSuppliedFeedNameOrId, protocolAgnosticRequest.Feed, protocol)
    {
    }

    public IProtocol Protocol { get; }
  }
}
