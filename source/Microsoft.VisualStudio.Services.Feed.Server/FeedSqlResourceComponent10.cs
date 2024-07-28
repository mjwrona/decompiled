// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedSqlResourceComponent10
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
  public class FeedSqlResourceComponent10 : FeedSqlResourceComponent9
  {
    public override FeedInternalState GetInternalState(Guid feedId, Guid? projectId)
    {
      this.PrepareStoredProcedure("Feed.prc_GetFeedInternalState");
      this.BindInt("@dataspaceId", this.GetDataspaceId(Guid.Empty));
      this.BindGuid("@feedId", feedId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<FeedInternalState>((ObjectBinder<FeedInternalState>) new FeedInternalStateBinder());
        return resultCollection.GetCurrent<FeedInternalState>().Items.FirstOrDefault<FeedInternalState>();
      }
    }
  }
}
