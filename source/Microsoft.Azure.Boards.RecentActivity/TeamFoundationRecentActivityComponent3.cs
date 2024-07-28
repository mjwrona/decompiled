// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.RecentActivity.TeamFoundationRecentActivityComponent3
// Assembly: Microsoft.Azure.Boards.RecentActivity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 684DCFA4-4764-4794-94A6-960AF811434C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.RecentActivity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.Azure.Boards.RecentActivity
{
  internal class TeamFoundationRecentActivityComponent3 : TeamFoundationRecentActivityComponent2
  {
    public override void UpdateRecentActivities(
      IVssRequestContext requestContext,
      IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity> userActivities,
      IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity> projectActivities,
      RecentActivityRetentionPolicy retentionPolicy)
    {
      try
      {
        this.TraceEnter(15162011, nameof (UpdateRecentActivities));
        IEnumerable<KeyValuePair<string, string>> rows = userActivities.Select<Microsoft.Azure.Boards.RecentActivity.RecentActivity, KeyValuePair<string, string>>((System.Func<Microsoft.Azure.Boards.RecentActivity.RecentActivity, KeyValuePair<string, string>>) (activity => new KeyValuePair<string, string>(activity.ArtifactId, activity.ActivityDetails)));
        Microsoft.Azure.Boards.RecentActivity.RecentActivity recentActivity = userActivities.First<Microsoft.Azure.Boards.RecentActivity.RecentActivity>();
        this.DataspaceRlsEnabled = false;
        this.PrepareStoredProcedure("prc_UpdateRecentActivities");
        this.BindDateTime("@activityDate", recentActivity.ActivityDate);
        this.BindGuid("@identityId", recentActivity.IdentityId);
        this.BindGuid("@artifactKind", recentActivity.ArtifactKind);
        this.BindKeyValuePairStringTable("@userActivityDetails", rows);
        this.BindTable("@projectActivityDetails", "typ_RecentActivitiesTable", this.GetProjectActivityDetails(projectActivities));
        this.BindInt("@maxActivitiesPerUser", retentionPolicy.RetentionCountPerUser);
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
      this.BindInt("@limit", limit);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Microsoft.Azure.Boards.RecentActivity.RecentActivity>((ObjectBinder<Microsoft.Azure.Boards.RecentActivity.RecentActivity>) new TeamFoundationRecentActivityComponent.TeamFoundationRecentActivityRowBinder());
      return (IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity>) resultCollection.GetCurrent<Microsoft.Azure.Boards.RecentActivity.RecentActivity>().Items;
    }

    public override int CleanupProjectActivities(
      IVssRequestContext requestContext,
      Guid artifactKind,
      int retentionCountPerProject)
    {
      this.PrepareStoredProcedure("prc_CleanupProjectActivities");
      this.BindGuid("@artifactKind", artifactKind);
      this.BindInt("@maxActivitiesPerProject", retentionCountPerProject);
      return (int) this.ExecuteScalar();
    }
  }
}
