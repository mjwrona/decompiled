// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Services.IPipelineTriggerService
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Deployment.Services
{
  [DefaultServiceImplementation(typeof (PipelineTriggerService))]
  public interface IPipelineTriggerService : IVssFrameworkService
  {
    IList<PipelineDefinitionTrigger> CreatePipelineTriggers(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineDefinitionId,
      IList<PipelineDefinitionTrigger> triggers);

    void DeletePipelineTriggers(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<int> pipelineDefinitionIds,
      string alias = "");

    IList<PipelineDefinitionTrigger> GetPipelineTriggers(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineDefinitionId);

    IList<PipelineDefinitionTrigger> GetPipelineTriggers(
      IVssRequestContext requestContext,
      string uniqueResourceIdentifier);
  }
}
