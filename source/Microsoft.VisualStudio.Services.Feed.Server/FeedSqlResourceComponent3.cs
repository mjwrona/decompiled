// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedSqlResourceComponent3
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedSqlResourceComponent3 : FeedSqlResourceComponent
  {
    public override Microsoft.VisualStudio.Services.Feed.WebApi.Feed GetFeed(
      string feedName,
      Guid? projectId,
      bool includeDeleted = false,
      bool includeDeletedUpstreams = false)
    {
      this.PrepareStoredProcedure("Feed.prc_GetFeed");
      this.BindInt("@dataspaceId", this.GetDataspaceId(Guid.Empty));
      this.BindString("@feedName", feedName, 64, false, SqlDbType.NVarChar);
      this.BindBoolean("@includeDeleted", includeDeleted);
      return this.ReadFeed();
    }
  }
}
