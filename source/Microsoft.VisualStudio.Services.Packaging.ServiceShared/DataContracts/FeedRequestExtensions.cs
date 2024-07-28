// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts.FeedRequestExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts
{
  public static class FeedRequestExtensions
  {
    public static IFeedRequest WithProtocol(
      this IProtocolAgnosticFeedRequest protocolAgnosticFeedRequest,
      IProtocol protocol)
    {
      return (IFeedRequest) new FeedRequest(protocolAgnosticFeedRequest, protocol);
    }

    public static string GetFeedNameOrIdForUri(
      this IProtocolAgnosticFeedRequest feedRequest,
      PackagingUriNamePreference namePreference)
    {
      if (namePreference == PackagingUriNamePreference.PreferCanonicalId)
        return feedRequest.Feed.FullyQualifiedId;
      if (namePreference != PackagingUriNamePreference.PreferUserSuppliedNameOrId)
        throw new ArgumentOutOfRangeException(nameof (namePreference), (object) namePreference, (string) null);
      return !string.IsNullOrWhiteSpace(feedRequest.UserSuppliedFeedNameOrId) ? feedRequest.UserSuppliedFeedNameOrId : feedRequest.Feed.FullyQualifiedId;
    }

    public static string GetProjectNameOrIdForUri(
      this IProtocolAgnosticFeedRequest feedRequest,
      PackagingUriNamePreference namePreference)
    {
      if (namePreference != PackagingUriNamePreference.PreferCanonicalId)
      {
        if (namePreference != PackagingUriNamePreference.PreferUserSuppliedNameOrId)
          throw new ArgumentOutOfRangeException(nameof (namePreference), (object) namePreference, (string) null);
        if (!string.IsNullOrWhiteSpace(feedRequest.UserSuppliedProjectNameOrId))
          return feedRequest.UserSuppliedProjectNameOrId;
        return feedRequest.Feed.Project?.Id.ToString();
      }
      return feedRequest.Feed.Project?.Id.ToString();
    }
  }
}
