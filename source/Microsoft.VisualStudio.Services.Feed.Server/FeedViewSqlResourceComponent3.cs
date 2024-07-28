// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedViewSqlResourceComponent3
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedViewSqlResourceComponent3 : FeedViewSqlResourceComponent2
  {
    public override FeedView CreateFeedView(
      FeedIdentity feedId,
      Guid viewId,
      string viewName,
      FeedViewType viewType,
      FeedVisibility visibility)
    {
      this.PrepareStoredProcedure("Feed.prc_CreateFeedView");
      this.BindFeedIdentity(feedId);
      this.BindGuid("@viewId", viewId);
      this.BindString("@viewName", viewName, 64, false, SqlDbType.NVarChar);
      this.BindInt("@viewType", (int) viewType);
      return this.ReadFeedView();
    }
  }
}
