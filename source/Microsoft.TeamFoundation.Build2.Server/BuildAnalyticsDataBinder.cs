// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildAnalyticsDataBinder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class BuildAnalyticsDataBinder : BuildObjectBinder<BuildAnalyticsData>
  {
    private SqlColumnBinder m_buildId = new SqlColumnBinder("BuildId");
    private SqlColumnBinder m_definitionId = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder m_definitionVersion = new SqlColumnBinder("DefinitionVersion");
    private SqlColumnBinder m_buildNumber = new SqlColumnBinder("BuildNumber");
    private SqlColumnBinder m_buildNumberRevision = new SqlColumnBinder("BuildNumberRevision");
    private SqlColumnBinder m_repositoryType = new SqlColumnBinder("RepositoryType");
    private SqlColumnBinder m_repositoryIdentifier = new SqlColumnBinder("RepositoryIdentifier");
    private SqlColumnBinder m_branchName = new SqlColumnBinder("BranchName");
    private SqlColumnBinder m_status = new SqlColumnBinder("Status");
    private SqlColumnBinder m_queueTime = new SqlColumnBinder("QueueTime");
    private SqlColumnBinder m_startTime = new SqlColumnBinder("StartTime");
    private SqlColumnBinder m_finishTime = new SqlColumnBinder("FinishTime");
    private SqlColumnBinder m_changedOn = new SqlColumnBinder("ChangedOn");
    private SqlColumnBinder m_reason = new SqlColumnBinder("Reason");
    private SqlColumnBinder m_result = new SqlColumnBinder("Result");
    private SqlColumnBinder m_planId = new SqlColumnBinder("PlanId");
    private Guid m_projectId;

    public BuildAnalyticsDataBinder(IVssRequestContext requestContext, Guid projectId)
      : base(requestContext)
    {
      this.m_projectId = projectId;
    }

    protected override BuildAnalyticsData Bind()
    {
      BuildAnalyticsData buildAnalyticsData = new BuildAnalyticsData();
      buildAnalyticsData.ProjectGuid = this.m_projectId;
      buildAnalyticsData.BuildId = this.m_buildId.GetInt32((IDataReader) this.Reader);
      buildAnalyticsData.BuildNumber = DBHelper.DBPathToServerPath(this.m_buildNumber.GetString((IDataReader) this.Reader, false));
      buildAnalyticsData.BuildNumberRevision = this.m_buildNumberRevision.GetNullableInt32((IDataReader) this.Reader);
      buildAnalyticsData.DefinitionId = this.m_definitionId.GetInt32((IDataReader) this.Reader);
      buildAnalyticsData.DefinitionVersion = this.m_definitionVersion.GetInt32((IDataReader) this.Reader);
      buildAnalyticsData.RepositoryId = this.m_repositoryIdentifier.GetString((IDataReader) this.Reader, true);
      buildAnalyticsData.RepositoryType = this.m_repositoryType.GetString((IDataReader) this.Reader, true);
      buildAnalyticsData.BranchName = this.m_branchName.GetString((IDataReader) this.Reader, true);
      buildAnalyticsData.Status = new BuildStatus?((BuildStatus) this.m_status.GetByte((IDataReader) this.Reader));
      buildAnalyticsData.StartTime = this.m_startTime.GetNullableDateTime((IDataReader) this.Reader);
      buildAnalyticsData.FinishTime = this.m_finishTime.GetNullableDateTime((IDataReader) this.Reader);
      buildAnalyticsData.QueueTime = this.m_queueTime.GetNullableDateTime((IDataReader) this.Reader);
      buildAnalyticsData.ChangedOn = this.m_changedOn.GetDateTime((IDataReader) this.Reader);
      buildAnalyticsData.Reason = (BuildReason) this.m_reason.GetInt32((IDataReader) this.Reader);
      buildAnalyticsData.Result = this.m_result.IsNull((IDataReader) this.Reader) ? new BuildResult?() : new BuildResult?((BuildResult) this.m_result.GetByte((IDataReader) this.Reader));
      buildAnalyticsData.PlanId = this.m_planId.GetGuid((IDataReader) this.Reader);
      return buildAnalyticsData;
    }
  }
}
