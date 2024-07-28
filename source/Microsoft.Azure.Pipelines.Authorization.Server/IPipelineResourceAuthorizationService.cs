// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Authorization.Server.IPipelineResourceAuthorizationService
// Assembly: Microsoft.Azure.Pipelines.Authorization.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 22B31FF9-0E6B-45B0-A4F8-77598802CAB3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Authorization.Server.dll

using Microsoft.Azure.Pipelines.Authorization.WebApi;
using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.Pipelines.Authorization.Server
{
  [DefaultServiceImplementation(typeof (PipelineResourceAuthorizationService))]
  public interface IPipelineResourceAuthorizationService : IVssFrameworkService
  {
    Task<PipelineProcessResources> GetAuthorizedResourcesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      ResourceType? resourceType = null,
      string resourceId = null);

    Task<PipelineProcessResources> GetProjectAuthorizedResourcesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      ResourceType? resourceType = null,
      string resourceId = null);

    Task<PipelineProcessResources> UpdateResourcesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      PipelineProcessResources resourcesToUpdate,
      int definitionId);

    Task<PipelineProcessResources> UpdateProjectResourcesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      PipelineProcessResources resourcesToUpdate);

    void RemoveDefinitionSpecificAuthorization(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId);

    Task<ResourcePipelinePermissions> GetPipelinePermissionsForResource(
      IVssRequestContext requestContext,
      Guid projectId,
      string resourceType,
      string resourceId);

    Task<List<ResourcePipelinePermissions>> GetPipelinePermissionsForResources(
      IVssRequestContext requestContext,
      Guid projectId,
      List<Resource> resources,
      int? pipelineFilter);

    Task<ResourcePipelinePermissions> UpdatePipelinePermisionsForResource(
      IVssRequestContext requestContext,
      Guid projectId,
      string resourceType,
      string resourceId,
      ResourcePipelinePermissions resourcePipelinePermissions);

    Task DeletePipelinePermissionsForResource(
      IVssRequestContext requestContext,
      Guid projectId,
      string resourceType,
      string resourceId);

    Task<List<ResourcePipelinePermissions>> UpdatePipelinePermisionsForResources(
      IVssRequestContext requestContext,
      Guid projectId,
      List<ResourcePipelinePermissions> resourcePipelinePermissionsList);
  }
}
