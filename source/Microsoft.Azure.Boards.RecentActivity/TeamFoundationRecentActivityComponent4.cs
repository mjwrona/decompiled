// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.RecentActivity.TeamFoundationRecentActivityComponent4
// Assembly: Microsoft.Azure.Boards.RecentActivity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 684DCFA4-4764-4794-94A6-960AF811434C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.RecentActivity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.Azure.Boards.RecentActivity
{
  internal class TeamFoundationRecentActivityComponent4 : TeamFoundationRecentActivityComponent3
  {
    public override IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity> GetUserActivities(
      IVssRequestContext requestContext,
      Guid identityId,
      Guid artifactKind,
      int maxActivitiesPerUser)
    {
      this.PrepareStoredProcedure("prc_GetRecentActivities");
      this.BindGuid("@identityId", identityId);
      this.BindGuid("@artifactKind", artifactKind);
      this.BindInt("@maxActivitiesPerUser", maxActivitiesPerUser);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Microsoft.Azure.Boards.RecentActivity.RecentActivity>(this.CreateBinder());
      return (IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity>) resultCollection.GetCurrent<Microsoft.Azure.Boards.RecentActivity.RecentActivity>().Items;
    }

    public override long CleanupRecentUserActivity(
      IVssRequestContext requestContext,
      Guid artifactKind,
      int maxActivitiesPerUser,
      long lastprocessedTime)
    {
      this.PrepareStoredProcedure("prc_CleanupUserRecentActivities");
      this.BindGuid("@artifactKind", artifactKind);
      this.BindInt("@maxActivitiesPerUser", maxActivitiesPerUser);
      this.BindLong("@lastProcessedTime", lastprocessedTime);
      object obj = this.ExecuteScalar();
      return !(obj is DBNull) ? (long) obj : 0L;
    }
  }
}
