// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedViewCache2Data
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Feed.Server
{
  internal class FeedViewCache2Data
  {
    public FeedViewCache2Data(
      Dictionary<Guid, IReadOnlyList<FeedView>> viewsByFeed)
    {
      this.ViewsByFeed = viewsByFeed;
    }

    public Dictionary<Guid, IReadOnlyList<FeedView>> ViewsByFeed { get; }
  }
}
