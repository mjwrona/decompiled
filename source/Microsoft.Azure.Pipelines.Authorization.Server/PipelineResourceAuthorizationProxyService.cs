// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Authorization.Server.PipelineResourceAuthorizationProxyService
// Assembly: Microsoft.Azure.Pipelines.Authorization.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 22B31FF9-0E6B-45B0-A4F8-77598802CAB3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Authorization.Server.dll

using Microsoft.Azure.Pipelines.Authorization.WebApi;
using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.Pipelines.Authorization.Server
{
  public class PipelineResourceAuthorizationProxyService : 
    IPipelineResourceAuthorizationProxyService,
    IVssFrameworkService
  {
    public async Task AuthorizeResourceForAllPipelinesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      string resourceId,
      string typeName)
    {
      string resourceTypeString = ResourceTypeNames.GetResourceTypeString(typeName);
      if (!ResourceTypeNames.TryParse(resourceTypeString, out ResourceType _))
        throw new InvalidResourceType(PipelineAuthorizationResources.InvalidResourceType());
      ResourcePipelinePermissions resourcePipelinePermissions = new ResourcePipelinePermissions()
      {
        AllPipelines = new Permission()
        {
          Authorized = true
        }
      };
      ResourcePipelinePermissions pipelinePermissions = await requestContext.GetService<IPipelineResourceAuthorizationService>().UpdatePipelinePermisionsForResource(requestContext, projectId, resourceTypeString, resourceId, resourcePipelinePermissions);
    }

    public async Task DeletePipelinePermissionsForResource(
      IVssRequestContext requestContext,
      Guid projectId,
      string resourceId,
      string resourceType)
    {
      await requestContext.GetService<IPipelineResourceAuthorizationService>().DeletePipelinePermissionsForResource(requestContext, projectId, resourceType, resourceId);
    }

    public async Task InheritAuthorizationPolicyFromEnvironmentAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      string resourceId,
      string typeName)
    {
      string resourceTypeString = ResourceTypeNames.GetResourceTypeString(typeName);
      if (!ResourceTypeNames.TryParse(resourceTypeString, out ResourceType _))
        throw new InvalidResourceType(PipelineAuthorizationResources.InvalidResourceType());
      IPipelineResourceAuthorizationService authorizationService = requestContext.GetService<IPipelineResourceAuthorizationService>();
      ResourcePipelinePermissions permissionsForResource = await authorizationService.GetPipelinePermissionsForResource(requestContext, projectId, "environment", environmentId.ToString());
      ResourcePipelinePermissions resourcePipelinePermissions = new ResourcePipelinePermissions()
      {
        AllPipelines = permissionsForResource.AllPipelines
      };
      resourcePipelinePermissions.Pipelines = permissionsForResource.Pipelines;
      ResourcePipelinePermissions pipelinePermissions = await authorizationService.UpdatePipelinePermisionsForResource(requestContext, projectId, resourceTypeString, resourceId, resourcePipelinePermissions);
      authorizationService = (IPipelineResourceAuthorizationService) null;
      resourceTypeString = (string) null;
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
