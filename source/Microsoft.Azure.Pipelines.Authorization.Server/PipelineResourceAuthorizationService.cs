// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Authorization.Server.PipelineResourceAuthorizationService
// Assembly: Microsoft.Azure.Pipelines.Authorization.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 22B31FF9-0E6B-45B0-A4F8-77598802CAB3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Authorization.Server.dll

using Microsoft.Azure.Pipelines.Authorization.Server.DataAccess;
using Microsoft.Azure.Pipelines.Authorization.WebApi;
using Microsoft.Azure.Pipelines.Checks.Common;
using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.Pipelines.Authorization.Server
{
  internal class PipelineResourceAuthorizationService : 
    IPipelineResourceAuthorizationService,
    IVssFrameworkService
  {
    private const string TraceLayer = "PipelineResourceAuthorizationService";

    public async Task<PipelineProcessResources> GetAuthorizedResourcesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      ResourceType? resourceType,
      string resourceId)
    {
      PipelineProcessResources authorizedResourcesAsync1;
      using (new MethodScope(requestContext, nameof (PipelineResourceAuthorizationService), nameof (GetAuthorizedResourcesAsync)))
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "PipelinePolicy.Authorization");
        if (!string.IsNullOrEmpty(resourceId) && !resourceType.HasValue)
          throw new InvalidOperationException(PipelineAuthorizationResources.ResourcesQueryRequiresType());
        await this.ValidateDefinition(requestContext, projectId, definitionId);
        PipelineProcessResources authorizedResourcesAsync2;
        using (PipelineResourceAuthorizationComponent rac = requestContext.CreateComponent<PipelineResourceAuthorizationComponent>())
          authorizedResourcesAsync2 = await rac.GetAuthorizedResourcesAsync(projectId, new int?(definitionId), resourceType, resourceId);
        if (requestContext.IsFeatureEnabled("Pipelines.Policy.IgnoreResourcesAuthorizedForAll"))
          authorizedResourcesAsync2.Resources.RemoveAll((Predicate<PipelineResourceReference>) (resource => !resource.DefinitionId.HasValue));
        authorizedResourcesAsync1 = authorizedResourcesAsync2;
      }
      return authorizedResourcesAsync1;
    }

    public async Task<PipelineProcessResources> GetProjectAuthorizedResourcesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      ResourceType? resourceType,
      string resourceId)
    {
      PipelineProcessResources authorizedResourcesAsync;
      using (requestContext.TraceScope(nameof (PipelineResourceAuthorizationService), nameof (GetProjectAuthorizedResourcesAsync)))
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "PipelinePolicy.Authorization");
        if (!string.IsNullOrEmpty(resourceId) && !resourceType.HasValue)
          throw new InvalidOperationException(PipelineAuthorizationResources.ResourcesQueryRequiresType());
        using (PipelineResourceAuthorizationComponent rac = requestContext.CreateComponent<PipelineResourceAuthorizationComponent>())
          authorizedResourcesAsync = await rac.GetAuthorizedResourcesAsync(projectId, new int?(0), resourceType, resourceId);
      }
      return authorizedResourcesAsync;
    }

    public async Task<PipelineProcessResources> UpdateResourcesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      PipelineProcessResources resourcesToUpdate,
      int definitionId)
    {
      PipelineProcessResources processResources1;
      using (new MethodScope(requestContext, nameof (PipelineResourceAuthorizationService), nameof (UpdateResourcesAsync)))
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "PipelinePolicy.Authorization");
        ArgumentUtility.CheckForNull<PipelineProcessResources>(resourcesToUpdate, nameof (resourcesToUpdate));
        await this.ValidateDefinition(requestContext, projectId, definitionId);
        PipelineProcessResources processResources2;
        using (PipelineResourceAuthorizationComponent rac = requestContext.CreateComponent<PipelineResourceAuthorizationComponent>())
          processResources2 = await rac.UpdateAuthorizedResourcesAsync(projectId, new int?(definitionId), requestContext.GetUserId(true), resourcesToUpdate);
        CustomerIntelligenceData telemetryData = new CustomerIntelligenceData();
        List<Dictionary<string, object>> dictionaryList1 = new List<Dictionary<string, object>>();
        List<Dictionary<string, object>> dictionaryList2 = new List<Dictionary<string, object>>();
        foreach (PipelineResourceReference resource in resourcesToUpdate.Resources)
        {
          this.AuditResourceAuthorizations(requestContext, projectId, new int?(definitionId), resource.Type, resource.Id, resource.Authorized);
          if (resource.Authorized)
            dictionaryList1.Add(new Dictionary<string, object>()
            {
              ["resourceType"] = (object) resource.Type,
              ["resourceId"] = (object) resource.Id,
              ["definitionsUpdated"] = (object) new List<int>()
              {
                definitionId
              }
            });
          else
            dictionaryList2.Add(new Dictionary<string, object>()
            {
              ["resourceType"] = (object) resource.Type,
              ["resourceId"] = (object) resource.Id,
              ["definitionsUpdated"] = (object) new List<int>()
              {
                definitionId
              }
            });
        }
        telemetryData.Add("Added", (object) dictionaryList1);
        telemetryData.Add("Removed", (object) dictionaryList2);
        this.AddTelemetry(requestContext, "PipelineResourceAuthorizationService.UpdateAuthorize", telemetryData);
        processResources1 = processResources2;
      }
      return processResources1;
    }

    public async Task<PipelineProcessResources> UpdateProjectResourcesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      PipelineProcessResources resourcesToUpdate)
    {
      PipelineProcessResources processResources1;
      using (requestContext.TraceScope(nameof (PipelineResourceAuthorizationService), nameof (UpdateProjectResourcesAsync)))
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "PipelinePolicy.Authorization");
        ArgumentUtility.CheckForNull<PipelineProcessResources>(resourcesToUpdate, nameof (resourcesToUpdate));
        PipelineProcessResources processResources2 = (PipelineProcessResources) null;
        using (PipelineResourceAuthorizationComponent rac = requestContext.CreateComponent<PipelineResourceAuthorizationComponent>())
          processResources2 = await rac.UpdateAuthorizedResourcesAsync(projectId, new int?(), requestContext.GetUserId(true), resourcesToUpdate);
        CustomerIntelligenceData telemetryData = new CustomerIntelligenceData();
        List<string> values1 = new List<string>();
        List<string> values2 = new List<string>();
        foreach (PipelineResourceReference resource in resourcesToUpdate.Resources)
        {
          this.AuditResourceAuthorizations(requestContext, projectId, new int?(), resource.Type, resource.Id, resource.Authorized);
          if (resource.Authorized)
            values1.Add(string.Format("{0}.{1} ", (object) resource.Type, (object) resource.Id));
          else
            values2.Add(string.Format("{0}.{1} ", (object) resource.Type, (object) resource.Id));
        }
        telemetryData.Add("NewStateIsRestricted", values1);
        telemetryData.Add("NewStateIsUnRestricted", values2);
        this.AddTelemetry(requestContext, "PipelineResourceAuthorizationService.IsAuthorizedForAllDefinitions", telemetryData);
        processResources1 = processResources2;
      }
      return processResources1;
    }

    public void RemoveDefinitionSpecificAuthorization(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId)
    {
      using (requestContext.TraceScope(nameof (PipelineResourceAuthorizationService), nameof (RemoveDefinitionSpecificAuthorization)))
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "PipelinePolicy.Authorization");
        using (PipelineResourceAuthorizationComponent component = requestContext.CreateComponent<PipelineResourceAuthorizationComponent>())
          component.RemoveDefinitionSpecificAuthorization(projectId, definitionId);
      }
    }

    public async Task<ResourcePipelinePermissions> GetPipelinePermissionsForResource(
      IVssRequestContext requestContext,
      Guid projectId,
      string resourceType,
      string resourceId)
    {
      PipelineResourceAuthorizationService authorizationService1 = this;
      ResourcePipelinePermissions permissionsForResource;
      using (requestContext.TraceScope(nameof (PipelineResourceAuthorizationService), nameof (GetPipelinePermissionsForResource)))
      {
        PipelineResourceAuthorizationService authorizationService2 = authorizationService1;
        IVssRequestContext requestContext1 = requestContext;
        Guid projectId1 = projectId;
        List<Resource> resources = new List<Resource>();
        resources.Add(new Resource()
        {
          Type = resourceType,
          Id = resourceId
        });
        int? pipelineFilter = new int?();
        // ISSUE: explicit non-virtual call
        List<ResourcePipelinePermissions> permissionsForResources = await __nonvirtual (authorizationService2.GetPipelinePermissionsForResources(requestContext1, projectId1, resources, pipelineFilter));
        ResourcePipelinePermissions pipelinePermissions;
        if (!permissionsForResources.Any<ResourcePipelinePermissions>())
          pipelinePermissions = new ResourcePipelinePermissions()
          {
            Resource = new Resource()
            {
              Type = resourceType,
              Id = resourceId
            }
          };
        else
          pipelinePermissions = permissionsForResources.FirstOrDefault<ResourcePipelinePermissions>();
        permissionsForResource = pipelinePermissions;
      }
      return permissionsForResource;
    }

    public async Task<List<ResourcePipelinePermissions>> GetPipelinePermissionsForResources(
      IVssRequestContext requestContext,
      Guid projectId,
      List<Resource> resources,
      int? pipelineFilter)
    {
      List<ResourcePipelinePermissions> permissionsForResources;
      using (requestContext.TraceScope(nameof (PipelineResourceAuthorizationService), nameof (GetPipelinePermissionsForResources)))
      {
        resources.ForEach((Action<Resource>) (resource => this.ValidateAuthorizationUtil(requestContext, projectId, resource.Type, resource.Id, (ResourcePipelinePermissions) null)));
        List<Resource> filteredResources = ChecksUtilities.GetResourcesWithPermission(requestContext, projectId, resources);
        string noPermissionResources = string.Empty;
        resources.ForEach((Action<Resource>) (resource =>
        {
          if (filteredResources.Exists((Predicate<Resource>) (res => res.Type == resource.Type && res.Id == resource.Id)))
            return;
          noPermissionResources += string.Format("ResourceType {0} - ResourceId {1} ,", (object) resource.Type, (object) resource.Id);
        }));
        requestContext.TraceInfo(34002004, nameof (PipelineResourceAuthorizationService), "Resources filtered out because of permission check", (object) noPermissionResources);
        List<ResourcePipelinePermissions> filteredAuthorizations = new List<ResourcePipelinePermissions>();
        List<ResourcePipelinePermissions> pipelinePermissionsList;
        using (PipelineResourceAuthorizationComponent rac = requestContext.CreateComponent<PipelineResourceAuthorizationComponent>())
          pipelinePermissionsList = await rac.GetPipelinePermissionsForResources(projectId, filteredResources);
        if (!pipelineFilter.HasValue || !requestContext.IsFeatureEnabled("Pipelines.Checks.SkipStalePipelinesRemoval"))
        {
          pipelinePermissionsList = await this.RemoveStalePipelines(requestContext, projectId, pipelinePermissionsList);
          requestContext.TraceInfo(34002001, nameof (PipelineResourceAuthorizationService), "Stale Pipelines Removed");
        }
        List<ResourcePipelinePermissions> result;
        if (!pipelineFilter.HasValue)
        {
          result = pipelinePermissionsList;
        }
        else
        {
          foreach (ResourcePipelinePermissions pipelinePermissions in pipelinePermissionsList)
          {
            if (pipelinePermissions.Pipelines.Exists(closure_3 ?? (closure_3 = (Predicate<PipelinePermission>) (pipeline =>
            {
              int id = pipeline.Id;
              int? nullable = pipelineFilter;
              int valueOrDefault = nullable.GetValueOrDefault();
              return id == valueOrDefault & nullable.HasValue;
            }))) || pipelinePermissions.AllPipelines != null)
              filteredAuthorizations.Add(pipelinePermissions);
          }
          result = filteredAuthorizations;
        }
        List<Resource> list = ChecksUtilities.GetDefaultAuthorizedResources(requestContext, projectId, filteredResources, pipelineFilter.GetValueOrDefault()).Intersect<Resource>((IEnumerable<Resource>) resources).ToList<Resource>();
        requestContext.TraceInfo(34002002, nameof (PipelineResourceAuthorizationService), "Count of Resources authorized by default {0} ", (object) list.Count);
        list.ForEach((Action<Resource>) (resourceAuthorizedByDefault =>
        {
          ResourcePipelinePermissions pipelinePermissions1 = result.Find((Predicate<ResourcePipelinePermissions>) (res => res.Resource.Type == resourceAuthorizedByDefault.Type && res.Resource.Id == resourceAuthorizedByDefault.Id));
          Guid userId = requestContext.GetUserId(true);
          Permission permission = new Permission()
          {
            Authorized = true,
            AuthorizedBy = new IdentityRef()
            {
              Id = userId.ToString()
            },
            AuthorizedOn = new DateTime?(DateTime.Now)
          };
          if (pipelinePermissions1 != null)
          {
            pipelinePermissions1.AllPipelines = permission;
          }
          else
          {
            ResourcePipelinePermissions pipelinePermissions2 = new ResourcePipelinePermissions()
            {
              Resource = resourceAuthorizedByDefault,
              AllPipelines = permission
            };
            if (pipelineFilter.HasValue)
            {
              List<PipelinePermission> pipelinePermissionList = new List<PipelinePermission>()
              {
                new PipelinePermission()
                {
                  Authorized = true,
                  Id = pipelineFilter.Value
                }
              };
              pipelinePermissions2.Pipelines = pipelinePermissionList;
            }
            result.Add(pipelinePermissions2);
          }
        }));
        this.PopulateAuthorizedByForPipelines(requestContext, pipelinePermissionsList);
        permissionsForResources = result;
      }
      return permissionsForResources;
    }

    public void AddTelemetry(
      IVssRequestContext requestContext,
      string feature,
      CustomerIntelligenceData telemetryData)
    {
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "PipelinePolicy.Authorization", feature, telemetryData);
    }

    public async Task<ResourcePipelinePermissions> UpdatePipelinePermisionsForResource(
      IVssRequestContext requestContext,
      Guid projectId,
      string resourceType,
      string resourceId,
      ResourcePipelinePermissions resourcePipelinePermissions)
    {
      PipelineResourceAuthorizationService authorizationService = this;
      ResourcePipelinePermissions pipelinePermissions1;
      using (requestContext.TraceScope(nameof (PipelineResourceAuthorizationService), nameof (UpdatePipelinePermisionsForResource)))
      {
        await authorizationService.ValidateAuthorizationRequest(requestContext, projectId, resourceType, resourceId, resourcePipelinePermissions);
        ResourcePermission resourcePermission = ResourcePermission.Use;
        if (resourcePipelinePermissions.AllPipelines != null)
        {
          resourcePermission = ResourcePermission.Admin;
          ResourceType result;
          ResourceTypeNames.TryParse(resourceType, out result);
          if (result != (ResourceType) 0 && resourcePipelinePermissions.AllPipelines.Authorized && !ChecksUtilities.HasProjectLevelAdminPermission(requestContext, projectId, result))
          {
            string message = PipelineAuthorizationResources.MissingProjectAdminPermissionExceptionMessage();
            requestContext.TraceError(34002004, nameof (PipelineResourceAuthorizationService), "For ResourceType {0} ResourceId {1} - {2}.", (object) resourceType, (object) resourceId, (object) message);
            throw new AccessDeniedException(message);
          }
        }
        IVssRequestContext requestContext1 = requestContext;
        Guid projectId1 = projectId;
        List<Resource> resources = new List<Resource>();
        resources.Add(new Resource()
        {
          Type = resourceType,
          Id = resourceId
        });
        int permission = (int) resourcePermission;
        List<Resource> resourcesWithPermission = ChecksUtilities.GetResourcesWithPermission(requestContext1, projectId1, resources, (ResourcePermission) permission);
        if (resourcesWithPermission == null || resourcesWithPermission.Count != 1)
        {
          string message = PipelineAuthorizationResources.MissingUsePermissionExceptionMessage();
          if (resourcePipelinePermissions.AllPipelines != null)
            message = PipelineAuthorizationResources.MissingAdminPermissionExceptionMessage();
          requestContext.TraceError(34002004, nameof (PipelineResourceAuthorizationService), "For ResourceType {0} ResourceId {1} - {2}.", (object) resourceType, (object) resourceId, (object) message);
          throw new AccessDeniedException(message);
        }
        requestContext.TraceInfo(34002000, "ResourceType {0} - ResourceId {1} - PipelinesCount {2}- AllPipelines {3}", resourceType, (object) resourceId, (object) resourcePipelinePermissions.Pipelines.Count, (object) (bool) (resourcePipelinePermissions.AllPipelines == null ? 0 : (resourcePipelinePermissions.AllPipelines.Authorized ? 1 : 0)));
        // ISSUE: explicit non-virtual call
        ResourcePipelinePermissions previousResourcePipelinePermission = await __nonvirtual (authorizationService.GetPipelinePermissionsForResource(requestContext, projectId, resourceType, resourceId));
        ResourcePipelinePermissions pipelinePermissions2;
        using (PipelineResourceAuthorizationComponent rac = requestContext.CreateComponent<PipelineResourceAuthorizationComponent>())
          pipelinePermissions2 = await rac.UpdatePipelinePermisionsForResource(projectId, resourceType, resourceId, requestContext.GetUserId(true), resourcePipelinePermissions);
        ResourcePipelinePermissions pipelinePermissions3 = (await authorizationService.RemoveStalePipelines(requestContext, projectId, new List<ResourcePipelinePermissions>()
        {
          pipelinePermissions2
        })).First<ResourcePipelinePermissions>();
        requestContext.TraceInfo(34002001, nameof (PipelineResourceAuthorizationService), "Stale Pipelines Removed");
        resourcePipelinePermissions.Resource = new Resource()
        {
          Type = resourceType,
          Id = resourceId
        };
        authorizationService.AuditResourcesAndLogTelemetry(requestContext, projectId, new List<ResourcePipelinePermissions>()
        {
          resourcePipelinePermissions
        });
        authorizationService.PopulateAuthorizedByForPipelines(requestContext, new List<ResourcePipelinePermissions>()
        {
          pipelinePermissions3
        });
        if (requestContext.IsFeatureEnabled("Pipelines.Checks.AuthChecksAsyncFlow"))
          authorizationService.NotifyPipelinePermissionsUpdatedIfNeeded(requestContext, projectId, new List<ResourcePipelinePermissions>()
          {
            previousResourcePipelinePermission
          }, new List<ResourcePipelinePermissions>()
          {
            pipelinePermissions3
          });
        pipelinePermissions1 = pipelinePermissions3;
      }
      return pipelinePermissions1;
    }

    public async Task DeletePipelinePermissionsForResource(
      IVssRequestContext requestContext,
      Guid projectId,
      string resourceType,
      string resourceId)
    {
      using (requestContext.TraceScope(nameof (PipelineResourceAuthorizationService), nameof (DeletePipelinePermissionsForResource)))
      {
        await this.ValidateAuthorizationRequest(requestContext, projectId, resourceType, resourceId, (ResourcePipelinePermissions) null);
        if (!requestContext.IsSystemContext)
        {
          List<Resource> resourcesWithPermission = ChecksUtilities.GetResourcesWithPermission(requestContext, projectId, new List<Resource>()
          {
            new Resource() { Type = resourceType, Id = resourceId }
          }, ResourcePermission.Admin);
          if (resourcesWithPermission == null || resourcesWithPermission.Count != 1)
          {
            requestContext.TraceError(34002004, nameof (PipelineResourceAuthorizationService), "For ResourceType {0} ResourceId {1} - {2}.", (object) resourceType, (object) resourceId, (object) PipelineAuthorizationResources.MissingAdminPermissionExceptionMessage());
            throw new AccessDeniedException(PipelineAuthorizationResources.MissingAdminPermissionExceptionMessage());
          }
        }
        using (PipelineResourceAuthorizationComponent rac = requestContext.CreateComponent<PipelineResourceAuthorizationComponent>())
          await rac.DeletePipelinePermissionsForResource(projectId, resourceType, resourceId);
        requestContext.TraceInfo(34002003, nameof (PipelineResourceAuthorizationService), "Deleted all authorization entries for Type - {0} and Id {1} ", (object) resourceType, (object) resourceId);
      }
    }

    public async Task<List<ResourcePipelinePermissions>> UpdatePipelinePermisionsForResources(
      IVssRequestContext requestContext,
      Guid projectId,
      List<ResourcePipelinePermissions> resourcePipelinePermissionsList)
    {
      List<ResourcePipelinePermissions> pipelinePermissionsList1;
      using (requestContext.TraceScope(nameof (PipelineResourceAuthorizationService), nameof (UpdatePipelinePermisionsForResources)))
      {
        await this.ValidateBatchAuthorizationRequest(requestContext, projectId, resourcePipelinePermissionsList);
        List<Resource> resources1 = new List<Resource>();
        List<Resource> resources2 = new List<Resource>();
        List<ResourcePipelinePermissions> filteredList = new List<ResourcePipelinePermissions>();
        foreach (ResourcePipelinePermissions pipelinePermissions in resourcePipelinePermissionsList)
        {
          if (pipelinePermissions.AllPipelines != null)
          {
            resources1.Add(pipelinePermissions.Resource);
            ResourceType result;
            ResourceTypeNames.TryParse(pipelinePermissions.Resource.Type, out result);
            if (result != (ResourceType) 0 && pipelinePermissions.AllPipelines.Authorized && !ChecksUtilities.HasProjectLevelAdminPermission(requestContext, projectId, result))
              resources1.Remove(pipelinePermissions.Resource);
          }
          else
            resources2.Add(pipelinePermissions.Resource);
        }
        List<Resource> authorizedResources = ChecksUtilities.GetResourcesWithPermission(requestContext, projectId, resources1, ResourcePermission.Admin).AddRangeIfRangeNotNull<Resource, List<Resource>>((IEnumerable<Resource>) ChecksUtilities.GetResourcesWithPermission(requestContext, projectId, resources2, ResourcePermission.Use));
        string noPermissionResources = string.Empty;
        resourcePipelinePermissionsList.ForEach((Action<ResourcePipelinePermissions>) (resourceAuthorization =>
        {
          Resource resource = resourceAuthorization.Resource;
          if (authorizedResources.Exists((Predicate<Resource>) (res => res.Type == resource.Type && res.Id == resource.Id)))
            filteredList.Add(resourceAuthorization);
          else
            noPermissionResources += string.Format("ResourceType {0} - ResourceId {1} ,", (object) resource.Type, (object) resource.Id);
        }));
        requestContext.TraceInfo(34002004, nameof (PipelineResourceAuthorizationService), "Resources filtered out because of permission check", (object) noPermissionResources);
        requestContext.TraceInfo(34002000, nameof (PipelineResourceAuthorizationService), "Count of resources whose authorization is to be updated", (object) filteredList.Count);
        List<ResourcePipelinePermissions> previousResourcePipelinePermissions = await this.GetPipelinePermissionsForResources(requestContext, projectId, authorizedResources, new int?());
        List<ResourcePipelinePermissions> authorizedPipelinesForResources;
        try
        {
          using (PipelineResourceAuthorizationComponent rac = requestContext.CreateComponent<PipelineResourceAuthorizationComponent>())
            authorizedPipelinesForResources = await rac.UpdatePipelinePermisionsForResources(projectId, requestContext.GetUserId(true), filteredList);
        }
        catch (Exception ex)
        {
          string str = filteredList == null || !filteredList.Any<ResourcePipelinePermissions>() ? string.Empty : JsonConvert.SerializeObject((object) filteredList);
          requestContext.TraceError(34002009, nameof (PipelineResourceAuthorizationService), "PipelineResourceAuthorizationService::UpdatePipelinePermissionsForResources failed. Parameters: ProjectId: {0}, PermissionsRequest: {1}", (object) projectId, (object) str);
          throw;
        }
        List<ResourcePipelinePermissions> pipelinePermissionsList2 = await this.RemoveStalePipelines(requestContext, projectId, authorizedPipelinesForResources);
        requestContext.TraceInfo(34002001, nameof (PipelineResourceAuthorizationService), "Stale Pipelines Removed");
        this.AuditResourcesAndLogTelemetry(requestContext, projectId, filteredList);
        this.PopulateAuthorizedByForPipelines(requestContext, pipelinePermissionsList2);
        if (requestContext.IsFeatureEnabled("Pipelines.Checks.AuthChecksAsyncFlow"))
          this.NotifyPipelinePermissionsUpdatedIfNeeded(requestContext, projectId, previousResourcePipelinePermissions, pipelinePermissionsList2);
        pipelinePermissionsList1 = pipelinePermissionsList2;
      }
      return pipelinePermissionsList1;
    }

    private bool IsNewPipelinePermissionGranted(
      ResourcePipelinePermissions oldPermissions,
      ResourcePipelinePermissions newPermissions)
    {
      if (oldPermissions == null)
        return newPermissions != null && (newPermissions.AllPipelines != null || newPermissions.Pipelines.Any<PipelinePermission>());
      if (oldPermissions.AllPipelines == null)
      {
        if (newPermissions.AllPipelines != null)
          return true;
        foreach (PipelinePermission pipeline1 in newPermissions.Pipelines)
        {
          PipelinePermission pipeline = pipeline1;
          if (!oldPermissions.Pipelines.Exists((Predicate<PipelinePermission>) (p => p.Id == pipeline.Id)))
            return true;
        }
      }
      return false;
    }

    private void NotifyPipelinePermissionsUpdatedIfNeeded(
      IVssRequestContext requestContext,
      Guid projectId,
      List<ResourcePipelinePermissions> oldPermissions,
      List<ResourcePipelinePermissions> newPermissions)
    {
      IAuthorizationEventPublisherService service = requestContext.GetService<IAuthorizationEventPublisherService>();
      foreach (ResourcePipelinePermissions newPermission in newPermissions)
      {
        ResourcePipelinePermissions newResourcePipelinePermission = newPermission;
        if (this.IsNewPipelinePermissionGranted(oldPermissions.Find((Predicate<ResourcePipelinePermissions>) (r => r.Resource.Equals(newResourcePipelinePermission.Resource))), newResourcePipelinePermission))
          service.OnAuthorizationCompleted(requestContext, projectId, newResourcePipelinePermission);
      }
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private void AuditResourcesAndLogTelemetry(
      IVssRequestContext requestContext,
      Guid projectId,
      List<ResourcePipelinePermissions> resourcePipelinePermissionsList)
    {
      CustomerIntelligenceData telemetryData1 = new CustomerIntelligenceData();
      CustomerIntelligenceData telemetryData2 = new CustomerIntelligenceData();
      List<Dictionary<string, object>> dictionaryList1 = new List<Dictionary<string, object>>();
      List<Dictionary<string, object>> dictionaryList2 = new List<Dictionary<string, object>>();
      List<string> values1 = new List<string>();
      List<string> values2 = new List<string>();
      foreach (ResourcePipelinePermissions pipelinePermissions in resourcePipelinePermissionsList)
      {
        List<int> authorizedPipelines = new List<int>();
        List<int> unauthorizedPipelines = new List<int>();
        Resource resource = pipelinePermissions.Resource;
        pipelinePermissions.Pipelines.ForEach((Action<PipelinePermission>) (pipeline =>
        {
          if (pipeline.Authorized)
            authorizedPipelines.Add(pipeline.Id);
          else
            unauthorizedPipelines.Add(pipeline.Id);
          this.AuditResourceAuthorizations(requestContext, projectId, new int?(pipeline.Id), resource.Type, resource.Id, pipeline.Authorized);
        }));
        dictionaryList1.Add(new Dictionary<string, object>()
        {
          ["resourceType"] = (object) resource.Type,
          ["resourceId"] = (object) resource.Id,
          ["definitionsUpdated"] = (object) authorizedPipelines
        });
        dictionaryList2.Add(new Dictionary<string, object>()
        {
          ["resourceType"] = (object) resource.Type,
          ["resourceId"] = (object) resource.Id,
          ["definitionsUpdated"] = (object) unauthorizedPipelines
        });
        if (pipelinePermissions.AllPipelines != null)
        {
          this.AuditResourceAuthorizations(requestContext, projectId, new int?(), resource.Type.ToString(), resource.Id, pipelinePermissions.AllPipelines.Authorized);
          if (pipelinePermissions.AllPipelines.Authorized)
            values1.Add(string.Format("{0}.{1} ", (object) resource.Type, (object) resource.Id));
          else
            values2.Add(string.Format("{0}.{1} ", (object) resource.Type, (object) resource.Id));
        }
      }
      telemetryData1.Add("Added", (object) dictionaryList1);
      telemetryData1.Add("Removed", (object) dictionaryList2);
      telemetryData2.Add("NewStateIsRestricted", values1);
      telemetryData2.Add("NewStateIsUnRestricted", values2);
      this.AddTelemetry(requestContext, "PipelineResourceAuthorizationService.IsAuthorizedForAllDefinitions", telemetryData2);
      this.AddTelemetry(requestContext, "PipelineResourceAuthorizationService.UpdateAuthorize", telemetryData1);
    }

    private async Task ValidateDefinition(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId)
    {
      await requestContext.GetService<IDefinitionExtensionService>().GetDefinitionPlugin().ValidateDefinitionAsync(requestContext, projectId, definitionId);
    }

    private async Task ValidateDefinitions(
      IVssRequestContext requestContext,
      Guid projectId,
      List<int> definitionIds)
    {
      await requestContext.GetService<IDefinitionExtensionService>().GetDefinitionPlugin().ValidateDefinitionsAsync(requestContext, projectId, definitionIds);
    }

    private async Task<List<ResourcePipelinePermissions>> RemoveStalePipelines(
      IVssRequestContext requestContext,
      Guid projectId,
      List<ResourcePipelinePermissions> authorizedPipelinesForResources)
    {
      requestContext.TraceInfo(34002010, nameof (PipelineResourceAuthorizationService), "Entering RemoveStalePipelines method");
      HashSet<int> pipelineIdCollection = new HashSet<int>();
      authorizedPipelinesForResources.ForEach((Action<ResourcePipelinePermissions>) (authorizedPipelinesForResource => pipelineIdCollection.AddRange<int, HashSet<int>>(authorizedPipelinesForResource.Pipelines.Select<PipelinePermission, int>((Func<PipelinePermission, int>) (pipeline => pipeline.Id)))));
      requestContext.TraceInfo(34002010, nameof (PipelineResourceAuthorizationService), "Collected pipeline Ids of authorized pipelines : size {0}", (object) pipelineIdCollection.Count<int>());
      IDefinitionPlugin definitionPlugin = requestContext.GetService<IDefinitionExtensionService>().GetDefinitionPlugin();
      requestContext.TraceInfo(34002010, nameof (PipelineResourceAuthorizationService), "Filtering valid definition Ids");
      IVssRequestContext requestContext1 = requestContext;
      Guid projectId1 = projectId;
      List<int> list = pipelineIdCollection.ToList<int>();
      List<int> validPipelineIds = await definitionPlugin.FilterValidDefinitionIdsAsync(requestContext1, projectId1, list);
      requestContext.TraceInfo(34002010, nameof (PipelineResourceAuthorizationService), "Filtered valid definition Ids");
      authorizedPipelinesForResources.ForEach((Action<ResourcePipelinePermissions>) (authorizedPipelinesForResource => authorizedPipelinesForResource.Pipelines.RemoveAll((Predicate<PipelinePermission>) (pipeline => !validPipelineIds.Contains(pipeline.Id)))));
      requestContext.TraceInfo(34002010, nameof (PipelineResourceAuthorizationService), "Leaving RemoveStalePipelines method");
      return authorizedPipelinesForResources;
    }

    private async Task ValidateAuthorizationRequest(
      IVssRequestContext requestContext,
      Guid projectId,
      string resourceType,
      string resourceId,
      ResourcePipelinePermissions resourcePipelinePermissions)
    {
      this.ValidateAuthorizationUtil(requestContext, projectId, resourceType, resourceId, resourcePipelinePermissions);
      if (resourcePipelinePermissions == null || !resourcePipelinePermissions.Pipelines.Any<PipelinePermission>())
        return;
      Resource resource = resourcePipelinePermissions.Resource ?? new Resource();
      if (resource.Type != null && resource.Type != resourceType)
      {
        requestContext.TraceError(34002006, nameof (PipelineResourceAuthorizationService), "Resource type in body doesn't match resource type in url");
        throw new InvalidOperationException(PipelineAuthorizationResources.UrlBodyResourceTypeMismatch());
      }
      if (!string.IsNullOrEmpty(resource.Id) && resource.Id != resourceId)
      {
        requestContext.TraceError(34002006, nameof (PipelineResourceAuthorizationService), "Resource id in body doesn't match resource id in url");
        throw new InvalidOperationException(PipelineAuthorizationResources.UrlBodyResourceIdMismatch());
      }
      await this.ValidateDefinitions(requestContext, projectId, resourcePipelinePermissions.Pipelines.Select<PipelinePermission, int>((Func<PipelinePermission, int>) (pipeline => pipeline.Id)).ToList<int>());
    }

    private async Task ValidateBatchAuthorizationRequest(
      IVssRequestContext requestContext,
      Guid projectId,
      List<ResourcePipelinePermissions> resourcePipelinePermissionsList)
    {
      if (resourcePipelinePermissionsList == null)
      {
        requestContext.TraceError(34002006, nameof (PipelineResourceAuthorizationService), "Permission Request is Null");
        throw new InvalidOperationException(PipelineAuthorizationResources.NullPermissionsRequest());
      }
      List<int> definitionIds = new List<int>();
      resourcePipelinePermissionsList.ForEach((Action<ResourcePipelinePermissions>) (resourcePipelinePermissions =>
      {
        if (resourcePipelinePermissions.Resource == null)
        {
          requestContext.TraceError(34002006, nameof (PipelineResourceAuthorizationService), "Resource is Null");
          throw new InvalidOperationException(PipelineAuthorizationResources.MissingResourceObjectInBatchRequest());
        }
        Resource resource = resourcePipelinePermissions.Resource;
        this.ValidateAuthorizationUtil(requestContext, projectId, resource.Type, resource.Id, resourcePipelinePermissions);
        definitionIds.AddRange((IEnumerable<int>) resourcePipelinePermissions.Pipelines.Select<PipelinePermission, int>((Func<PipelinePermission, int>) (pipeline => pipeline.Id)).ToList<int>());
      }));
      await this.ValidateDefinitions(requestContext, projectId, definitionIds.Distinct<int>().ToList<int>());
    }

    private void ValidateAuthorizationUtil(
      IVssRequestContext requestContext,
      Guid projectId,
      string resourceType,
      string resourceId,
      ResourcePipelinePermissions resourcePipelinePermissions)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "PipelinePolicy.Authorization");
      if (string.IsNullOrEmpty(resourceId) || string.IsNullOrEmpty(resourceType))
      {
        requestContext.TraceError(34002006, nameof (PipelineResourceAuthorizationService), "ResourceType/ResourceId is Null/Invalid ");
        throw new InvalidOperationException(PipelineAuthorizationResources.ResourcesQueryRequiresTypeAndId());
      }
      ResourceType result;
      ResourceTypeNames.TryParse(resourceType, out result);
      if (result == (ResourceType) 0)
      {
        requestContext.TraceError(34002006, nameof (PipelineResourceAuthorizationService), "ResourceType is Invalid for resource Id {0} ", (object) resourceId);
        throw new InvalidResourceType(PipelineAuthorizationResources.InvalidResourceType());
      }
      if (resourceId.Length > 100)
      {
        requestContext.TraceError(34002006, nameof (PipelineResourceAuthorizationService), "ResourceId exceeded max Length allowed for ResourceId {0} ", (object) resourceId);
        throw new InvalidOperationException(PipelineAuthorizationResources.ResourceIdLengthOverflow((object) 100.ToString()));
      }
      if (resourcePipelinePermissions != null && resourcePipelinePermissions.Pipelines.Any<PipelinePermission>() && resourcePipelinePermissions.Pipelines.Count != resourcePipelinePermissions.Pipelines.Select<PipelinePermission, int>((Func<PipelinePermission, int>) (pipeline => pipeline.Id)).Distinct<int>().Count<int>())
      {
        requestContext.TraceError(34002006, nameof (PipelineResourceAuthorizationService), "Duplicate Pipeline entries for resource Id {0} - Duplicate Pipeline Ids {1} ", (object) resourceId, (object) resourcePipelinePermissions.Pipelines.GroupBy<PipelinePermission, int>((Func<PipelinePermission, int>) (pipeline => pipeline.Id)).SelectMany<IGrouping<int, PipelinePermission>, PipelinePermission>((Func<IGrouping<int, PipelinePermission>, IEnumerable<PipelinePermission>>) (grp => grp.Skip<PipelinePermission>(1).Take<PipelinePermission>(1))).ToString());
        throw new InvalidOperationException(PipelineAuthorizationResources.DuplicateDefinitionIds());
      }
    }

    private void AuditResourceAuthorizations(
      IVssRequestContext requestContext,
      Guid projectId,
      int? definitionId,
      string resourceType,
      string resourceId,
      bool authorized)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>()
      {
        {
          "ResourceId",
          (object) resourceId
        },
        {
          "ResourceType",
          (object) resourceType
        }
      };
      if (definitionId.HasValue)
      {
        dictionary.Add("PipelineId", (object) definitionId.ToString());
        if (authorized)
        {
          IVssRequestContext requestContext1 = requestContext;
          Dictionary<string, object> data = dictionary;
          Guid guid = projectId;
          Guid targetHostId = new Guid();
          Guid projectId1 = guid;
          requestContext1.LogAuditEvent("Pipelines.ResourceAuthorizedForPipeline", data, targetHostId, projectId1);
        }
        else
        {
          IVssRequestContext requestContext2 = requestContext;
          Dictionary<string, object> data = dictionary;
          Guid guid = projectId;
          Guid targetHostId = new Guid();
          Guid projectId2 = guid;
          requestContext2.LogAuditEvent("Pipelines.ResourceUnauthorizedForPipeline", data, targetHostId, projectId2);
        }
      }
      else if (authorized)
      {
        IVssRequestContext requestContext3 = requestContext;
        Dictionary<string, object> data = dictionary;
        Guid guid = projectId;
        Guid targetHostId = new Guid();
        Guid projectId3 = guid;
        requestContext3.LogAuditEvent("Pipelines.ResourceAuthorizedForProject", data, targetHostId, projectId3);
      }
      else
      {
        IVssRequestContext requestContext4 = requestContext;
        Dictionary<string, object> data = dictionary;
        Guid guid = projectId;
        Guid targetHostId = new Guid();
        Guid projectId4 = guid;
        requestContext4.LogAuditEvent("Pipelines.ResourceUnauthorizedForProject", data, targetHostId, projectId4);
      }
    }

    private void PopulateAuthorizedByForPipelines(
      IVssRequestContext requestContext,
      List<ResourcePipelinePermissions> resourcePipelinePermissionsList)
    {
      List<string> authorizedByIds = new List<string>();
      resourcePipelinePermissionsList.ForEach((Action<ResourcePipelinePermissions>) (resourcePipelinePermissions =>
      {
        authorizedByIds.AddRange(resourcePipelinePermissions.Pipelines.Select<PipelinePermission, string>((Func<PipelinePermission, string>) (pipeline => pipeline.AuthorizedBy.Id)));
        if (resourcePipelinePermissions.AllPipelines == null)
          return;
        authorizedByIds.Add(resourcePipelinePermissions.AllPipelines.AuthorizedBy.Id);
      }));
      if (authorizedByIds == null || !authorizedByIds.Any<string>())
        return;
      IDictionary<string, IdentityRef> identities = requestContext.GetService<IdentityService>().QueryIdentities(requestContext, (IList<string>) authorizedByIds.Distinct<string>().ToList<string>());
      resourcePipelinePermissionsList.ForEach((Action<ResourcePipelinePermissions>) (resourcePipelinePermissions =>
      {
        resourcePipelinePermissions.Pipelines.ForEach((Action<PipelinePermission>) (pipeline =>
        {
          IdentityRef identityRef;
          if (!identities.TryGetValue(pipeline.AuthorizedBy.Id, out identityRef))
            return;
          pipeline.AuthorizedBy = identityRef;
        }));
        IdentityRef identityRef1;
        if (resourcePipelinePermissions.AllPipelines == null || !identities.TryGetValue(resourcePipelinePermissions.AllPipelines.AuthorizedBy.Id, out identityRef1))
          return;
        resourcePipelinePermissions.AllPipelines.AuthorizedBy = identityRef1;
      }));
    }
  }
}
