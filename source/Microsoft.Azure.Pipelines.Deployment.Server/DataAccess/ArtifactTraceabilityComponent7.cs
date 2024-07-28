// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.ArtifactTraceabilityComponent7
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Models;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  public class ArtifactTraceabilityComponent7 : ArtifactTraceabilityComponent6
  {
    public override IList<CDPipelineRunData> GetCDPipelinesRunsDataForCIPipeline(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineRunId,
      int paginationLimit,
      int continuationToken)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (GetCDPipelinesRunsDataForCIPipeline)))
      {
        this.PrepareStoredProcedure("Deployment.prc_GetCDPipelinesRunsInformationForCIPipelineRun");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
        this.BindInt("@pipelineRunId", pipelineRunId);
        this.BindString("@artifactType", "Pipeline", 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindInt("@top", paginationLimit);
        this.BindInt("@continuationToken", continuationToken);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<CDPipelineRunData>((ObjectBinder<CDPipelineRunData>) new CDPipelineInfoBinder());
          return (IList<CDPipelineRunData>) resultCollection.GetCurrent<CDPipelineRunData>().Items;
        }
      }
    }
  }
}
