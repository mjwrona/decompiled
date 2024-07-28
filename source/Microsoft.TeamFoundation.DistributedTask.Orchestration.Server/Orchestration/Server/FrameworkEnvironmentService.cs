// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.FrameworkEnvironmentService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal class FrameworkEnvironmentService : 
    IDistributedTaskEnvironmentService,
    IVssFrameworkService
  {
    public Task CancelPhaseAsync(
      IVssRequestContext requestContext,
      string phaseOrchestrationId,
      string reason)
    {
      throw new NotImplementedException();
    }

    public void CreateTeamProject(IVssRequestContext requestContext, Guid projectId) => throw new InvalidOperationException("CreateTeamProject can only be called by ProjectCreate event.");

    public void DeleteTeamProject(IVssRequestContext requestContext, Guid projectId) => throw new InvalidOperationException("DeleteTeamProject can only be called by ProjectDelete event.");

    public EnvironmentInstance AddEnvironment(
      IVssRequestContext requestContext,
      Guid projectId,
      EnvironmentInstance environment,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> administrators = null)
    {
      throw new NotSupportedException();
    }

    public EnvironmentInstance GetEnvironmentById(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      EnvironmentActionFilter actionFilter = EnvironmentActionFilter.None,
      bool includeResources = false)
    {
      throw new NotSupportedException();
    }

    public EnvironmentInstance GetEnvironmentByName(
      IVssRequestContext requestContext,
      Guid projectId,
      string environmentName,
      EnvironmentActionFilter actionFilter = EnvironmentActionFilter.None,
      bool includeResources = false,
      bool includeLinkedResources = false)
    {
      throw new NotSupportedException();
    }

    public Task NotifyJobCompletedAsync(
      IVssRequestContext requestContext,
      string phaseOrchestrationId,
      JobInstance eventData)
    {
      throw new NotImplementedException();
    }

    public Task NotifyJobStartedAsync(
      IVssRequestContext requestContext,
      string phaseOrchestrationId,
      JobStartedEventData eventData)
    {
      throw new NotImplementedException();
    }

    public Task RunDeploymentJobAsync(
      IVssRequestContext requestContext,
      ProviderPhaseRequest phaseRequest,
      PhaseExecutionContext phaseExecutionContext)
    {
      throw new NotImplementedException();
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
      throw new NotImplementedException();
    }

    public Task<IList<EnvironmentInstance>> GetEnvironmentsByIds(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<int> environmentIds,
      EnvironmentActionFilter actionFilter = EnvironmentActionFilter.None,
      bool includeResources = false)
    {
      throw new NotImplementedException();
    }

    public IPagedList<EnvironmentInstance> GetEnvironments(
      IVssRequestContext requestContext,
      Guid projectId,
      string environmentName = null,
      string lastEnvironmentName = null,
      EnvironmentActionFilter actionFilter = EnvironmentActionFilter.None,
      int maxEnvironmentsCount = 50)
    {
      throw new NotImplementedException();
    }

    public IList<EnvironmentLinkedResourceReference> GetEnvironmentOwnedResources(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId)
    {
      throw new NotImplementedException();
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
