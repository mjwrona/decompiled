// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedViewSqlResourceComponent6
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedViewSqlResourceComponent6 : FeedViewSqlResourceComponent5
  {
    public override bool CanGetAllNonDeletedFeedViewsForCollection => true;

    public override IEnumerable<(Guid FeedId, FeedView View)> GetAllNonDeletedFeedViewsInCollection()
    {
      this.PrepareStoredProcedure("Feed.prc_GetAllNonDeletedFeedViewsInCollection");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<(Guid, FeedView)>((ObjectBinder<(Guid, FeedView)>) new FeedViewWithFeedIdBinder());
        return (IEnumerable<(Guid, FeedView)>) resultCollection.GetCurrent<(Guid, FeedView)>().Items;
      }
    }
  }
}
