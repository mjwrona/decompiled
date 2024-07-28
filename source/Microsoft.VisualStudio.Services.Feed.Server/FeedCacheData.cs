// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedCacheData
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  internal class FeedCacheData : Dictionary<Guid, Microsoft.VisualStudio.Services.Feed.WebApi.Feed>
  {
    public FeedCacheData(IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> feeds)
      : base((IDictionary<Guid, Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) feeds.ToDictionary<Microsoft.VisualStudio.Services.Feed.WebApi.Feed, Guid>((Func<Microsoft.VisualStudio.Services.Feed.WebApi.Feed, Guid>) (feed => feed.Id)))
    {
    }
  }
}
