// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildDataBinderHelper17
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class BuildDataBinderHelper17
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
    private SqlColumnBinder m_changesCalculated = new SqlColumnBinder("ChangesCalculated");
    private SqlColumnBinder m_path = new SqlColumnBinder("Path");
    private SqlColumnBinder m_triggerInfo = new SqlColumnBinder("TriggerInfo");
    private SqlColumnBinder m_triggeredByDataspaceId = new SqlColumnBinder("TriggeredByDataspaceId");
    private SqlColumnBinder m_triggeredByDefinitionId = new SqlColumnBinder("TriggeredByDefinitionId");
    private SqlColumnBinder m_triggeredByDefVersion = new SqlColumnBinder("TriggeredByDefVersion");
    private SqlColumnBinder m_triggeredByBuildId = new SqlColumnBinder("TriggeredByBuildId");
    private SqlColumnBinder m_sourceVersionInfo = new SqlColumnBinder("SourceVersionInfo");
    private SqlColumnBinder m_templateParameters = new SqlColumnBinder("TemplateParameters");
    private SqlColumnBinder m_appendCommitMessageToRunName = new SqlColumnBinder("AppendCommitMessageToRunName");

    public BuildData Bind(SqlDataReader reader, BuildSqlComponentBase resourceComponent)
    {
      BuildData buildData1 = new BuildData()
      {
        Id = this.m_buildId.GetInt32((IDataReader) reader),
        ProjectId = resourceComponent.GetDataspaceIdentifier(this.m_dataspaceId.GetInt32((IDataReader) reader)),
        BuildNumber = DBHelper.DBPathToServerPath(this.m_buildNumber.GetString((IDataReader) reader, false)),
        BuildNumberRevision = this.m_buildNumberRevision.GetNullableInt32((IDataReader) reader),
        SourceBranch = this.m_sourceBranch.GetString((IDataReader) reader, true),
        SourceVersion = this.m_sourceVersion.GetString((IDataReader) reader, true),
        Parameters = this.m_parameters.GetString((IDataReader) reader, true),
        Status = new BuildStatus?((BuildStatus) this.m_status.GetByte((IDataReader) reader)),
        Priority = (QueuePriority) this.m_priority.GetByte((IDataReader) reader),
        StartTime = this.m_startTime.GetNullableDateTime((IDataReader) reader),
        FinishTime = this.m_finishTime.GetNullableDateTime((IDataReader) reader),
        QueueTime = this.m_queueTime.GetNullableDateTime((IDataReader) reader),
        Reason = (BuildReason) this.m_reason.GetInt32((IDataReader) reader),
        RequestedFor = this.m_requestedFor.GetGuid((IDataReader) reader),
        RequestedBy = this.m_requestedBy.GetGuid((IDataReader) reader),
        LastChangedDate = this.m_changedOn.GetDateTime((IDataReader) reader),
        LastChangedBy = this.m_changedBy.GetGuid((IDataReader) reader),
        Deleted = this.m_deleted.GetBoolean((IDataReader) reader),
        DeletedBy = new Guid?(this.m_deletedBy.GetGuid((IDataReader) reader, true)),
        DeletedDate = this.m_deletedOn.GetNullableDateTime((IDataReader) reader),
        DeletedReason = this.m_deletedReason.GetString((IDataReader) reader, true),
        SourceVersionInfoString = this.m_sourceVersionInfo.GetString((IDataReader) reader, true),
        TriggerInfoString = this.m_triggerInfo.GetString((IDataReader) reader, true),
        QueueId = this.m_queueId.GetNullableInt32((IDataReader) reader),
        AppendCommitMessageToRunName = this.m_appendCommitMessageToRunName.GetBoolean((IDataReader) reader, false, true)
      };
      string toDeserialize = this.m_templateParameters.GetString((IDataReader) reader, true);
      Dictionary<string, object> dictionary;
      if (!string.IsNullOrEmpty(toDeserialize) && JsonUtility.FromString<Dictionary<string, Dictionary<string, object>>>(toDeserialize).TryGetValue("TemplateParameters", out dictionary))
        buildData1.TemplateParameters = dictionary;
      if (!this.m_result.IsNull((IDataReader) reader))
        buildData1.Result = new BuildResult?((BuildResult) this.m_result.GetByte((IDataReader) reader));
      if (!this.m_repositoryType.IsNull((IDataReader) reader) && !this.m_repositoryIdentifier.IsNull((IDataReader) reader))
        buildData1.Repository = new MinimalBuildRepository()
        {
          Id = this.m_repositoryIdentifier.GetString((IDataReader) reader, true),
          Type = this.m_repositoryType.GetString((IDataReader) reader, true)
        };
      Uri buildUri = UriHelper.CreateBuildUri(buildData1.Id);
      buildData1.Definition = new MinimalBuildDefinition()
      {
        ProjectId = buildData1.ProjectId,
        Id = this.m_definitionId.GetInt32((IDataReader) reader),
        Revision = new int?(this.m_definitionVersion.GetInt32((IDataReader) reader)),
        Name = DBHelper.DBPathToServerPath(this.m_definitionName.GetString((IDataReader) reader, false)),
        QueueStatus = this.m_queueStatus.ColumnExists((IDataReader) reader) ? (DefinitionQueueStatus) this.m_queueStatus.GetByte((IDataReader) reader) : DefinitionQueueStatus.Enabled,
        Path = DBHelper.DBPathToServerPath(this.m_path.GetString((IDataReader) reader, false))
      };
      buildData1.Uri = buildUri;
      buildData1.Logs = new LogReference()
      {
        Type = "Container"
      };
      if (!this.m_validationIssues.IsNull((IDataReader) reader))
        buildData1.ValidationResultsString = this.m_validationIssues.GetString((IDataReader) reader, true);
      if (!this.m_queueOptions.IsNull((IDataReader) reader))
        buildData1.QueueOptions = (QueueOptions) this.m_queueOptions.GetByte((IDataReader) reader);
      Guid? nullable1 = buildData1.DeletedBy;
      Guid empty = Guid.Empty;
      if ((nullable1.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) != 0)
      {
        BuildData buildData2 = buildData1;
        nullable1 = new Guid?();
        Guid? nullable2 = nullable1;
        buildData2.DeletedBy = nullable2;
      }
      buildData1.ChangesCalculated = this.m_changesCalculated.GetBoolean((IDataReader) reader, false);
      if (!this.m_triggeredByDefinitionId.IsNull((IDataReader) reader))
        buildData1.TriggeredByBuild = new TriggeredByBuild()
        {
          ProjectId = resourceComponent.GetDataspaceIdentifier(this.m_triggeredByDataspaceId.GetInt32((IDataReader) reader)),
          DefinitionId = this.m_triggeredByDefinitionId.GetInt32((IDataReader) reader, 0),
          DefinitionVersion = new int?(this.m_triggeredByDefVersion.GetInt32((IDataReader) reader, 0)),
          BuildId = this.m_triggeredByBuildId.GetInt32((IDataReader) reader, 0)
        };
      return buildData1;
    }
  }
}
