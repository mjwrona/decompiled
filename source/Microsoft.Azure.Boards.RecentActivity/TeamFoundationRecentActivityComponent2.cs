// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.RecentActivity.TeamFoundationRecentActivityComponent2
// Assembly: Microsoft.Azure.Boards.RecentActivity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 684DCFA4-4764-4794-94A6-960AF811434C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.RecentActivity.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.Azure.Boards.RecentActivity
{
  internal class TeamFoundationRecentActivityComponent2 : TeamFoundationRecentActivityComponent
  {
    private static SqlMetaData[] typ_RecentActivitiesTable = new SqlMetaData[3]
    {
      new SqlMetaData("ArtifactId", SqlDbType.NVarChar, 200L),
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("ActivityDetails", SqlDbType.NVarChar, -1L)
    };

    public override void UpdateRecentActivities(
      IVssRequestContext requestContext,
      IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity> userActivities,
      IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity> projectActivities,
      RecentActivityRetentionPolicy retentionPolicy)
    {
      try
      {
        this.TraceEnter(15162011, nameof (UpdateRecentActivities));
        this.PrepareStoredProcedure("prc_UpdateRecentActivities");
        IEnumerable<KeyValuePair<string, string>> rows = userActivities.Select<Microsoft.Azure.Boards.RecentActivity.RecentActivity, KeyValuePair<string, string>>((System.Func<Microsoft.Azure.Boards.RecentActivity.RecentActivity, KeyValuePair<string, string>>) (activity => new KeyValuePair<string, string>(activity.ArtifactId, activity.ActivityDetails)));
        Microsoft.Azure.Boards.RecentActivity.RecentActivity recentActivity = userActivities.First<Microsoft.Azure.Boards.RecentActivity.RecentActivity>();
        this.BindDateTime("@activityDate", recentActivity.ActivityDate);
        this.BindGuid("@identityId", recentActivity.IdentityId);
        this.BindGuid("@artifactKind", recentActivity.ArtifactKind);
        this.BindKeyValuePairStringTable("@userActivityDetails", rows);
        this.BindTable("@projectActivityDetails", "typ_RecentActivitiesTable", this.GetProjectActivityDetails(projectActivities));
        this.BindInt("@maxActivitiesPerUser", retentionPolicy.RetentionCountPerUser);
        this.BindInt("@maxActivitiesPerProject", retentionPolicy.RetentionCountPerProject);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.TraceLeave(15162012, nameof (UpdateRecentActivities));
      }
    }

    public override IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity> GetProjectActivities(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      int limit)
    {
      this.PrepareStoredProcedure("prc_GetRecentProjectActivities");
      this.BindDataspace(new Guid?(projectId));
      this.BindGuid("@artifactKind", artifactKind);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Microsoft.Azure.Boards.RecentActivity.RecentActivity>((ObjectBinder<Microsoft.Azure.Boards.RecentActivity.RecentActivity>) new TeamFoundationRecentActivityComponent.TeamFoundationRecentActivityRowBinder());
      return (IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity>) resultCollection.GetCurrent<Microsoft.Azure.Boards.RecentActivity.RecentActivity>().Items;
    }

    protected void BindDataspace(Guid? projectId) => this.BindNullableInt("@dataspaceId", projectId.HasValue ? new int?(this.GetDataspaceId(projectId.Value)) : new int?());

    protected IEnumerable<SqlDataRecord> GetProjectActivityDetails(
      IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity> recentActivities)
    {
      TeamFoundationRecentActivityComponent2 activityComponent2 = this;
      foreach (Microsoft.Azure.Boards.RecentActivity.RecentActivity recentActivity in (IEnumerable<Microsoft.Azure.Boards.RecentActivity.RecentActivity>) recentActivities)
      {
        SqlDataRecord projectActivityDetail = new SqlDataRecord(TeamFoundationRecentActivityComponent2.typ_RecentActivitiesTable);
        int num1 = 0;
        SqlDataRecord sqlDataRecord1 = projectActivityDetail;
        int ordinal1 = num1;
        int num2 = ordinal1 + 1;
        string artifactId = recentActivity.ArtifactId;
        sqlDataRecord1.SetString(ordinal1, artifactId);
        SqlDataRecord sqlDataRecord2 = projectActivityDetail;
        int ordinal2 = num2;
        int num3 = ordinal2 + 1;
        int dataspaceId = activityComponent2.GetDataspaceId(recentActivity.ProjectId);
        sqlDataRecord2.SetInt32(ordinal2, dataspaceId);
        SqlDataRecord sqlDataRecord3 = projectActivityDetail;
        int ordinal3 = num3;
        int num4 = ordinal3 + 1;
        string str = recentActivity.ActivityDetails ?? string.Empty;
        sqlDataRecord3.SetString(ordinal3, str);
        yield return projectActivityDetail;
      }
    }
  }
}
