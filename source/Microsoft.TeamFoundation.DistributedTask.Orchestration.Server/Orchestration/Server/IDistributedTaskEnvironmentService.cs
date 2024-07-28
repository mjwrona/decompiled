// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.IDistributedTaskEnvironmentService
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
  [DefaultServiceImplementation(typeof (FrameworkEnvironmentService))]
  public interface IDistributedTaskEnvironmentService : IVssFrameworkService
  {
    void CreateTeamProject(IVssRequestContext requestContext, Guid projectId);

    void DeleteTeamProject(IVssRequestContext systemRequestContext, Guid projectId);

    EnvironmentInstance AddEnvironment(
      IVssRequestContext requestContext,
      Guid projectId,
      EnvironmentInstance environment,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> administrators = null);

    EnvironmentInstance GetEnvironmentById(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      EnvironmentActionFilter actionFilter = EnvironmentActionFilter.None,
      bool includeResources = false);

    Task<IList<EnvironmentInstance>> GetEnvironmentsByIds(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<int> environmentIds,
      EnvironmentActionFilter actionFilter = EnvironmentActionFilter.None,
      bool includeResources = false);

    EnvironmentInstance GetEnvironmentByName(
      IVssRequestContext requestContext,
      Guid projectId,
      string environmentName,
      EnvironmentActionFilter actionFilter = EnvironmentActionFilter.None,
      bool includeResources = false,
      bool includeLinkedResources = false);

    EnvironmentInstance ResolveEnvironmentByName(
      IVssRequestContext requestContext,
      Guid projectId,
      string environmentName,
      EnvironmentActionFilter actionFilter = EnvironmentActionFilter.None,
      bool includeResources = false,
      bool includeLinkedResources = false,
      bool createIfMissing = true);

    IPagedList<EnvironmentInstance> GetEnvironments(
      IVssRequestContext requestContext,
      Guid projectId,
      string environmentName = null,
      string lastEnvironmentName = null,
      EnvironmentActionFilter actionFilter = EnvironmentActionFilter.None,
      int maxEnvironmentsCount = 50);

    IList<EnvironmentLinkedResourceReference> GetEnvironmentOwnedResources(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId);

    Task RunDeploymentJobAsync(
      IVssRequestContext requestContext,
      ProviderPhaseRequest phaseRequest,
      PhaseExecutionContext phaseExecutionContext);

    Task NotifyJobStartedAsync(
      IVssRequestContext requestContext,
      string phaseOrchestrationId,
      JobStartedEventData eventData);

    Task NotifyJobCompletedAsync(
      IVssRequestContext requestContext,
      string phaseOrchestrationId,
      JobInstance eventData);

    Task CancelPhaseAsync(
      IVssRequestContext requestContext,
      string phaseOrchestrationId,
      string reason);
  }
}
