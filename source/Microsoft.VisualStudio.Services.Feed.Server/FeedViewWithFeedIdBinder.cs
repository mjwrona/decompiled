// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedViewWithFeedIdBinder
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  internal class FeedViewWithFeedIdBinder : ObjectBinder<(Guid FeedId, FeedView View)>
  {
    private SqlColumnBinder feedId = new SqlColumnBinder("FeedId");
    private SqlColumnBinder viewId = new SqlColumnBinder("ViewId");
    private SqlColumnBinder viewName = new SqlColumnBinder("ViewName");
    private SqlColumnBinder viewType = new SqlColumnBinder("ViewType");
    private SqlColumnBinder visibility = new SqlColumnBinder("Visibility");

    protected override (Guid FeedId, FeedView View) Bind()
    {
      FeedViewType feedViewType = this.viewType.ColumnExists((IDataReader) this.Reader) ? (FeedViewType) this.viewType.GetInt32((IDataReader) this.Reader) : FeedViewType.Release;
      FeedVisibility feedVisibility = (FeedVisibility) this.visibility.GetByte((IDataReader) this.Reader, (byte) 0, (byte) 0);
      return (this.feedId.GetGuid((IDataReader) this.Reader), new FeedView()
      {
        Id = this.viewId.GetGuid((IDataReader) this.Reader),
        Name = this.viewName.GetString((IDataReader) this.Reader, false),
        Type = feedViewType,
        Visibility = new FeedVisibility?(feedVisibility)
      });
    }
  }
}
