// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.RecentActivity.TeamFoundationProjectUserRecentActivityComponent3
// Assembly: Microsoft.Azure.Boards.RecentActivity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 684DCFA4-4764-4794-94A6-960AF811434C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.RecentActivity.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.Azure.Boards.RecentActivity
{
  internal class TeamFoundationProjectUserRecentActivityComponent3 : 
    TeamFoundationProjectUserRecentActivityComponent2
  {
    private static readonly SqlMetaData[] s_guidTable = new SqlMetaData[1]
    {
      new SqlMetaData("Id", SqlDbType.UniqueIdentifier)
    };

    public override IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity> GetProjectUserRecentActivities(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid identityId,
      int maxActivitiesPerUser,
      IEnumerable<Guid> artifactKindIds)
    {
      this.PrepareStoredProcedure("prc_GetProjectUserRecentActivities");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindGuid("@identityId", identityId);
      this.BindInt("@maxActivitiesPerUser", maxActivitiesPerUser);
      this.BindArtifactKindIdTable("@artifactKinds", artifactKindIds);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Microsoft.Azure.Boards.RecentActivity.RecentActivity>((ObjectBinder<Microsoft.Azure.Boards.RecentActivity.RecentActivity>) new TeamFoundationProjectUserRecentActivityComponent.TeamFoundationProjectUserRecentActivityRowBinder());
      return (IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity>) resultCollection.GetCurrent<Microsoft.Azure.Boards.RecentActivity.RecentActivity>().Items;
    }

    protected SqlParameter BindArtifactKindIdTable(
      string parameterName,
      IEnumerable<Guid> artifactKindIds)
    {
      artifactKindIds = artifactKindIds ?? Enumerable.Empty<Guid>();
      System.Func<Guid, SqlDataRecord> selector = (System.Func<Guid, SqlDataRecord>) (containerId =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TeamFoundationProjectUserRecentActivityComponent3.s_guidTable);
        sqlDataRecord.SetGuid(0, containerId);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_GuidTable", artifactKindIds.Select<Guid, SqlDataRecord>(selector));
    }
  }
}
