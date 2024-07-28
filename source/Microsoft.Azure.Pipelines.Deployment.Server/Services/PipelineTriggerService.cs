// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Services.PipelineTriggerService
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.DataAccess;
using Microsoft.Azure.Pipelines.Deployment.Extensions;
using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Deployment.Services
{
  [DefaultServiceImplementation(typeof (PipelineTriggerService))]
  public class PipelineTriggerService : IPipelineTriggerService, IVssFrameworkService
  {
    private const string c_layer = "PipelineTriggerService";

    public IList<PipelineDefinitionTrigger> CreatePipelineTriggers(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineDefinitionId,
      IList<PipelineDefinitionTrigger> triggers)
    {
      using (new MethodScope(requestContext, nameof (PipelineTriggerService), nameof (CreatePipelineTriggers)))
      {
        using (PipelineTriggerSqlComponent component = requestContext.CreateComponent<PipelineTriggerSqlComponent>())
          return component.CreatePipelineTrigger(projectId, pipelineDefinitionId, triggers);
      }
    }

    public void DeletePipelineTriggers(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<int> pipelineDefinitionIds,
      string alias = "")
    {
      using (new MethodScope(requestContext, nameof (PipelineTriggerService), nameof (DeletePipelineTriggers)))
      {
        using (PipelineTriggerSqlComponent component = requestContext.CreateComponent<PipelineTriggerSqlComponent>())
          component.DeletePipelineTriggers(projectId, pipelineDefinitionIds, alias);
      }
    }

    public IList<PipelineDefinitionTrigger> GetPipelineTriggers(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineDefinitionId)
    {
      IList<PipelineDefinitionTrigger> collection = (IList<PipelineDefinitionTrigger>) null;
      using (new MethodScope(requestContext, nameof (PipelineTriggerService), nameof (GetPipelineTriggers)))
      {
        using (PipelineTriggerSqlComponent component = requestContext.CreateComponent<PipelineTriggerSqlComponent>())
          collection = component.GetPipelineTriggers(projectId, pipelineDefinitionId);
      }
      if (collection != null)
        collection.ForEach<PipelineDefinitionTrigger>((Action<PipelineDefinitionTrigger>) (t => t.ArtifactDefinition.UpdateFromUniqueResourceIdentifier()));
      return collection;
    }

    public IList<PipelineDefinitionTrigger> GetPipelineTriggers(
      IVssRequestContext requestContext,
      string uniqueResourceIdentifier)
    {
      IList<PipelineDefinitionTrigger> collection = (IList<PipelineDefinitionTrigger>) null;
      using (new MethodScope(requestContext, nameof (PipelineTriggerService), nameof (GetPipelineTriggers)))
      {
        using (PipelineTriggerSqlComponent component = requestContext.CreateComponent<PipelineTriggerSqlComponent>())
          collection = component.GetPipelineTriggers(uniqueResourceIdentifier);
      }
      if (collection != null)
        collection.ForEach<PipelineDefinitionTrigger>((Action<PipelineDefinitionTrigger>) (t => t.ArtifactDefinition.UpdateFromUniqueResourceIdentifier()));
      return collection;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
