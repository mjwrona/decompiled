// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.RecentActivity.TeamFoundationProjectUserRecentActivityComponent
// Assembly: Microsoft.Azure.Boards.RecentActivity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 684DCFA4-4764-4794-94A6-960AF811434C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.RecentActivity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.Azure.Boards.RecentActivity
{
  public class TeamFoundationProjectUserRecentActivityComponent : TeamFoundationSqlResourceComponent
  {
    private const string s_serviceName = "TeamFoundationProjectUserRecentActivity";
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<TeamFoundationProjectUserRecentActivityComponent>(1, false),
      (IComponentCreator) new ComponentCreator<TeamFoundationProjectUserRecentActivityComponent2>(2, false),
      (IComponentCreator) new ComponentCreator<TeamFoundationProjectUserRecentActivityComponent3>(3, false)
    }, "TeamFoundationProjectUserRecentActivity");

    public TeamFoundationProjectUserRecentActivityComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual void UpdateProjectUserRecentActivities(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid identityId,
      DateTime activityDate,
      Guid artifactKind,
      string artifactId,
      IDictionary<string, string> activityDetails)
    {
      this.PrepareStoredProcedure("prc_UpdateProjectUserRecentActivities");
      this.BindDateTime("@activityDate", activityDate);
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindGuid("@identityId", identityId);
      this.BindGuid("@artifactKind", artifactKind);
      this.BindString("@artifactId", artifactId, 256, false, SqlDbType.VarChar);
      this.BindString("@activityDetails", JsonConvert.SerializeObject((object) activityDetails), int.MaxValue, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public virtual IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity> GetProjectUserRecentActivities(
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
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Microsoft.Azure.Boards.RecentActivity.RecentActivity>((ObjectBinder<Microsoft.Azure.Boards.RecentActivity.RecentActivity>) new TeamFoundationProjectUserRecentActivityComponent.TeamFoundationProjectUserRecentActivityRowBinder());
      return (IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity>) resultCollection.GetCurrent<Microsoft.Azure.Boards.RecentActivity.RecentActivity>().Items;
    }

    public virtual void CleanupRecentProjectUserActivity(
      IVssRequestContext requestContext,
      int maxActivitiesPerUser,
      int maxDaysPerActivity)
    {
    }

    protected class TeamFoundationProjectUserRecentActivityRowBinder : ObjectBinder<Microsoft.Azure.Boards.RecentActivity.RecentActivity>
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
