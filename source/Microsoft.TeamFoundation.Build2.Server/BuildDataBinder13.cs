// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildDataBinder13
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class BuildDataBinder13 : BuildObjectBinder<BuildData>
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

    public BuildDataBinder13(
      IVssRequestContext requestContext,
      BuildSqlComponentBase resourceComponent)
      : base(requestContext, resourceComponent)
    {
    }

    protected override BuildData Bind()
    {
      BuildData buildData1 = new BuildData();
      buildData1.Id = this.m_buildId.GetInt32((IDataReader) this.Reader);
      buildData1.ProjectId = this.ResourceComponent.GetDataspaceIdentifier(this.m_dataspaceId.GetInt32((IDataReader) this.Reader));
      buildData1.BuildNumber = DBHelper.DBPathToServerPath(this.m_buildNumber.GetString((IDataReader) this.Reader, false));
      buildData1.BuildNumberRevision = this.m_buildNumberRevision.GetNullableInt32((IDataReader) this.Reader);
      buildData1.SourceBranch = this.m_sourceBranch.GetString((IDataReader) this.Reader, true);
      buildData1.SourceVersion = this.m_sourceVersion.GetString((IDataReader) this.Reader, true);
      buildData1.Parameters = this.m_parameters.GetString((IDataReader) this.Reader, true);
      buildData1.Status = new BuildStatus?((BuildStatus) this.m_status.GetByte((IDataReader) this.Reader));
      buildData1.Priority = (QueuePriority) this.m_priority.GetByte((IDataReader) this.Reader);
      buildData1.StartTime = this.m_startTime.GetNullableDateTime((IDataReader) this.Reader);
      buildData1.FinishTime = this.m_finishTime.GetNullableDateTime((IDataReader) this.Reader);
      buildData1.QueueTime = this.m_queueTime.GetNullableDateTime((IDataReader) this.Reader);
      buildData1.Reason = (BuildReason) this.m_reason.GetInt32((IDataReader) this.Reader);
      buildData1.RequestedFor = this.m_requestedFor.GetGuid((IDataReader) this.Reader);
      buildData1.RequestedBy = this.m_requestedBy.GetGuid((IDataReader) this.Reader);
      buildData1.LastChangedDate = this.m_changedOn.GetDateTime((IDataReader) this.Reader);
      buildData1.LastChangedBy = this.m_changedBy.GetGuid((IDataReader) this.Reader);
      buildData1.Deleted = this.m_deleted.GetBoolean((IDataReader) this.Reader);
      buildData1.DeletedBy = new Guid?(this.m_deletedBy.GetGuid((IDataReader) this.Reader, true));
      buildData1.DeletedDate = this.m_deletedOn.GetNullableDateTime((IDataReader) this.Reader);
      buildData1.DeletedReason = this.m_deletedReason.GetString((IDataReader) this.Reader, true);
      buildData1.QueueId = this.m_queueId.GetNullableInt32((IDataReader) this.Reader);
      buildData1.TriggerInfoString = this.m_triggerInfo.GetString((IDataReader) this.Reader, true);
      if (!this.m_result.IsNull((IDataReader) this.Reader))
        buildData1.Result = new BuildResult?((BuildResult) this.m_result.GetByte((IDataReader) this.Reader));
      if (!this.m_repositoryType.IsNull((IDataReader) this.Reader) && !this.m_repositoryIdentifier.IsNull((IDataReader) this.Reader))
      {
        BuildData buildData2 = buildData1;
        BuildRepository buildRepository = new BuildRepository();
        buildRepository.Id = this.m_repositoryIdentifier.GetString((IDataReader) this.Reader, true);
        buildRepository.Type = this.m_repositoryType.GetString((IDataReader) this.Reader, true);
        buildData2.Repository = (MinimalBuildRepository) buildRepository;
      }
      buildData1.Definition = new MinimalBuildDefinition()
      {
        ProjectId = buildData1.ProjectId,
        Id = this.m_definitionId.GetInt32((IDataReader) this.Reader),
        Revision = new int?(this.m_definitionVersion.GetInt32((IDataReader) this.Reader)),
        Name = DBHelper.DBPathToServerPath(this.m_definitionName.GetString((IDataReader) this.Reader, false)),
        QueueStatus = this.m_queueStatus.ColumnExists((IDataReader) this.Reader) ? (DefinitionQueueStatus) this.m_queueStatus.GetByte((IDataReader) this.Reader) : DefinitionQueueStatus.Enabled,
        Path = DBHelper.DBPathToServerPath(this.m_path.GetString((IDataReader) this.Reader, false))
      };
      buildData1.Uri = UriHelper.CreateBuildUri(buildData1.Id);
      buildData1.Logs = new LogReference()
      {
        Type = "Container"
      };
      if (!this.m_validationIssues.IsNull((IDataReader) this.Reader))
        buildData1.ValidationResultsString = this.m_validationIssues.GetString((IDataReader) this.Reader, true);
      if (!this.m_queueOptions.IsNull((IDataReader) this.Reader))
        buildData1.QueueOptions = (QueueOptions) this.m_queueOptions.GetByte((IDataReader) this.Reader);
      Guid? deletedBy = buildData1.DeletedBy;
      Guid empty = Guid.Empty;
      if ((deletedBy.HasValue ? (deletedBy.HasValue ? (deletedBy.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) != 0)
        buildData1.DeletedBy = new Guid?();
      buildData1.ChangesCalculated = this.m_changesCalculated.GetBoolean((IDataReader) this.Reader, false);
      return buildData1;
    }
  }
}
