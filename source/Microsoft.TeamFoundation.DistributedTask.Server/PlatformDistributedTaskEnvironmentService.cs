// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.PlatformDistributedTaskEnvironmentService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class PlatformDistributedTaskEnvironmentService : 
    IDistributedTaskEnvironmentService,
    IVssFrameworkService
  {
    public void CreateTeamProject(IVssRequestContext requestContext, Guid projectId) => requestContext.GetService<EnvironmentService>().CreateTeamProject(requestContext, projectId);

    public void DeleteTeamProject(IVssRequestContext requestContext, Guid projectId) => requestContext.GetService<EnvironmentService>().DeleteTeamProject(requestContext, projectId);

    public EnvironmentInstance AddEnvironment(
      IVssRequestContext requestContext,
      Guid projectId,
      EnvironmentInstance environment,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> administrators = null)
    {
      return requestContext.GetService<EnvironmentService>().AddEnvironment(requestContext, projectId, environment, administrators, "");
    }

    public EnvironmentInstance GetEnvironmentById(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      EnvironmentActionFilter actionFilter = EnvironmentActionFilter.None,
      bool includeResources = false)
    {
      return requestContext.GetService<EnvironmentService>().GetEnvironmentById(requestContext, projectId, environmentId, actionFilter, includeResources, false);
    }

    public async Task<IList<EnvironmentInstance>> GetEnvironmentsByIds(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<int> environmentIds,
      EnvironmentActionFilter actionFilter = EnvironmentActionFilter.None,
      bool includeResources = false)
    {
      return await requestContext.GetService<EnvironmentService>().GetEnvironmentsByIdsAsync(requestContext, projectId, environmentIds, actionFilter, includeResources, false);
    }

    public EnvironmentInstance GetEnvironmentByName(
      IVssRequestContext requestContext,
      Guid projectId,
      string environmentName,
      EnvironmentActionFilter actionFilter = EnvironmentActionFilter.None,
      bool includeResources = false,
      bool includeLinkedResources = false)
    {
      EnvironmentInstance environmentByName = requestContext.GetService<EnvironmentService>().GetEnvironmentByName(requestContext, projectId, environmentName, actionFilter, includeResources, includeLinkedResources);
      if (environmentByName != null)
        environmentByName.ReferencedResources = new PipelineResources();
      return environmentByName;
    }

    public EnvironmentInstance ResolveEnvironmentByName(
      IVssRequestContext requestContext,
      Guid projectId,
      string environmentName,
      EnvironmentActionFilter actionFilter = EnvironmentActionFilter.None,
      bool includeResources = false,
      bool includeLinkedResources = false,
      bool createIfMissing = true)
    {
      return requestContext.GetService<EnvironmentService>().ResolveEnvironmentByName(requestContext, projectId, environmentName, actionFilter, includeResources, includeLinkedResources, createIfMissing);
    }

    public IList<EnvironmentLinkedResourceReference> GetEnvironmentOwnedResources(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId)
    {
      return requestContext.GetService<EnvironmentService>().GetEnvironmentOwnedResources(requestContext, projectId, environmentId);
    }

    public Task RunDeploymentJobAsync(
      IVssRequestContext requestContext,
      ProviderPhaseRequest phaseRequest,
      PhaseExecutionContext phaseExecutionContext)
    {
      return requestContext.GetService<IDeploymentPhaseService>().StartPhaseAsync(requestContext, phaseRequest, phaseExecutionContext);
    }

    public Task NotifyJobCompletedAsync(
      IVssRequestContext requestContext,
      string phaseOrchestrationId,
      JobInstance eventData)
    {
      return requestContext.GetService<IDeploymentPhaseService>().JobCompletedAsync(requestContext, phaseOrchestrationId, eventData);
    }

    public Task NotifyJobStartedAsync(
      IVssRequestContext requestContext,
      string phaseOrchestrationId,
      JobStartedEventData eventData)
    {
      return requestContext.GetService<IDeploymentPhaseService>().JobStartedAsync(requestContext, phaseOrchestrationId, eventData);
    }

    public Task CancelPhaseAsync(
      IVssRequestContext requestContext,
      string phaseOrchestrationId,
      string reason)
    {
      return requestContext.GetService<IDeploymentPhaseService>().CancelPhaseAsync(requestContext, phaseOrchestrationId, reason);
    }

    public IPagedList<EnvironmentInstance> GetEnvironments(
      IVssRequestContext requestContext,
      Guid projectId,
      string environmentName = null,
      string lastEnvironmentName = null,
      EnvironmentActionFilter actionFilter = EnvironmentActionFilter.None,
      int maxEnvironmentsCount = 50)
    {
      return requestContext.GetService<EnvironmentService>().GetEnvironments(requestContext, projectId, environmentName, lastEnvironmentName, actionFilter, maxEnvironmentsCount);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
