// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.RecentActivity.TeamFoundationRecentActivityComponent
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
  public class TeamFoundationRecentActivityComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[4]
    {
      (IComponentCreator) new ComponentCreator<TeamFoundationRecentActivityComponent>(1, false),
      (IComponentCreator) new ComponentCreator<TeamFoundationRecentActivityComponent2>(2, false),
      (IComponentCreator) new ComponentCreator<TeamFoundationRecentActivityComponent3>(3, false),
      (IComponentCreator) new ComponentCreator<TeamFoundationRecentActivityComponent4>(4, false)
    }, "TeamFoundationRecentActivity");

    public TeamFoundationRecentActivityComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual void UpdateRecentActivities(
      IVssRequestContext requestContext,
      IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity> activities,
      RecentActivityRetentionPolicy retentionPolicy)
    {
      this.PrepareStoredProcedure("prc_UpdateRecentActivities");
      IEnumerable<KeyValuePair<string, string>> rows = activities.Select<Microsoft.Azure.Boards.RecentActivity.RecentActivity, KeyValuePair<string, string>>((System.Func<Microsoft.Azure.Boards.RecentActivity.RecentActivity, KeyValuePair<string, string>>) (activity => new KeyValuePair<string, string>(activity.ArtifactId, activity.ActivityDetails)));
      this.BindDateTime("@activityDate", activities.First<Microsoft.Azure.Boards.RecentActivity.RecentActivity>().ActivityDate);
      this.BindGuid("@identityId", activities.First<Microsoft.Azure.Boards.RecentActivity.RecentActivity>().IdentityId);
      this.BindGuid("@artifactKind", activities.First<Microsoft.Azure.Boards.RecentActivity.RecentActivity>().ArtifactKind);
      this.BindKeyValuePairStringTable("@artifactIdDetails", rows);
      this.BindInt("@maxActivitiesPerUser", retentionPolicy.RetentionCountPerUser);
      this.ExecuteNonQuery();
    }

    public virtual IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity> GetUserActivities(
      IVssRequestContext requestContext,
      Guid identityId,
      Guid artifactKind,
      int maxActivitiesPerUser)
    {
      this.PrepareStoredProcedure("prc_GetRecentActivities");
      this.BindGuid("@identityId", identityId);
      this.BindGuid("@artifactKind", artifactKind);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Microsoft.Azure.Boards.RecentActivity.RecentActivity>(this.CreateBinder());
      return (IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity>) resultCollection.GetCurrent<Microsoft.Azure.Boards.RecentActivity.RecentActivity>().Items;
    }

    public virtual void UpdateRecentActivities(
      IVssRequestContext requestContext,
      IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity> userActivities,
      IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity> projectActivities,
      RecentActivityRetentionPolicy retentionPolicy)
    {
      this.UpdateRecentActivities(requestContext, userActivities, retentionPolicy);
    }

    public virtual IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity> GetProjectActivities(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      int limit)
    {
      return (IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity>) new List<Microsoft.Azure.Boards.RecentActivity.RecentActivity>();
    }

    public virtual int CleanupProjectActivities(
      IVssRequestContext requestContext,
      Guid artifactKind,
      int retentionCountPerProject)
    {
      return 0;
    }

    public virtual long CleanupRecentUserActivity(
      IVssRequestContext requestContext,
      Guid artifactKind,
      int maxActivitiesPerUser,
      long lastprocessedTime)
    {
      return 0;
    }

    protected ObjectBinder<Microsoft.Azure.Boards.RecentActivity.RecentActivity> CreateBinder() => (ObjectBinder<Microsoft.Azure.Boards.RecentActivity.RecentActivity>) new TeamFoundationRecentActivityComponent.TeamFoundationRecentActivityRowBinder();

    protected class TeamFoundationRecentActivityRowBinder : ObjectBinder<Microsoft.Azure.Boards.RecentActivity.RecentActivity>
    {
      protected SqlColumnBinder m_ArtifactKindCol = new SqlColumnBinder("ArtifactKind");
      protected SqlColumnBinder m_ArtifactIdCol = new SqlColumnBinder("ArtifactId");
      protected SqlColumnBinder m_IdentityIdCol = new SqlColumnBinder("IdentityId");
      protected SqlColumnBinder m_ActivityDetailsCol = new SqlColumnBinder("ActivityDetails");
      protected SqlColumnBinder m_ActivityDateCol = new SqlColumnBinder("ActivityDate");

      protected override Microsoft.Azure.Boards.RecentActivity.RecentActivity Bind() => new Microsoft.Azure.Boards.RecentActivity.RecentActivity()
      {
        ArtifactKind = this.m_ArtifactKindCol.GetGuid((IDataReader) this.Reader),
        ArtifactId = this.m_ArtifactIdCol.GetString((IDataReader) this.Reader, true),
        IdentityId = this.m_IdentityIdCol.GetGuid((IDataReader) this.Reader),
        ActivityDetails = this.m_ActivityDetailsCol.GetString((IDataReader) this.Reader, true),
        ActivityDate = this.m_ActivityDateCol.GetDateTime((IDataReader) this.Reader)
      };
    }
  }
}
