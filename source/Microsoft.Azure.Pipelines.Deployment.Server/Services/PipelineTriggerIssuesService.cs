// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Services.PipelineTriggerIssuesService
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
  internal class PipelineTriggerIssuesService : IPipelineTriggerIssuesService, IVssFrameworkService
  {
    private const string c_layer = "PipelineTriggerIssuesService";

    public void CreatePipelineTriggerIssues(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineDefinitionId,
      IList<PipelineTriggerIssues> triggerIssues)
    {
      using (new MethodScope(requestContext, nameof (PipelineTriggerIssuesService), nameof (CreatePipelineTriggerIssues)))
      {
        using (PipelineTriggerIssuesSqlComponent component = requestContext.CreateComponent<PipelineTriggerIssuesSqlComponent>())
          component.CreatePipelineTriggerIssues(projectId, pipelineDefinitionId, triggerIssues);
      }
    }

    public void DeletePipelineTriggerIssues(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<int> pipelineDefinitionIds,
      bool isError = false)
    {
      using (new MethodScope(requestContext, nameof (PipelineTriggerIssuesService), nameof (DeletePipelineTriggerIssues)))
      {
        using (PipelineTriggerIssuesSqlComponent component = requestContext.CreateComponent<PipelineTriggerIssuesSqlComponent>())
          component.DeletePipelineTriggerIssues(projectId, pipelineDefinitionIds, isError);
      }
    }

    public IList<PipelineTriggerIssues> GetPipelineTriggerIssues(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineDefinitionId)
    {
      using (new MethodScope(requestContext, nameof (PipelineTriggerIssuesService), nameof (GetPipelineTriggerIssues)))
      {
        using (PipelineTriggerIssuesSqlComponent component = requestContext.CreateComponent<PipelineTriggerIssuesSqlComponent>())
          return component.GetPipelineTriggerIssues(projectId, pipelineDefinitionId);
      }
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
