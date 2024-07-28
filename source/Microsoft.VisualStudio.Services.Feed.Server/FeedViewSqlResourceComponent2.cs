// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedViewSqlResourceComponent2
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedViewSqlResourceComponent2 : FeedViewSqlResourceComponent
  {
    public override IEnumerable<FeedView> GetFeedViews(FeedIdentity feedId, bool? isDeleted = false)
    {
      this.PrepareStoredProcedure("Feed.prc_GetFeedView");
      this.BindFeedIdentity(feedId);
      this.BindNullableBoolean(nameof (isDeleted), isDeleted);
      return this.ReadFeedViews();
    }
  }
}
