// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Server.IPipelineBuilderService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Server
{
  [DefaultServiceImplementation(typeof (PipelineBuilderService))]
  public interface IPipelineBuilderService : IVssFrameworkService
  {
    PipelineBuilder GetBuilder(
      IVssRequestContext requestContext,
      Guid projectId,
      string planType,
      int definitionId,
      PipelineResources authorizedResources,
      bool authorizeNewResources = false,
      bool evaluateCounters = false,
      IDictionary<string, int> counters = null);

    PipelineBuilder GetBuilder(
      IVssRequestContext requestContext,
      Guid projectId,
      string planType,
      int planVersion,
      int definitionId,
      Guid planId,
      PipelineEnvironment environment);

    PipelineBuilder GetBuilder(IVssRequestContext requestContext, TaskOrchestrationPlan plan);

    ICounterStore GetCounterStore(
      IVssRequestContext requestContext,
      bool evaluateCounters = false,
      IDictionary<string, int> counters = null);

    IPackageStore GetPackageStore(IVssRequestContext requestContext);

    IResourceStore GetResourceStore(
      IVssRequestContext requestContext,
      Guid projectId,
      PipelineResources authorizedResources,
      bool authorizeNewResources = false);

    ITaskStore GetTaskStore(IVssRequestContext requestContext);

    ITaskTemplateStore GetTaskTemplateStore(IVssRequestContext requestContext, Guid projectId);
  }
}
