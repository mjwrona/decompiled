// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.IEnvironmentService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Server.Data.Model;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  [DefaultServiceImplementation(typeof (EnvironmentService))]
  public interface IEnvironmentService : IVssFrameworkService
  {
    EnvironmentInstance AddEnvironment(
      IVssRequestContext requestContext,
      Guid projectId,
      EnvironmentInstance environment,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> administrators = null,
      string clientSource = "");

    EnvironmentInstance GetEnvironmentById(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      EnvironmentActionFilter actionFilter = EnvironmentActionFilter.None,
      bool includeResourceReferences = false,
      bool includeLinkedResources = false);

    EnvironmentInstance GetEnvironmentByName(
      IVssRequestContext requestContext,
      Guid projectId,
      string environmentName,
      EnvironmentActionFilter actionFilter = EnvironmentActionFilter.None,
      bool includeResourceReferences = false,
      bool includeLinkedResources = false);

    IList<EnvironmentInstance> GetEnvironmentsByModifiedTime(
      IVssRequestContext requestContext,
      Guid projectId,
      DateTime? fromDate,
      int batchSize);

    IPagedList<EnvironmentInstance> GetEnvironments(
      IVssRequestContext requestContext,
      Guid projectId,
      string environmentName = null,
      string lastEnvironmentName = null,
      EnvironmentActionFilter actionFilter = EnvironmentActionFilter.None,
      int maxEnvironmentsCount = 50);

    EnvironmentInstance UpdateEnvironment(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      EnvironmentInstance environment);

    void DeleteEnvironment(IVssRequestContext requestContext, Guid projectId, int environmentId);

    IPagedList<EnvironmentObject> GetEnvironmentsWithFilters(
      IVssRequestContext requestContext,
      Guid projectId,
      string environmentName = null,
      string lastEnvironmentName = null,
      EnvironmentActionFilter actionFilter = EnvironmentActionFilter.None,
      bool includeLastCompletedRequest = false,
      EnvironmentJobStatus environmentJobStatus = EnvironmentJobStatus.None,
      int maxEnvironmentsCount = 50);

    EnvironmentObject GetEnvironmentResourceWithFilters(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      string resourceName = null,
      EnvironmentResourceType environmentResourceTypeFilter = EnvironmentResourceType.Generic | EnvironmentResourceType.VirtualMachine | EnvironmentResourceType.Kubernetes,
      EnvironmentActionFilter actionFilter = EnvironmentActionFilter.None,
      bool includeResourceReferences = false,
      bool includeLastCompletedRequest = false,
      EnvironmentJobStatus environmentJobStatus = EnvironmentJobStatus.None);

    EnvironmentInstance ResolveEnvironmentByName(
      IVssRequestContext requestContext,
      Guid projectId,
      string environmentName,
      EnvironmentActionFilter actionFilter,
      bool includeResources,
      bool includeResourceReferences,
      bool autopopulateIfMissing);

    Task<IList<EnvironmentInstance>> GetEnvironmentsByIdsAsync(
      IVssRequestContext tfsRequestContext,
      Guid ProjectId,
      IList<int> idsValue,
      EnvironmentActionFilter actionFilter,
      bool includeResourceReferences = false,
      bool includeLinkedResources = false);

    TaskAgentPoolReference GetEnvironmentPool(
      IVssRequestContext requestContext,
      string environmentName);

    TaskAgentPoolReference GetEnvironmentPool(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId);

    Task<TaskAgentPoolReference> ProvisionEnvironmentPoolAsync(
      IVssRequestContext requestContext,
      string environmentName);

    Task<TaskAgentPoolReference> ProvisionEnvironmentPoolAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId);

    string GeneratePersonalAccessTokenWithEnvironmentScope(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId);
  }
}
