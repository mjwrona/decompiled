// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.PipelineTriggerSqlComponent
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
  public class PipelineTriggerSqlComponent : DeploymentSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<PipelineTriggerSqlComponent>(1),
      (IComponentCreator) new ComponentCreator<PipelineTriggerSqlComponent2>(2)
    }, "DeploymentPipelineTrigger", "Deployment");

    public PipelineTriggerSqlComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual void DeletePipelineTriggers(
      Guid projectId,
      IList<int> pipelineDefinitionIds,
      string alias = "")
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (DeletePipelineTriggers)))
      {
        this.PrepareStoredProcedure("Deployment.prc_DeletePipelineTriggers");
        this.BindDataspaceId(projectId);
        this.BindInt32Table("@pipelineDefinitionIds", (IEnumerable<int>) pipelineDefinitionIds);
        this.BindString("@alias", alias, 256, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.ExecuteNonQuery();
      }
    }

    public virtual IList<PipelineDefinitionTrigger> CreatePipelineTrigger(
      Guid projectId,
      int pipelineDefinitionId,
      IList<PipelineDefinitionTrigger> triggers)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (CreatePipelineTrigger)))
      {
        this.PrepareStoredProcedure("Deployment.prc_CreatePipelineTriggers");
        this.BindDataspaceId(projectId);
        this.BindInt("@pipelineDefinitionId", pipelineDefinitionId);
        this.BindTriggerTable("@triggers", triggers);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<PipelineDefinitionTrigger>((ObjectBinder<PipelineDefinitionTrigger>) new PipelineDefinitionTriggerBinder());
          return (IList<PipelineDefinitionTrigger>) resultCollection.GetCurrent<PipelineDefinitionTrigger>().Items;
        }
      }
    }

    public virtual IList<PipelineDefinitionTrigger> GetPipelineTriggers(
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
          resultCollection.AddBinder<PipelineDefinitionTrigger>((ObjectBinder<PipelineDefinitionTrigger>) new PipelineDefinitionTriggerBinder());
          return (IList<PipelineDefinitionTrigger>) resultCollection.GetCurrent<PipelineDefinitionTrigger>().Items;
        }
      }
    }

    public virtual IList<PipelineDefinitionTrigger> GetPipelineTriggers(
      string uniqueResourceIdentifier)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (GetPipelineTriggers)))
      {
        this.PrepareStoredProcedure("Deployment.prc_GetPipelineTriggersByUniqueResourceIdentifier");
        this.BindString("@uniqueResourceIdentifier", uniqueResourceIdentifier, 2048, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<PipelineDefinitionTrigger>((ObjectBinder<PipelineDefinitionTrigger>) new CollectionPipelineDefinitionTriggerBinder(this));
          return (IList<PipelineDefinitionTrigger>) resultCollection.GetCurrent<PipelineDefinitionTrigger>().Items;
        }
      }
    }
  }
}
