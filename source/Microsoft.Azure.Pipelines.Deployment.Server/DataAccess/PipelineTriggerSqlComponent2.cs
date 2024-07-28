// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.PipelineTriggerSqlComponent2
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  public class PipelineTriggerSqlComponent2 : PipelineTriggerSqlComponent
  {
    public PipelineTriggerSqlComponent2() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public override IList<PipelineDefinitionTrigger> CreatePipelineTrigger(
      Guid projectId,
      int pipelineDefinitionId,
      IList<PipelineDefinitionTrigger> triggers)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (CreatePipelineTrigger)))
      {
        this.PrepareStoredProcedure("Deployment.prc_CreatePipelineTriggers");
        this.BindDataspaceId(projectId);
        this.BindInt("@pipelineDefinitionId", pipelineDefinitionId);
        this.BindTriggerTable2("@triggers", triggers);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<PipelineDefinitionTrigger>((ObjectBinder<PipelineDefinitionTrigger>) new PipelineDefinitionTriggerBinder2());
          return (IList<PipelineDefinitionTrigger>) resultCollection.GetCurrent<PipelineDefinitionTrigger>().Items;
        }
      }
    }

    public override IList<PipelineDefinitionTrigger> GetPipelineTriggers(
      Guid projectId,
      int pipelineDefinitionId)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (GetPipelineTriggers)))
      {
        this.PrepareStoredProcedure("Deployment.prc_GetPipelineTriggers");
        this.BindDataspaceId(projectId);
        this.BindInt("@pipelineDefinitionId", pipelineDefinitionId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<PipelineDefinitionTrigger>((ObjectBinder<PipelineDefinitionTrigger>) new PipelineDefinitionTriggerBinder2());
          return (IList<PipelineDefinitionTrigger>) resultCollection.GetCurrent<PipelineDefinitionTrigger>().Items;
        }
      }
    }

    public override IList<PipelineDefinitionTrigger> GetPipelineTriggers(
      string uniqueResourceIdentifier)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (GetPipelineTriggers)))
      {
        this.PrepareStoredProcedure("Deployment.prc_GetPipelineTriggersByUniqueResourceIdentifier");
        this.BindString("@uniqueResourceIdentifier", uniqueResourceIdentifier, 2048, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<PipelineDefinitionTrigger>((ObjectBinder<PipelineDefinitionTrigger>) new CollectionPipelineDefinitionTriggerBinder2(this));
          return (IList<PipelineDefinitionTrigger>) resultCollection.GetCurrent<PipelineDefinitionTrigger>().Items;
        }
      }
    }
  }
}
