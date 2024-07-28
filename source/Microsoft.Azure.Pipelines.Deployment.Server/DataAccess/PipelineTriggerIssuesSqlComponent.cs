// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.PipelineTriggerIssuesSqlComponent
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
  internal class PipelineTriggerIssuesSqlComponent : DeploymentSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<PipelineTriggerIssuesSqlComponent>(1)
    }, "DeploymentPipelineTriggerIssues", "Deployment");

    public PipelineTriggerIssuesSqlComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual void DeletePipelineTriggerIssues(
      Guid projectId,
      IList<int> pipelineDefinitionIds,
      bool isError = false)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (DeletePipelineTriggerIssues)))
      {
        this.PrepareStoredProcedure("Deployment.prc_DeletePipelineTriggerIssues");
        this.BindDataspaceId(projectId);
        this.BindInt32Table("@pipelineDefinitionIds", (IEnumerable<int>) pipelineDefinitionIds);
        this.BindBoolean("@isError", isError);
        this.ExecuteNonQuery();
      }
    }

    public void CreatePipelineTriggerIssues(
      Guid projectId,
      int pipelineDefinitionId,
      IList<PipelineTriggerIssues> triggerIssues)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (CreatePipelineTriggerIssues)))
      {
        this.PrepareStoredProcedure("Deployment.prc_CreatePipelineTriggerIssues");
        this.BindDataspaceId(projectId);
        this.BindInt("@pipelineDefinitionId", pipelineDefinitionId);
        this.BindTriggerTable("@triggerIssues", triggerIssues);
        this.ExecuteNonQuery();
      }
    }

    public virtual IList<PipelineTriggerIssues> GetPipelineTriggerIssues(
      Guid projectId,
      int pipelineDefinitionId)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (GetPipelineTriggerIssues)))
      {
        this.PrepareStoredProcedure("Deployment.prc_GetPipelineTriggerIssues");
        this.BindDataspaceId(projectId);
        this.BindInt("@pipelineDefinitionId", pipelineDefinitionId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<PipelineTriggerIssues>((ObjectBinder<PipelineTriggerIssues>) new PipelineTriggerIssuesBinder());
          return (IList<PipelineTriggerIssues>) resultCollection.GetCurrent<PipelineTriggerIssues>().Items;
        }
      }
    }
  }
}
