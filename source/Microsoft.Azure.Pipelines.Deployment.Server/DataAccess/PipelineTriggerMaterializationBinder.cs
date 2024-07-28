// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.PipelineTriggerMaterializationBinder
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  internal class PipelineTriggerMaterializationBinder : 
    ObjectBinder<PipelineTriggerMaterializationRef>
  {
    private SqlColumnBinder m_yamlFileName = new SqlColumnBinder("YAMLFileName");
    private SqlColumnBinder m_commitId = new SqlColumnBinder("CommitId");
    private SqlColumnBinder m_repositoryUrl = new SqlColumnBinder("RepositoryUrl");
    private SqlColumnBinder m_lastMaterializedDate = new SqlColumnBinder("LastMaterializedDate");

    protected override PipelineTriggerMaterializationRef Bind() => new PipelineTriggerMaterializationRef()
    {
      YAMLFileName = this.m_yamlFileName.GetString((IDataReader) this.Reader, false),
      CommitId = this.m_commitId.GetString((IDataReader) this.Reader, false),
      RepositoryUrl = this.m_repositoryUrl.GetString((IDataReader) this.Reader, false),
      LastMaterializedDate = this.m_lastMaterializedDate.GetDateTime((IDataReader) this.Reader)
    };
  }
}
