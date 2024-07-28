// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedViewBinder
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  internal class FeedViewBinder : ObjectBinder<FeedView>
  {
    private SqlColumnBinder feedId = new SqlColumnBinder("FeedId");
    private SqlColumnBinder viewId = new SqlColumnBinder("ViewId");
    private SqlColumnBinder viewName = new SqlColumnBinder("ViewName");
    private SqlColumnBinder viewType = new SqlColumnBinder("ViewType");
    private SqlColumnBinder visibility = new SqlColumnBinder("Visibility");

    protected override FeedView Bind()
    {
      FeedViewType feedViewType = this.viewType.ColumnExists((IDataReader) this.Reader) ? (FeedViewType) this.viewType.GetInt32((IDataReader) this.Reader) : FeedViewType.Release;
      FeedVisibility feedVisibility = (FeedVisibility) this.visibility.GetByte((IDataReader) this.Reader, (byte) 0, (byte) 0);
      return new FeedView()
      {
        Id = this.viewId.GetGuid((IDataReader) this.Reader),
        Name = this.viewName.GetString((IDataReader) this.Reader, false),
        Type = feedViewType,
        Visibility = new FeedVisibility?(feedVisibility)
      };
    }

    internal static FeedView DeepCloneFeedView(FeedView feedView) => feedView == null ? (FeedView) null : FeedViewBinder.CreateFeedView(feedView.Id, feedView.Name, feedView.Type, feedView.Visibility);

    private static FeedView CreateFeedView(
      Guid id,
      string name,
      FeedViewType type,
      FeedVisibility? visibility)
    {
      return new FeedView()
      {
        Id = id,
        Name = name,
        Type = type,
        Visibility = visibility
      };
    }
  }
}
