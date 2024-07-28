// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildDefinitionBinder12
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Data;
using System.Globalization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class BuildDefinitionBinder12 : BuildObjectBinder<BuildDefinition>
  {
    private SqlColumnBinder m_dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder m_definitionId = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder m_definitionVersion = new SqlColumnBinder("DefinitionVersion");
    private SqlColumnBinder m_definitionName = new SqlColumnBinder("DefinitionName");
    private SqlColumnBinder m_quality = new SqlColumnBinder("Quality");
    private SqlColumnBinder m_queueId = new SqlColumnBinder("QueueId");
    private SqlColumnBinder m_queueStatus = new SqlColumnBinder("QueueStatus");
    private SqlColumnBinder m_parentDefinitionId = new SqlColumnBinder("ParentDefinitionId");
    private SqlColumnBinder m_parentName = new SqlColumnBinder("ParentName");
    private SqlColumnBinder m_parentVersion = new SqlColumnBinder("ParentVersion");
    private SqlColumnBinder m_parentQueueStatus = new SqlColumnBinder("ParentQueueStatus");
    private SqlColumnBinder m_author = new SqlColumnBinder("Author");
    private SqlColumnBinder m_createdOn = new SqlColumnBinder("CreatedOn");
    private SqlColumnBinder m_description = new SqlColumnBinder("Description");
    private SqlColumnBinder m_buildNumberFormat = new SqlColumnBinder("BuildNumberFormat");
    private SqlColumnBinder m_jobAuthorizationScope = new SqlColumnBinder("JobAuthorizationScope");
    private SqlColumnBinder m_jobTimeout = new SqlColumnBinder("JobTimeout");
    private SqlColumnBinder m_comment = new SqlColumnBinder("Comment");
    private SqlColumnBinder m_options = new SqlColumnBinder("Options");
    private SqlColumnBinder m_repository = new SqlColumnBinder("Repository");
    private SqlColumnBinder m_triggers = new SqlColumnBinder("Triggers");
    private SqlColumnBinder m_process = new SqlColumnBinder("Process");
    private SqlColumnBinder m_variables = new SqlColumnBinder("Variables");
    private SqlColumnBinder m_demands = new SqlColumnBinder("Demands");
    private SqlColumnBinder m_retentionRules = new SqlColumnBinder("RetentionPolicy");
    private SqlColumnBinder m_badgeEnabled = new SqlColumnBinder("BadgeEnabled");
    private SqlColumnBinder m_path = new SqlColumnBinder("Path");
    private SqlColumnBinder m_processParameters = new SqlColumnBinder("ProcessParameters");
    private SqlColumnBinder m_jobCancelTimeout = new SqlColumnBinder("JobCancelTimeout");

    public BuildDefinitionBinder12(
      IVssRequestContext requestContext,
      BuildSqlComponentBase component)
      : base(requestContext, component)
    {
    }

    protected override BuildDefinition Bind()
    {
      BuildDefinition buildDefinition1 = new BuildDefinition();
      buildDefinition1.ProjectId = this.ResourceComponent.GetDataspaceIdentifier(this.m_dataspaceId.GetInt32((IDataReader) this.Reader));
      buildDefinition1.Id = this.m_definitionId.GetInt32((IDataReader) this.Reader);
      buildDefinition1.Revision = new int?(this.m_definitionVersion.GetInt32((IDataReader) this.Reader));
      buildDefinition1.Name = DBHelper.DBPathToServerPath(this.m_definitionName.GetString((IDataReader) this.Reader, false));
      buildDefinition1.DefinitionQuality = new DefinitionQuality?((DefinitionQuality) this.m_quality.GetByte((IDataReader) this.Reader));
      buildDefinition1.Uri = UriHelper.CreateArtifactUri("Definition", buildDefinition1.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!this.m_queueId.IsNull((IDataReader) this.Reader))
        buildDefinition1.Queue = new AgentPoolQueue()
        {
          Id = this.m_queueId.GetInt32((IDataReader) this.Reader)
        };
      buildDefinition1.QueueStatus = (DefinitionQueueStatus) this.m_queueStatus.GetByte((IDataReader) this.Reader);
      buildDefinition1.AuthoredBy = this.m_author.GetGuid((IDataReader) this.Reader);
      buildDefinition1.CreatedDate = this.m_createdOn.GetDateTime((IDataReader) this.Reader);
      buildDefinition1.Description = this.m_description.GetString((IDataReader) this.Reader, true);
      buildDefinition1.BuildNumberFormat = this.m_buildNumberFormat.GetString((IDataReader) this.Reader, true);
      buildDefinition1.JobAuthorizationScope = (BuildAuthorizationScope) this.m_jobAuthorizationScope.GetByte((IDataReader) this.Reader, (byte) 1);
      buildDefinition1.JobTimeoutInMinutes = this.m_jobTimeout.GetInt32((IDataReader) this.Reader, 0);
      buildDefinition1.JobCancelTimeoutInMinutes = this.m_jobCancelTimeout.GetInt32((IDataReader) this.Reader, 0);
      buildDefinition1.BadgeEnabled = this.m_badgeEnabled.GetBoolean((IDataReader) this.Reader, false);
      buildDefinition1.Comment = this.m_comment.GetString((IDataReader) this.Reader, true);
      buildDefinition1.Path = DBHelper.DBPathToServerPath(this.m_path.GetString((IDataReader) this.Reader, false));
      int? nullableInt32 = this.m_parentDefinitionId.GetNullableInt32((IDataReader) this.Reader);
      if (nullableInt32.GetValueOrDefault() > 0)
      {
        BuildDefinition buildDefinition2 = buildDefinition1;
        BuildDefinition buildDefinition3 = new BuildDefinition();
        buildDefinition3.Id = nullableInt32.Value;
        buildDefinition3.ProjectId = buildDefinition1.ProjectId;
        buildDefinition3.Revision = this.m_parentVersion.GetNullableInt32((IDataReader) this.Reader);
        buildDefinition3.Type = DefinitionType.Build;
        buildDefinition3.Name = DBHelper.DBPathToServerPath(this.m_parentName.GetString((IDataReader) this.Reader, true));
        buildDefinition3.Uri = UriHelper.CreateArtifactUri("Definition", nullableInt32.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        buildDefinition2.ParentDefinition = buildDefinition3;
        byte? nullableByte = this.m_parentQueueStatus.GetNullableByte((IDataReader) this.Reader);
        if (nullableByte.HasValue)
          buildDefinition1.ParentDefinition.QueueStatus = (DefinitionQueueStatus) nullableByte.Value;
      }
      buildDefinition1.BuildOptionsString = this.m_options.GetString((IDataReader) this.Reader, true);
      buildDefinition1.RepositoryString = this.m_repository.GetString((IDataReader) this.Reader, true);
      buildDefinition1.TriggersString = this.m_triggers.GetString((IDataReader) this.Reader, true);
      string toDeserialize = this.m_process.GetString((IDataReader) this.Reader, true);
      if (string.IsNullOrEmpty(toDeserialize))
        buildDefinition1.Process = (BuildProcess) new DesignerProcess()
        {
          Phases = {
            new Phase()
          }
        };
      else
        buildDefinition1.Process = JsonUtility.FromString<BuildProcess>(toDeserialize);
      buildDefinition1.VariablesString = this.m_variables.GetString((IDataReader) this.Reader, true);
      buildDefinition1.DemandsString = this.m_demands.GetString((IDataReader) this.Reader, true);
      this.m_retentionRules.GetString((IDataReader) this.Reader, true);
      buildDefinition1.ProcessParametersString = this.m_processParameters.GetString((IDataReader) this.Reader, true);
      return buildDefinition1;
    }
  }
}
