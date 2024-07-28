// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildDataBinder15
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class BuildDataBinder15 : BuildObjectBinder<BuildData>
  {
    private SqlColumnBinder m_buildId = new SqlColumnBinder("BuildId");
    private SqlColumnBinder m_definitionId = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder m_definitionVersion = new SqlColumnBinder("DefinitionVersion");
    private SqlColumnBinder m_definitionName = new SqlColumnBinder("DefinitionName");
    private SqlColumnBinder m_queueStatus = new SqlColumnBinder("QueueStatus");
    private SqlColumnBinder m_queueId = new SqlColumnBinder("QueueId");
    private SqlColumnBinder m_buildNumber = new SqlColumnBinder("BuildNumber");
    private SqlColumnBinder m_buildNumberRevision = new SqlColumnBinder("BuildNumberRevision");
    private SqlColumnBinder m_dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder m_repositoryType = new SqlColumnBinder("RepositoryType");
    private SqlColumnBinder m_repositoryIdentifier = new SqlColumnBinder("RepositoryIdentifier");
    private SqlColumnBinder m_sourceBranch = new SqlColumnBinder("SourceBranch");
    private SqlColumnBinder m_sourceVersion = new SqlColumnBinder("SourceVersion");
    private SqlColumnBinder m_parameters = new SqlColumnBinder("Parameters");
    private SqlColumnBinder m_status = new SqlColumnBinder("Status");
    private SqlColumnBinder m_queueTime = new SqlColumnBinder("QueueTime");
    private SqlColumnBinder m_priority = new SqlColumnBinder("Priority");
    private SqlColumnBinder m_startTime = new SqlColumnBinder("StartTime");
    private SqlColumnBinder m_finishTime = new SqlColumnBinder("FinishTime");
    private SqlColumnBinder m_reason = new SqlColumnBinder("Reason");
    private SqlColumnBinder m_result = new SqlColumnBinder("Result");
    private SqlColumnBinder m_requestedFor = new SqlColumnBinder("RequestedFor");
    private SqlColumnBinder m_requestedBy = new SqlColumnBinder("RequestedBy");
    private SqlColumnBinder m_changedOn = new SqlColumnBinder("ChangedOn");
    private SqlColumnBinder m_changedBy = new SqlColumnBinder("ChangedBy");
    private SqlColumnBinder m_deleted = new SqlColumnBinder("Deleted");
    private SqlColumnBinder m_deletedOn = new SqlColumnBinder("DeletedOn");
    private SqlColumnBinder m_deletedBy = new SqlColumnBinder("DeletedBy");
    private SqlColumnBinder m_deletedReason = new SqlColumnBinder("DeletedReason");
    private SqlColumnBinder m_validationIssues = new SqlColumnBinder("ValidationIssues");
    private SqlColumnBinder m_queueOptions = new SqlColumnBinder("QueueOptions");
    private SqlColumnBinder m_keepForever = new SqlColumnBinder("KeepForever");
    private SqlColumnBinder m_changesCalculated = new SqlColumnBinder("ChangesCalculated");
    private SqlColumnBinder m_path = new SqlColumnBinder("Path");
    private SqlColumnBinder m_retainedByRelease = new SqlColumnBinder("RetainedByRelease");
    private SqlColumnBinder m_triggerInfo = new SqlColumnBinder("TriggerInfo");
    private SqlColumnBinder m_triggeredByDataspaceId = new SqlColumnBinder("TriggeredByDataspaceId");
    private SqlColumnBinder m_triggeredByDefinitionId = new SqlColumnBinder("TriggeredByDefinitionId");
    private SqlColumnBinder m_triggeredByDefVersion = new SqlColumnBinder("TriggeredByDefVersion");
    private SqlColumnBinder m_triggeredByBuildId = new SqlColumnBinder("TriggeredByBuildId");
    private SqlColumnBinder m_sourceVersionInfo = new SqlColumnBinder("SourceVersionInfo");

    public BuildDataBinder15(
      IVssRequestContext requestContext,
      BuildSqlComponentBase resourceComponent)
      : base(requestContext, resourceComponent)
    {
    }

    protected override BuildData Bind()
    {
      BuildData buildData = new BuildData();
      buildData.Id = this.m_buildId.GetInt32((IDataReader) this.Reader);
      buildData.ProjectId = this.ResourceComponent.GetDataspaceIdentifier(this.m_dataspaceId.GetInt32((IDataReader) this.Reader));
      buildData.BuildNumber = DBHelper.DBPathToServerPath(this.m_buildNumber.GetString((IDataReader) this.Reader, false));
      buildData.BuildNumberRevision = this.m_buildNumberRevision.GetNullableInt32((IDataReader) this.Reader);
      buildData.SourceBranch = this.m_sourceBranch.GetString((IDataReader) this.Reader, true);
      buildData.SourceVersion = this.m_sourceVersion.GetString((IDataReader) this.Reader, true);
      buildData.Parameters = this.m_parameters.GetString((IDataReader) this.Reader, true);
      buildData.Status = new BuildStatus?((BuildStatus) this.m_status.GetByte((IDataReader) this.Reader));
      buildData.Priority = (QueuePriority) this.m_priority.GetByte((IDataReader) this.Reader);
      buildData.StartTime = this.m_startTime.GetNullableDateTime((IDataReader) this.Reader);
      buildData.FinishTime = this.m_finishTime.GetNullableDateTime((IDataReader) this.Reader);
      buildData.QueueTime = this.m_queueTime.GetNullableDateTime((IDataReader) this.Reader);
      buildData.Reason = (BuildReason) this.m_reason.GetInt32((IDataReader) this.Reader);
      buildData.RequestedFor = this.m_requestedFor.GetGuid((IDataReader) this.Reader);
      buildData.RequestedBy = this.m_requestedBy.GetGuid((IDataReader) this.Reader);
      buildData.LastChangedDate = this.m_changedOn.GetDateTime((IDataReader) this.Reader);
      buildData.LastChangedBy = this.m_changedBy.GetGuid((IDataReader) this.Reader);
      buildData.Deleted = this.m_deleted.GetBoolean((IDataReader) this.Reader);
      buildData.DeletedBy = new Guid?(this.m_deletedBy.GetGuid((IDataReader) this.Reader, true));
      buildData.DeletedDate = this.m_deletedOn.GetNullableDateTime((IDataReader) this.Reader);
      buildData.DeletedReason = this.m_deletedReason.GetString((IDataReader) this.Reader, true);
      buildData.SourceVersionInfoString = this.m_sourceVersionInfo.GetString((IDataReader) this.Reader, true);
      buildData.TriggerInfoString = this.m_triggerInfo.GetString((IDataReader) this.Reader, true);
      buildData.QueueId = this.m_queueId.GetNullableInt32((IDataReader) this.Reader);
      if (!this.m_result.IsNull((IDataReader) this.Reader))
        buildData.Result = new BuildResult?((BuildResult) this.m_result.GetByte((IDataReader) this.Reader));
      if (!this.m_repositoryType.IsNull((IDataReader) this.Reader) && !this.m_repositoryIdentifier.IsNull((IDataReader) this.Reader))
        buildData.Repository = new MinimalBuildRepository()
        {
          Id = this.m_repositoryIdentifier.GetString((IDataReader) this.Reader, true),
          Type = this.m_repositoryType.GetString((IDataReader) this.Reader, true)
        };
      Uri buildUri = UriHelper.CreateBuildUri(buildData.Id);
      buildData.Definition = new MinimalBuildDefinition()
      {
        ProjectId = buildData.ProjectId,
        Id = this.m_definitionId.GetInt32((IDataReader) this.Reader),
        Revision = new int?(this.m_definitionVersion.GetInt32((IDataReader) this.Reader)),
        Name = DBHelper.DBPathToServerPath(this.m_definitionName.GetString((IDataReader) this.Reader, false)),
        QueueStatus = this.m_queueStatus.ColumnExists((IDataReader) this.Reader) ? (DefinitionQueueStatus) this.m_queueStatus.GetByte((IDataReader) this.Reader) : DefinitionQueueStatus.Enabled,
        Path = DBHelper.DBPathToServerPath(this.m_path.GetString((IDataReader) this.Reader, false))
      };
      buildData.Uri = buildUri;
      buildData.Logs = new LogReference()
      {
        Type = "Container"
      };
      if (!this.m_validationIssues.IsNull((IDataReader) this.Reader))
        buildData.ValidationResultsString = this.m_validationIssues.GetString((IDataReader) this.Reader, true);
      if (!this.m_queueOptions.IsNull((IDataReader) this.Reader))
        buildData.QueueOptions = (QueueOptions) this.m_queueOptions.GetByte((IDataReader) this.Reader);
      Guid? deletedBy = buildData.DeletedBy;
      Guid empty = Guid.Empty;
      if ((deletedBy.HasValue ? (deletedBy.HasValue ? (deletedBy.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) != 0)
        buildData.DeletedBy = new Guid?();
      buildData.ChangesCalculated = this.m_changesCalculated.GetBoolean((IDataReader) this.Reader, false);
      if (!this.m_triggeredByDefinitionId.IsNull((IDataReader) this.Reader))
        buildData.TriggeredByBuild = new TriggeredByBuild()
        {
          ProjectId = this.ResourceComponent.GetDataspaceIdentifier(this.m_triggeredByDataspaceId.GetInt32((IDataReader) this.Reader)),
          DefinitionId = this.m_triggeredByDefinitionId.GetInt32((IDataReader) this.Reader, 0),
          DefinitionVersion = new int?(this.m_triggeredByDefVersion.GetInt32((IDataReader) this.Reader, 0)),
          BuildId = this.m_triggeredByBuildId.GetInt32((IDataReader) this.Reader, 0)
        };
      return buildData;
    }
  }
}
