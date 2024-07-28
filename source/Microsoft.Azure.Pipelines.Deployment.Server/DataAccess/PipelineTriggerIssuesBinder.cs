// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.PipelineTriggerIssuesBinder
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  internal class PipelineTriggerIssuesBinder : ObjectBinder<PipelineTriggerIssues>
  {
    private SqlColumnBinder m_pipelineDefinitionId = new SqlColumnBinder("PipelineDefinitionId");
    private SqlColumnBinder m_alias = new SqlColumnBinder("Alias");
    private SqlColumnBinder m_buildNumber = new SqlColumnBinder("BuildNumber");
    private SqlColumnBinder m_sourceVersion = new SqlColumnBinder("CommitId");
    private SqlColumnBinder m_repositoryUrl = new SqlColumnBinder("RepositoryUrl");
    private SqlColumnBinder m_createdOn = new SqlColumnBinder("LastMaterializedDate");
    private SqlColumnBinder m_errorMessage = new SqlColumnBinder("ErrorMessage");
    private SqlColumnBinder m_isError = new SqlColumnBinder("IsError");

    protected override PipelineTriggerIssues Bind() => new PipelineTriggerIssues()
    {
      PipelineDefinitionId = this.m_pipelineDefinitionId.GetInt32((IDataReader) this.Reader),
      Alias = this.m_alias.GetString((IDataReader) this.Reader, false),
      BuildNumber = this.m_buildNumber.GetString((IDataReader) this.Reader, false),
      SourceVersion = this.m_sourceVersion.GetString((IDataReader) this.Reader, false),
      RepositoryUrl = this.m_repositoryUrl.GetString((IDataReader) this.Reader, false),
      CreatedOn = this.m_createdOn.GetDateTime((IDataReader) this.Reader),
      ErrorMessage = this.m_errorMessage.GetString((IDataReader) this.Reader, false),
      isError = this.m_isError.GetBoolean((IDataReader) this.Reader)
    };
  }
}
