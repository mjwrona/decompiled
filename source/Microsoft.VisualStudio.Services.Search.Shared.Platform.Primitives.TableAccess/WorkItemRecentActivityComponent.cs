// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.WorkItemRecentActivityComponent
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  public class WorkItemRecentActivityComponent : TeamFoundationSqlResourceComponent
  {
    private const string ServiceName = "Search_RecentActivity";
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<WorkItemRecentActivityComponent>(1),
      (IComponentCreator) new ComponentCreator<WorkItemRecentActivityComponent2>(2)
    }, "Search_RecentActivity");

    public WorkItemRecentActivityComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual void UpdateProjectUserRecentActivities(
      Guid projectId,
      Guid identityId,
      DateTime activityDate,
      int artifactId,
      int areaId)
    {
      this.PrepareStoredProcedure(this.SchemaName + ".prc_UpdateProjectUserRecentActivities");
      this.BindGuid("@projectId", projectId);
      this.BindDateTime("@activityDate", activityDate);
      this.BindGuid("@identityId", identityId);
      this.BindInt("@artifactId", artifactId);
      this.BindInt("@areaId", areaId);
      this.ExecuteNonQuery();
    }

    public virtual IReadOnlyCollection<RecentActivity> GetProjectUserRecentActivities(
      Guid projectId,
      Guid identityId,
      int maxActivitiesPerUser)
    {
      this.PrepareStoredProcedure(this.SchemaName + ".prc_GetProjectUserRecentActivities");
      this.BindGuid("@identityId", identityId);
      this.BindGuid("@projectId", projectId);
      this.BindInt("@maxActivitiesPerUser", maxActivitiesPerUser);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<RecentActivity>((ObjectBinder<RecentActivity>) new WorkItemRecentActivityComponent.WorkItemRecentActivityRowBinder());
      return (IReadOnlyCollection<RecentActivity>) resultCollection.GetCurrent<RecentActivity>().Items;
    }

    public virtual void CleanupRecentProjectUserActivity(int maxActivitiesPerUser)
    {
      this.PrepareStoredProcedure(this.SchemaName + ".prc_CleanupProjectUserRecentActivities");
      this.BindInt("@maxActivitiesPerUser", maxActivitiesPerUser);
      this.ExecuteNonQuery();
    }

    protected virtual string SchemaName => "dbo";

    protected class WorkItemRecentActivityRowBinder : ObjectBinder<RecentActivity>
    {
      protected SqlColumnBinder m_ArtifactIdCol = new SqlColumnBinder("ArtifactId");
      protected SqlColumnBinder m_IdentityIdCol = new SqlColumnBinder("IdentityId");
      protected SqlColumnBinder m_ProjectId = new SqlColumnBinder("ProjectId");
      protected SqlColumnBinder m_ActivityDateCol = new SqlColumnBinder("ActivityDate");
      protected SqlColumnBinder m_AreaIdCol = new SqlColumnBinder("AreaId");

      protected override RecentActivity Bind() => new RecentActivity()
      {
        ArtifactId = this.m_ArtifactIdCol.GetString((IDataReader) this.Reader, false),
        AreaId = this.m_AreaIdCol.GetInt32((IDataReader) this.Reader),
        ProjectId = this.m_ProjectId.GetGuid((IDataReader) this.Reader),
        IdentityId = this.m_IdentityIdCol.GetGuid((IDataReader) this.Reader),
        ActivityDate = this.m_ActivityDateCol.GetDateTime((IDataReader) this.Reader)
      };
    }
  }
}
