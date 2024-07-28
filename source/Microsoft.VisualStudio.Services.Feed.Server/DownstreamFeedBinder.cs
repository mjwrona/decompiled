// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.DownstreamFeedBinder
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class DownstreamFeedBinder : ObjectBinder<DownstreamFeed>
  {
    private SqlColumnBinder hostId = new SqlColumnBinder("ServiceHostId");
    private SqlColumnBinder feedId = new SqlColumnBinder("FeedId");

    protected override DownstreamFeed Bind() => new DownstreamFeed()
    {
      HostId = this.hostId.GetGuid((IDataReader) this.Reader, false),
      FeedId = this.feedId.GetGuid((IDataReader) this.Reader, false)
    };
  }
}
