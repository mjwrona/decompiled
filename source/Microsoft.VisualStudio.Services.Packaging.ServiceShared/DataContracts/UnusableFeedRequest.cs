// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts.UnusableFeedRequest
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts
{
  public class UnusableFeedRequest : IFeedRequest, IProtocolAgnosticFeedRequest
  {
    private UnusableFeedRequest()
    {
    }

    public static IFeedRequest Instance { get; } = (IFeedRequest) new UnusableFeedRequest();

    public FeedCore Feed => throw new InvalidOperationException("This FeedRequest does not represent a particular feed");

    public string UserSuppliedFeedNameOrId => throw new InvalidOperationException("This FeedRequest does not represent a particular feed");

    public string UserSuppliedProjectNameOrId => throw new InvalidOperationException("This FeedRequest does not represent a particular feed");

    public IProtocol Protocol => throw new InvalidOperationException("This FeedRequest does not represent a particular feed");
  }
}
