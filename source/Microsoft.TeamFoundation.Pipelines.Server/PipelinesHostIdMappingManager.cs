// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.PipelinesHostIdMappingManager
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public static class PipelinesHostIdMappingManager
  {
    private const string c_layer = "PipelinesHostIdMappingManager";

    public static Guid? GetHostId(
      IVssRequestContext requestContext,
      string providerId,
      HostIdMappingData mappingData,
      bool useExactMatching = false)
    {
      return requestContext.GetService<IHostIdMappingService>().GetHostId(requestContext, providerId, mappingData, useExactMatching);
    }

    public static void AddRoute(
      IVssRequestContext requestContext,
      IPipelineSourceProvider provider,
      string installationId)
    {
      Guid guid = requestContext.GetService<IHostIdMappingService>().AddRoute(requestContext, (IHostIdMappingProviderData) provider, installationId);
      if (provider.ExternalApp == null)
        return;
      requestContext.GetService<IDistributedTaskInstalledAppService>().AddInstallation(requestContext, provider.ExternalApp.AppId, installationId, new DistributedTaskInstalledAppData()
      {
        BillingHostId = guid,
        Data = provider.ExternalApp.GetInstallationDetails(requestContext, installationId)
      });
    }

    public static void RemoveRoute(
      IVssRequestContext requestContext,
      IPipelineSourceProvider provider,
      string installationId)
    {
      requestContext.GetService<IHostIdMappingService>().RemoveRoute(requestContext, (IHostIdMappingProviderData) provider, installationId);
      if (provider.ExternalApp == null)
        return;
      requestContext.TraceAlways(TracePoints.Events.BuildDefinitionChangedEventListenerHandleEvent, TraceLevel.Info, nameof (RemoveRoute), nameof (PipelinesHostIdMappingManager), "Removing installation for installation Id: " + installationId);
      requestContext.GetService<IDistributedTaskInstalledAppService>().RemoveInstallation(requestContext, provider.ExternalApp.AppId, installationId);
    }

    public static bool AdjustHostMappingForRepository(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      IPipelineSourceProvider provider)
    {
      bool flag = false;
      BuildDefinition previousDefinitionRevision;
      if (BuildServiceHelper.TryGetPreviousDefinitionRevision(requestContext, definition, out previousDefinitionRevision))
      {
        if (BuildServiceHelper.RepositoryChanged(requestContext, definition, previousDefinitionRevision) | BuildServiceHelper.ServiceEndpointChanged(requestContext, definition, previousDefinitionRevision) || !BuildServiceHelper.IsTriggerableDefinition(definition) || !provider.ConnectionCreator.IsProviderDefinition(requestContext, definition))
        {
          PipelinesHostIdMappingManager.RemoveHostMappingForRepository(requestContext, previousDefinitionRevision, provider);
          flag = true;
        }
        if (BuildServiceHelper.IsTriggerableDefinition(definition) && provider.ConnectionCreator.IsProviderDefinition(requestContext, definition))
        {
          PipelinesHostIdMappingManager.AddHostMappingForRepository(requestContext, definition, provider);
          flag = true;
        }
      }
      return flag;
    }

    public static bool AdjustHostMappingForResourceRepositories(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      List<BuildRepository> resourceRepos,
      IPipelineSourceProvider provider)
    {
      bool flag = false;
      if (BuildServiceHelper.IsTriggerableDefinition(definition))
      {
        if (resourceRepos.Where<BuildRepository>((Func<BuildRepository, bool>) (repo => repo.Id == definition.Repository.Id)) != null)
          resourceRepos.Add(definition.Repository);
        PipelinesHostIdMappingManager.RemoveUnneededResourceRepositoryMappings(requestContext, provider, definition, resourceRepos);
        foreach (BuildRepository resourceRepo in resourceRepos)
        {
          if (resourceRepo.HasTriggers() && provider.ConnectionCreator.IsProviderRepository(requestContext, resourceRepo, definition.ProjectId))
            PipelinesHostIdMappingManager.AddHostMappingForRepository(requestContext, definition, resourceRepo, provider);
        }
        flag = true;
      }
      return flag;
    }

    public static void AddHostMappingForRepository(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      IPipelineSourceProvider provider)
    {
      PipelinesHostIdMappingManager.AddHostMappingForRepository(requestContext, definition, definition.Repository, provider);
    }

    public static void AddHostMappingForRepository(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      BuildRepository repository,
      IPipelineSourceProvider provider)
    {
      foreach (IPipelineHostIdMappingRouter router in (IEnumerable<IHostIdMappingRouter>) provider.Routers)
      {
        HostIdMappingData mappingData;
        Guid conflictingHostId;
        if (router.TryExtractMappingDataForSingleRepository(requestContext, definition, repository, out mappingData) && !PipelinesHostIdMappingManager.TryAddHostIdMapping(requestContext, (IHostIdMappingRouter) router, provider.ProviderId, mappingData.Id, mappingData.Qualifier, out conflictingHostId))
          requestContext.TraceAlways(TracePoints.Events.BuildDefinitionChangedEventListenerHandleEvent, TraceLevel.Info, TracePoints.Area, nameof (PipelinesHostIdMappingManager), "HostIdMapping already exists. This host id: {0}; conflicting host id: {1}.", (object) requestContext.ServiceHost.InstanceId, (object) conflictingHostId);
      }
    }

    public static void RemoveHostMappingForRepository(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      IPipelineSourceProvider provider)
    {
      PipelinesHostIdMappingManager.RemoveHostMappingForRepository(requestContext, definition, definition.Repository, provider);
    }

    public static void RemoveHostMappingForRepository(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      BuildRepository repository,
      IPipelineSourceProvider provider)
    {
      foreach (IPipelineHostIdMappingRouter router in (IEnumerable<IHostIdMappingRouter>) provider.Routers)
      {
        HostIdMappingData mappingData;
        if (router.TryExtractMappingDataForSingleRepository(requestContext, definition, repository, out mappingData) && repository != null && repository.Id != null)
        {
          if (PipelinesHostIdMappingManager.OtherPipelinesExistThatNeedTheseMappings(requestContext, provider, definition, repository.Id))
          {
            requestContext.TraceAlways(TracePoints.Events.BuildDefinitionChangedEventListenerHandleEvent, TraceLevel.Info, nameof (RemoveHostMappingForRepository), nameof (PipelinesHostIdMappingManager), "Not removing mapping for collection {0}. Other definitions found for repo: {1}", (object) requestContext.ServiceHost.Name, (object) repository.Name);
          }
          else
          {
            requestContext.TraceAlways(TracePoints.Events.BuildDefinitionChangedEventListenerHandleEvent, TraceLevel.Info, nameof (RemoveHostMappingForRepository), nameof (PipelinesHostIdMappingManager), "Removing mapping for collection {0}. No other definitions found for repo: {1}", (object) requestContext.ServiceHost.Name, (object) repository.Name);
            PipelinesHostIdMappingManager.TryRemoveHostIdMapping(requestContext, (IHostIdMappingRouter) router, provider.ProviderId, mappingData, requestContext.ServiceHost.InstanceId);
          }
        }
      }
    }

    public static void RemoveUnneededResourceRepositoryMappings(
      IVssRequestContext requestContext,
      IPipelineSourceProvider provider,
      BuildDefinition definition,
      List<BuildRepository> resourceRepos)
    {
      // ISSUE: explicit non-virtual call
      if (definition == null || __nonvirtual (definition.Repository)?.Id == null)
        return;
      IHostIdMappingService service = requestContext.GetService<IHostIdMappingService>();
      foreach (IPipelineHostIdMappingRouter router in (IEnumerable<IHostIdMappingRouter>) provider.Routers)
      {
        HostIdMappingData mappingData;
        if (router.TryExtractMappingData(requestContext, definition, out mappingData))
        {
          List<HostIdMappingData> mappingDataList;
          router.TryExtractMappingDataForRepositories(requestContext, definition, resourceRepos, out mappingDataList);
          foreach (string unneededMapping in service.GetUnneededMappings(requestContext, provider.ProviderId, mappingData.PropertyName, mappingDataList))
          {
            string repositoryId;
            if (router.TryParseRepoIdFromRoutingKey(unneededMapping, out repositoryId) && PipelinesHostIdMappingManager.OtherPipelinesExistThatNeedTheseMappings(requestContext, provider, definition, repositoryId))
            {
              requestContext.TraceAlways(TracePoints.Events.BuildDefinitionChangedEventListenerHandleEvent, TraceLevel.Info, nameof (RemoveUnneededResourceRepositoryMappings), nameof (PipelinesHostIdMappingManager), "Not removing mapping {0} for collection {1}. Other definitions found for repo: {2}", (object) unneededMapping, (object) requestContext.ServiceHost.Name, (object) definition.Repository.Name);
            }
            else
            {
              requestContext.TraceAlways(TracePoints.Events.BuildDefinitionChangedEventListenerHandleEvent, TraceLevel.Info, nameof (RemoveUnneededResourceRepositoryMappings), nameof (PipelinesHostIdMappingManager), "Removing mapping {0} for collection {1}. No other definitions found for repo: {2}", (object) unneededMapping, (object) requestContext.ServiceHost.Name, (object) definition.Repository.Name);
              service.RemoveHostIdMappingViaKey(requestContext, provider.ProviderId, mappingData.PropertyName, unneededMapping);
            }
          }
        }
      }
    }

    public static bool TryAddHostIdMapping(
      IVssRequestContext requestContext,
      IHostIdMappingRouter mappingRouter,
      string providerId,
      string installationId,
      string qualifier,
      out Guid conflictingHostId)
    {
      return requestContext.GetService<IHostIdMappingService>().TryAddHostIdMapping(requestContext, mappingRouter, providerId, installationId, qualifier, out conflictingHostId);
    }

    public static bool TryRemoveHostIdMapping(
      IVssRequestContext requestContext,
      IHostIdMappingRouter mappingRouter,
      string providerId,
      HostIdMappingData mappingData,
      Guid expectedHostId)
    {
      return requestContext.GetService<IHostIdMappingService>().TryRemoveHostIdMapping(requestContext, mappingRouter, providerId, mappingData, expectedHostId);
    }

    public static void AddHostIdMapping(
      IVssRequestContext requestContext,
      string providerId,
      HostIdMappingData mappingData,
      Guid hostId)
    {
      requestContext.GetService<IHostIdMappingService>().AddHostIdMapping(requestContext, providerId, mappingData, hostId);
    }

    public static void RemoveHostIdMapping(
      IVssRequestContext requestContext,
      string providerId,
      HostIdMappingData mappingData)
    {
      requestContext.GetService<IHostIdMappingService>().RemoveHostIdMapping(requestContext, providerId, mappingData);
    }

    internal static void RemoveHostIdMappings(
      IVssRequestContext requestContext,
      string providerId,
      HostIdMappingData mappingData)
    {
      requestContext.GetService<IHostIdMappingService>().RemoveHostIdMappings(requestContext, providerId, mappingData);
    }

    private static bool OtherPipelinesExistThatNeedTheseMappings(
      IVssRequestContext requestContext,
      IPipelineSourceProvider provider,
      BuildDefinition definition,
      string repositoryId)
    {
      foreach (BuildDefinition repositoryDefinitionsFor in BuildServiceHelper.GetTriggerableRepositoryDefinitionsForCollection(requestContext, provider, repositoryId))
      {
        if ((!(repositoryDefinitionsFor.ProjectId == definition.ProjectId) || repositoryDefinitionsFor.Id != definition.Id) && provider.ConnectionCreator.IsProviderDefinition(requestContext, repositoryDefinitionsFor))
        {
          requestContext.TraceInfo(TracePoints.Events.BuildDefinitionChangedEventListenerHandleEvent, nameof (PipelinesHostIdMappingManager), "Found another pipeline for repo '{0}'. Collection: {1}, project: {2}, pipeline.Id: {3}", (object) repositoryId, (object) requestContext.ServiceHost.InstanceId, (object) repositoryDefinitionsFor.ProjectId, (object) repositoryDefinitionsFor.Id);
          return true;
        }
      }
      return false;
    }
  }
}
