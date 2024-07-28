// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Services.PipelineTriggerMaterializationService
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.DataAccess;
using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Deployment.Services
{
  internal class PipelineTriggerMaterializationService : 
    IPipelineTriggerMaterializationService,
    IVssFrameworkService
  {
    private const string c_layer = "PipelineTriggerMaterializationService";

    public void CreatePipelineTriggerMaterializationRef(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineDefinitionId,
      PipelineTriggerMaterializationRef pipelineTriggerMaterializationRef)
    {
      using (new MethodScope(requestContext, nameof (PipelineTriggerMaterializationService), nameof (CreatePipelineTriggerMaterializationRef)))
      {
        using (PipelineTriggerMaterializationSqlComponent component = requestContext.CreateComponent<PipelineTriggerMaterializationSqlComponent>())
          component.CreatePipelineTriggerMaterializationRef(projectId, pipelineDefinitionId, pipelineTriggerMaterializationRef);
      }
    }

    public void DeletePipelineTriggerMaterializationRef(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<int> pipelineDefinitionIds,
      bool deletePartialMateirializationRef)
    {
      using (new MethodScope(requestContext, nameof (PipelineTriggerMaterializationService), nameof (DeletePipelineTriggerMaterializationRef)))
      {
        using (PipelineTriggerMaterializationSqlComponent component = requestContext.CreateComponent<PipelineTriggerMaterializationSqlComponent>())
          component.DeletePipelineTriggerMaterializationRef(projectId, pipelineDefinitionIds, deletePartialMateirializationRef);
      }
    }

    public PipelineTriggerMaterializationRef GetPipelineTriggerMaterializationRef(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineDefinitionId)
    {
      using (new MethodScope(requestContext, nameof (PipelineTriggerMaterializationService), nameof (GetPipelineTriggerMaterializationRef)))
      {
        using (PipelineTriggerMaterializationSqlComponent component = requestContext.CreateComponent<PipelineTriggerMaterializationSqlComponent>())
          return component.GetPipelineTriggerMaterializationRef(projectId, pipelineDefinitionId);
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
