// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedSqlResourceConponent13
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Server.Types;
using System;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedSqlResourceConponent13 : FeedSqlResourceComponent12
  {
    public override FeedInternalState GetInternalState(Guid feedId, Guid? projectId)
    {
      this.PrepareStoredProcedure("Feed.prc_GetFeedInternalState");
      this.BindInt("@dataspaceId", this.GetFeedDataspaceId(projectId));
      this.BindGuid("@feedId", feedId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<FeedInternalState>((ObjectBinder<FeedInternalState>) new FeedInternalStateBinder());
        return resultCollection.GetCurrent<FeedInternalState>().Items.FirstOrDefault<FeedInternalState>();
      }
    }

    public override void ResetFeedUpgrade(Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      this.PrepareStoredProcedure("Feed.prc_ResetUpgradeFeedV2");
      this.BindInt("@dataspaceId", this.GetFeedDataspaceId(feed.Project?.Id));
      this.BindGuid("@feedId", feed.Id);
      this.ExecuteNonQuery();
    }

    public override void BeginFeedUpgrade(Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      this.PrepareStoredProcedure("Feed.prc_BeginUpgradeFeedV2");
      this.BindInt("@dataspaceId", this.GetFeedDataspaceId(feed.Project?.Id));
      this.BindGuid("@feedId", feed.Id);
      this.ExecuteNonQuery();
    }

    public override void EndFeedUpgrade(Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      this.PrepareStoredProcedure("Feed.prc_EndUpgradeFeedV2");
      this.BindInt("@dataspaceId", this.GetFeedDataspaceId(feed.Project?.Id));
      this.BindGuid("@feedId", feed.Id);
      this.ExecuteNonQuery();
    }
  }
}
