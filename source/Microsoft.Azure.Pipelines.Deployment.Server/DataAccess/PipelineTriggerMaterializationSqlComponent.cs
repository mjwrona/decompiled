// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.PipelineTriggerMaterializationSqlComponent
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  internal class PipelineTriggerMaterializationSqlComponent : DeploymentSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<PipelineTriggerMaterializationSqlComponent>(1)
    }, "DeploymentPipelineTriggerMaterialization", "Deployment");

    public PipelineTriggerMaterializationSqlComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual void DeletePipelineTriggerMaterializationRef(
      Guid projectId,
      IList<int> pipelineDefinitionIds,
      bool deletePartialMateirializationRef)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (DeletePipelineTriggerMaterializationRef)))
      {
        this.PrepareStoredProcedure("Deployment.prc_DeletePipelineTriggerMaterializationRef");
        this.BindDataspaceId(projectId);
        this.BindInt32Table("@pipelineDefinitionIds", (IEnumerable<int>) pipelineDefinitionIds);
        this.BindBoolean("@deletePartialMateirializationRef", deletePartialMateirializationRef);
        this.ExecuteNonQuery();
      }
    }

    public void CreatePipelineTriggerMaterializationRef(
      Guid projectId,
      int pipelineDefinitionId,
      PipelineTriggerMaterializationRef pipelineTriggerMaterializationRef)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (CreatePipelineTriggerMaterializationRef)))
      {
        this.PrepareStoredProcedure("Deployment.prc_CreatePipelineTriggerMaterializationRef");
        this.BindDataspaceId(projectId);
        this.BindInt("@pipelineDefinitionId", pipelineDefinitionId);
        this.BindTriggerTable("@triggerMaterializationData", pipelineTriggerMaterializationRef);
        this.ExecuteNonQuery();
      }
    }

    public virtual PipelineTriggerMaterializationRef GetPipelineTriggerMaterializationRef(
      Guid projectId,
      int pipelineDefinitionId)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (GetPipelineTriggerMaterializationRef)))
      {
        this.PrepareStoredProcedure("Deployment.prc_GetPipelineTriggerMaterializationRef");
        this.BindDataspaceId(projectId);
        this.BindInt("@pipelineDefinitionId", pipelineDefinitionId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<PipelineTriggerMaterializationRef>((ObjectBinder<PipelineTriggerMaterializationRef>) new PipelineTriggerMaterializationBinder());
          return resultCollection.GetCurrent<PipelineTriggerMaterializationRef>().First<PipelineTriggerMaterializationRef>();
        }
      }
    }
  }
}
