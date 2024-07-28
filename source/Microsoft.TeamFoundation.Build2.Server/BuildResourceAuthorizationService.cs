// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildResourceAuthorizationService
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.Azure.Pipelines.Authorization.Server;
using Microsoft.Azure.Pipelines.Authorization.WebApi;
using Microsoft.TeamFoundation.Build2.Server.ObjectModel;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class BuildResourceAuthorizationService : 
    IBuildResourceAuthorizationService,
    IVssFrameworkService,
    IBuildResourceAuthorizationServiceInternal
  {
    private const string TraceLayer = "BuildResourceAuthorizationService";

    public async Task<BuildProcessResources> GetResourcesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId)
    {
      using (requestContext.TraceScope(nameof (BuildResourceAuthorizationService), nameof (GetResourcesAsync)))
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
        BuildDefinition definition = await requestContext.GetService<IBuildDefinitionService>().GetDefinitionAsync(requestContext, projectId, definitionId);
        if (definition == null)
          throw new Microsoft.TeamFoundation.Build.WebApi.DefinitionNotFoundException(BuildServerResources.DefinitionNotFound((object) definitionId));
        BuildProcessResources authorizedResourcesAsync = await this.GetAuthorizedResourcesAsync(requestContext, definition, new ResourceType?(), (string) null);
        try
        {
          BuildProcessResources processResources = definition.LoadYamlPipeline(requestContext, true, authorizedResourcesAsync).Environment.Resources.ToBuildProcessResources();
          processResources.ExceptWith(authorizedResourcesAsync);
          foreach (ResourceReference allResource in processResources.AllResources)
            allResource.Authorized = false;
          return BuildProcessResources.Merge(authorizedResourcesAsync, processResources);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(nameof (BuildResourceAuthorizationService), ex);
          return authorizedResourcesAsync;
        }
      }
    }

    public async Task<BuildProcessResources> GetAuthorizedResourcesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      ResourceType? resourceType,
      string resourceId)
    {
      BuildProcessResources processResources1;
      using (requestContext.TraceScope(nameof (BuildResourceAuthorizationService), nameof (GetAuthorizedResourcesAsync)))
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
        if (!string.IsNullOrEmpty(resourceId) && !resourceType.HasValue)
          throw new InvalidOperationException(BuildServerResources.ResourcesQueryRequiresType());
        BuildProcessResources processResources2 = new BuildProcessResources();
        Microsoft.Azure.Pipelines.Checks.WebApi.ResourceType? pipelineResourceType = resourceType.ToPipelineResourceType();
        processResources1 = (await requestContext.GetService<IPipelineResourceAuthorizationService>().GetProjectAuthorizedResourcesAsync(requestContext, projectId, pipelineResourceType, resourceId)).ToBuildProcessResources();
      }
      return processResources1;
    }

    public async Task<BuildProcessResources> GetAuthorizedResourcesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      ResourceType? resourceType,
      string resourceId)
    {
      BuildProcessResources authorizedResourcesAsync;
      using (requestContext.TraceScope(nameof (BuildResourceAuthorizationService), nameof (GetAuthorizedResourcesAsync)))
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
        if (!string.IsNullOrEmpty(resourceId) && !resourceType.HasValue)
          throw new InvalidOperationException(BuildServerResources.ResourcesQueryRequiresType());
        BuildDefinition definitionAsync = await requestContext.GetService<IBuildDefinitionService>().GetDefinitionAsync(requestContext, projectId, definitionId);
        if (definitionAsync == null)
          throw new Microsoft.TeamFoundation.Build.WebApi.DefinitionNotFoundException(BuildServerResources.DefinitionNotFound((object) definitionId));
        authorizedResourcesAsync = await this.GetAuthorizedResourcesAsync(requestContext, definitionAsync, resourceType, resourceId);
      }
      return authorizedResourcesAsync;
    }

    public async Task<BuildProcessResources> UpdateResourcesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      BuildProcessResources resourcesToUpdate)
    {
      using (requestContext.TraceScope(nameof (BuildResourceAuthorizationService), nameof (UpdateResourcesAsync)))
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
        ArgumentUtility.CheckForNull<BuildProcessResources>(resourcesToUpdate, nameof (resourcesToUpdate));
        BuildDefinition definition = await requestContext.GetService<IBuildDefinitionService>().GetDefinitionAsync(requestContext, projectId, definitionId);
        BuildProcess buildProcess = definition != null ? definition.Process : throw new Microsoft.TeamFoundation.Build.WebApi.DefinitionNotFoundException(BuildServerResources.DefinitionNotFound((object) definitionId));
        if ((buildProcess != null ? (buildProcess.Type != 2 ? 1 : 0) : 1) != 0)
          return new BuildProcessResources();
        BuildProcessResources resourcesToAuthorize = resourcesToUpdate.GetAuthorizedResources();
        BuildProcessResources resourcesToUnauthorize = resourcesToUpdate.GetUnauthorizedResources();
        resourcesToAuthorize.ExceptWith(await this.GetAuthorizedResourcesAsync(requestContext, definition, new ResourceType?(), (string) null));
        PipelineProcessResources processResources1 = BuildProcessResources.Merge(requestContext.GetService<IPipelineBuilderService>().AuthorizeResources(requestContext, projectId, resourcesToAuthorize), resourcesToUnauthorize).ToPipelineProcessResources();
        BuildProcessResources processResources2 = (await requestContext.GetService<IPipelineResourceAuthorizationService>().UpdateResourcesAsync(requestContext, projectId, processResources1, definitionId)).ToBuildProcessResources();
        BuildProcessResources right = new BuildProcessResources();
        try
        {
          right = definition.LoadYamlPipeline(requestContext, true, processResources2).Environment.Resources.ToBuildProcessResources();
          right.ExceptWith(processResources2);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(nameof (BuildResourceAuthorizationService), ex);
        }
        return BuildProcessResources.Merge(processResources2, right);
      }
    }

    public async Task<BuildProcessResources> UpdateResourcesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildProcessResources resourcesToUpdate)
    {
      BuildProcessResources processResources1;
      using (requestContext.TraceScope(nameof (BuildResourceAuthorizationService), nameof (UpdateResourcesAsync)))
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
        ArgumentUtility.CheckForNull<BuildProcessResources>(resourcesToUpdate, nameof (resourcesToUpdate));
        BuildProcessResources authorizedResources = resourcesToUpdate.GetAuthorizedResources();
        BuildProcessResources unauthorizedResources = resourcesToUpdate.GetUnauthorizedResources();
        IPipelineBuilderService service = requestContext.GetService<IPipelineBuilderService>();
        PipelineProcessResources processResources2 = BuildProcessResources.Merge(service.AuthorizeResources(requestContext, projectId, authorizedResources, ResourceActionFilter.Manage), service.AuthorizeResources(requestContext, projectId, unauthorizedResources, ResourceActionFilter.Manage, false)).ToPipelineProcessResources();
        processResources1 = (await requestContext.GetService<IPipelineResourceAuthorizationService>().UpdateProjectResourcesAsync(requestContext, projectId, processResources2)).ToBuildProcessResources();
      }
      return processResources1;
    }

    public async Task<BuildProcessResources> GetAuthorizedResourcesAsync(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      ResourceType? resourceType,
      string resourceId)
    {
      ArgumentUtility.CheckForNull<BuildDefinition>(definition, nameof (definition));
      if (!string.IsNullOrEmpty(resourceId) && !resourceType.HasValue)
        throw new InvalidOperationException(BuildServerResources.ResourcesQueryRequiresType());
      BuildProcessResources authorizedResourcesAsync;
      using (requestContext.TraceScope(nameof (BuildResourceAuthorizationService), nameof (GetAuthorizedResourcesAsync)))
      {
        Microsoft.Azure.Pipelines.Checks.WebApi.ResourceType? pipelineResourceType = resourceType.ToPipelineResourceType();
        BuildProcessResources left = (await requestContext.GetService<IPipelineResourceAuthorizationService>().GetAuthorizedResourcesAsync(requestContext, definition.ProjectId, definition.Id, pipelineResourceType, resourceId)).ToBuildProcessResources();
        if ((definition.Process is YamlCompatProcess process ? process.Resources : (BuildProcessResources) null) != null)
        {
          foreach (ResourceReference allResource in process.Resources.AllResources)
            allResource.DefinitionId = new int?(definition.Id);
          left = BuildProcessResources.Merge(left, process.Resources);
        }
        authorizedResourcesAsync = left;
      }
      return authorizedResourcesAsync;
    }

    private void AuditResourceAuthorizations(
      IVssRequestContext requestContext,
      int? definitionId,
      BuildProcessResources resourcesToAuthorize,
      BuildProcessResources resourcesToUnauthorize,
      BuildProcessResources newlyAuthorizedResources)
    {
      foreach (ResourceReference allResource in resourcesToAuthorize.AllResources)
      {
        Dictionary<string, object> data = new Dictionary<string, object>()
        {
          {
            "ResourceId",
            (object) allResource.GetId()
          },
          {
            "ResourceType",
            (object) allResource.Type
          }
        };
        if (definitionId.HasValue)
        {
          data.Add("PipelineId", (object) definitionId.ToString());
          if (newlyAuthorizedResources.Contains(allResource))
            requestContext.LogAuditEvent("Pipelines.ResourceAuthorizedForPipeline", data);
          else
            requestContext.LogAuditEvent("Pipelines.ResourceNotAuthorizedForPipeline", data);
        }
        else if (newlyAuthorizedResources.Contains(allResource))
          requestContext.LogAuditEvent("Pipelines.ResourceAuthorizedForProject", data);
        else
          requestContext.LogAuditEvent("Pipelines.ResourceNotAuthorizedForProject", data);
      }
      foreach (ResourceReference allResource in resourcesToUnauthorize.AllResources)
      {
        Dictionary<string, object> data = new Dictionary<string, object>()
        {
          {
            "ResourceId",
            (object) allResource.GetId()
          },
          {
            "ResourceType",
            (object) allResource.Type
          }
        };
        if (definitionId.HasValue)
        {
          data.Add("PipelineId", (object) definitionId.ToString());
          requestContext.LogAuditEvent("Pipelines.ResourceUnauthorizedForPipeline", data);
        }
        else
          requestContext.LogAuditEvent("Pipelines.ResourceUnauthorizedForProject", data);
      }
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
