// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.PipelineRunTraceabilitySnapshotDataBinder
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability.Models;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  internal class PipelineRunTraceabilitySnapshotDataBinder : 
    ObjectBinder<PipelineRunTraceabilitySnapshot>
  {
    private SqlColumnBinder m_currentRunId = new SqlColumnBinder("CurrentRunId");
    private SqlColumnBinder m_baseRunArtifactVersionse = new SqlColumnBinder("BaseRunArtifactVersions");
    private SqlColumnBinder m_baseRunDetails = new SqlColumnBinder("BaseRunDetails");
    private SqlColumnBinder m_commitsCount = new SqlColumnBinder("CommitsCount");
    private SqlColumnBinder m_workItemsCount = new SqlColumnBinder("WorkItemsCount");

    protected override PipelineRunTraceabilitySnapshot Bind() => new PipelineRunTraceabilitySnapshot()
    {
      CurrentRunId = this.m_currentRunId.GetInt32((IDataReader) this.Reader),
      BaseRunArtifactVersions = this.m_baseRunArtifactVersionse.GetString((IDataReader) this.Reader, true),
      BaseRunDetails = this.m_baseRunDetails.GetString((IDataReader) this.Reader, true),
      CommitsCount = new int?(this.m_commitsCount.GetInt32((IDataReader) this.Reader, -1)),
      WorkItemsCount = new int?(this.m_workItemsCount.GetInt32((IDataReader) this.Reader, -1))
    };
  }
}
