// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Server.LocationDataProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;

namespace Microsoft.VisualStudio.Services.Location.Server
{
  internal abstract class LocationDataProvider : 
    IInternalLocationDataProvider,
    ILocationDataProvider,
    IDisposable
  {
    internal const string c_allowRemoteServiceDefinitionsWithRelativePath = "VisualStudio.FrameworkService.LocationService.AllowRemoteServiceDefinitionsWithRelativePath";
    private ILocationDataCache<string> m_locationCache;
    private IVssServiceHost m_serviceHost;
    private SemaphoreSlim m_loadSemaphore;
    private static readonly string s_area = "LocationService";
    private static readonly string s_layer = "LocalLocationDataProvider";

    internal LocationDataProvider()
    {
    }

    public LocationDataProvider(
      IVssRequestContext requestContext,
      ILocationDataCache<string> locationCache)
    {
      this.m_serviceHost = requestContext.ServiceHost;
      this.m_locationCache = locationCache;
      this.m_loadSemaphore = new SemaphoreSlim(requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, in FrameworkServerConstants.LocationServiceLoadSemaphore, 16));
    }

    public abstract Guid HostId { get; }

    public abstract Guid InstanceType { get; }

    protected abstract string CacheKey { get; }

    protected ILocationDataCache<string> LocationCache => this.m_locationCache;

    public abstract void SaveServiceDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<ServiceDefinition> serviceDefinitions);

    public abstract void RemoveServiceDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<ServiceDefinition> serviceDefinitions);

    public virtual ServiceDefinition FindServiceDefinition(
      IVssRequestContext requestContext,
      string serviceType,
      Guid identifier)
    {
      this.ValidateRequestContext(requestContext);
      LocationData locationData = this.GetLocationData(requestContext);
      ServiceDefinition serviceDefinition1 = locationData.FindServiceDefinition(serviceType, identifier);
      if (serviceDefinition1 != null)
        return LocationDataProvider.Clone(requestContext, serviceDefinition1);
      ServiceDefinition serviceDefinition2 = this.GetInheritedLocationData(requestContext).FindServiceDefinition(serviceType, identifier);
      return serviceDefinition2 != null ? this.PostProcessAndCloneInheritedServiceDefinition(requestContext, serviceDefinition2, locationData) : (ServiceDefinition) null;
    }

    public ServiceDefinition FindServiceDefinition(
      IVssRequestContext requestContext,
      string serviceType,
      string toolId)
    {
      this.ValidateRequestContext(requestContext);
      LocationData locationData = this.GetLocationData(requestContext);
      ServiceDefinition definitionByTypeTool = locationData.FindServiceDefinitionByTypeTool(serviceType, toolId);
      if (definitionByTypeTool != null)
        return LocationDataProvider.Clone(requestContext, definitionByTypeTool);
      return this.PostProcessAndCloneInheritedServiceDefinition(requestContext, this.GetInheritedLocationData(requestContext).FindServiceDefinitionByTypeTool(serviceType, toolId) ?? throw new TeamFoundationLocationServiceException(TFCommonResources.InvalidFindServiceByTypeAndToolId((object) serviceType, (object) toolId, (object) "0")), locationData);
    }

    public IEnumerable<ServiceDefinition> FindServiceDefinitions(
      IVssRequestContext requestContext,
      string serviceType)
    {
      this.ValidateRequestContext(requestContext);
      LocationData locationData = this.GetLocationData(requestContext);
      LocationData inheritedLocationData = this.GetInheritedLocationData(requestContext);
      return LocationDataProvider.Clone(requestContext, locationData.FindServiceDefinitionsByType(serviceType)).Union<ServiceDefinition>((IEnumerable<ServiceDefinition>) this.PostProcessAndCloneInheritedServiceDefinitions(requestContext, inheritedLocationData.FindServiceDefinitionsByType(serviceType), locationData), (IEqualityComparer<ServiceDefinition>) ServiceDefinitionComparer.Instance);
    }

    public IEnumerable<ServiceDefinition> FindServiceDefinitionsByToolId(
      IVssRequestContext requestContext,
      string toolId)
    {
      this.ValidateRequestContext(requestContext);
      LocationData locationData = this.GetLocationData(requestContext);
      LocationData inheritedLocationData = this.GetInheritedLocationData(requestContext);
      return LocationDataProvider.Clone(requestContext, locationData.FindServiceDefinitionsByTool(toolId)).Union<ServiceDefinition>((IEnumerable<ServiceDefinition>) this.PostProcessAndCloneInheritedServiceDefinitions(requestContext, inheritedLocationData.FindServiceDefinitionsByTool(toolId), locationData), (IEqualityComparer<ServiceDefinition>) ServiceDefinitionComparer.Instance);
    }

    public abstract ServiceDefinition FindServiceDefinitionWithFaultIn(
      IVssRequestContext requestContext,
      string serviceType,
      Guid identifier,
      bool previewFaultIn);

    public IEnumerable<ServiceDefinition> FindNonInheritedDefinitions(
      IVssRequestContext requestContext)
    {
      this.ValidateRequestContext(requestContext);
      LocationData locationData = this.GetLocationData(requestContext);
      return (IEnumerable<ServiceDefinition>) LocationDataProvider.Clone(requestContext, locationData.GetAllServiceDefinitions());
    }

    public string LocationForAccessMapping(
      IVssRequestContext requestContext,
      string serviceType,
      Guid serviceIdentifier,
      AccessMapping accessMapping)
    {
      ArgumentUtility.CheckForNull<AccessMapping>(accessMapping, nameof (accessMapping));
      ServiceDefinition serviceDefinition = this.FindServiceDefinition(requestContext, serviceType, serviceIdentifier);
      if (serviceDefinition == null)
        throw new ServiceDefinitionDoesNotExistException(TFCommonResources.ServiceDefinitionDoesNotExist((object) serviceType, (object) serviceIdentifier));
      LocationData locationData = this.GetLocationData(requestContext);
      return this.LocationForAccessMappingInternal(requestContext, serviceDefinition, accessMapping, locationData.WebAppRelativeDirectory);
    }

    public string LocationForAccessMapping(
      IVssRequestContext requestContext,
      string serviceType,
      string toolId,
      AccessMapping accessMapping)
    {
      this.ValidateRequestContext(requestContext);
      ArgumentUtility.CheckForNull<AccessMapping>(accessMapping, nameof (accessMapping));
      ServiceDefinition serviceDefinition = this.FindServiceDefinition(requestContext, serviceType, toolId);
      LocationData locationData = this.GetLocationData(requestContext);
      return this.LocationForAccessMappingInternal(requestContext, serviceDefinition, accessMapping, locationData.WebAppRelativeDirectory);
    }

    public string LocationForAccessMapping(
      IVssRequestContext requestContext,
      string relativePath,
      Microsoft.VisualStudio.Services.Location.RelativeToSetting relativeToSetting,
      AccessMapping accessMapping)
    {
      LocationData locationData = this.GetLocationData(requestContext);
      return this.LocationForAccessMappingInternal(requestContext, relativePath, relativeToSetting, accessMapping, locationData.WebAppRelativeDirectory);
    }

    public string LocationForAccessMapping(
      IVssRequestContext requestContext,
      ServiceDefinition serviceDefinition,
      AccessMapping accessMapping)
    {
      ArgumentUtility.CheckForNull<ServiceDefinition>(serviceDefinition, nameof (serviceDefinition));
      ArgumentUtility.CheckForNull<AccessMapping>(accessMapping, nameof (accessMapping));
      LocationData locationData = this.GetLocationData(requestContext);
      return this.LocationForAccessMappingInternal(requestContext, serviceDefinition, accessMapping, locationData.WebAppRelativeDirectory);
    }

    private string LocationForAccessMappingInternal(
      IVssRequestContext requestContext,
      ServiceDefinition serviceDefinition,
      AccessMapping accessMapping,
      string webAppRelativeDirectory)
    {
      if (serviceDefinition.ServiceOwner != accessMapping.ServiceOwner)
        throw new ArgumentException(FrameworkResources.LocationServiceOwnersDoNotMatch((object) "serviceDefinition.ServiceOwner", (object) serviceDefinition.ServiceOwner, (object) "accessMapping.ServiceOwner", (object) accessMapping.ServiceOwner, (object) "serviceDefinition.Identifier", (object) serviceDefinition.Identifier, (object) "serviceDefinition.ServiceType", (object) serviceDefinition.ServiceType));
      if (serviceDefinition.RelativeToSetting != Microsoft.VisualStudio.Services.Location.RelativeToSetting.FullyQualified)
        return this.LocationForAccessMappingInternal(requestContext, serviceDefinition.RelativePath, serviceDefinition.RelativeToSetting, accessMapping, webAppRelativeDirectory);
      return serviceDefinition.GetLocationMapping(accessMapping)?.Location;
    }

    private string LocationForAccessMappingInternal(
      IVssRequestContext requestContext,
      string relativePath,
      Microsoft.VisualStudio.Services.Location.RelativeToSetting relativeToSetting,
      AccessMapping accessMapping,
      string webAppRelativeDirectory)
    {
      if (string.IsNullOrEmpty(accessMapping.AccessPoint))
        throw new InvalidAccessPointException(TFCommonResources.InvalidAccessMappingLocationServiceUrl());
      if (accessMapping.VirtualDirectory != null)
        webAppRelativeDirectory = VirtualPathUtility.AppendTrailingSlash(accessMapping.VirtualDirectory);
      Uri uri = new Uri(accessMapping.AccessPoint);
      string path1 = string.Empty;
      switch (relativeToSetting)
      {
        case Microsoft.VisualStudio.Services.Location.RelativeToSetting.Context:
          path1 = TFCommonUtil.CombinePaths(uri.AbsoluteUri, webAppRelativeDirectory);
          break;
        case Microsoft.VisualStudio.Services.Location.RelativeToSetting.WebApplication:
          path1 = accessMapping.AccessPoint;
          break;
      }
      return TFCommonUtil.CombinePaths(path1, relativePath);
    }

    public AccessMapping ConfigureAccessMapping(
      IVssRequestContext requestContext,
      AccessMapping accessMapping,
      bool makeDefault)
    {
      return this.ConfigureAccessMapping(requestContext, accessMapping, makeDefault, false);
    }

    public virtual AccessMapping ConfigureAccessMapping(
      IVssRequestContext requestContext,
      AccessMapping accessMapping,
      bool makeDefault,
      bool allowOverlapping)
    {
      throw new NotSupportedException();
    }

    public virtual void SetDefaultAccessMapping(
      IVssRequestContext requestContext,
      AccessMapping accessMapping)
    {
      throw new NotSupportedException();
    }

    public AccessMapping GetPublicAccessMapping(IVssRequestContext requestContext)
    {
      LocationData locationData = this.GetLocationData(requestContext);
      return locationData.PublicAccessMapping != null ? locationData.PublicAccessMapping.Clone() : throw new AccessMappingNotRegisteredException(AccessMappingConstants.PublicAccessMappingMoniker);
    }

    public AccessMapping GetServerAccessMapping(IVssRequestContext requestContext)
    {
      LocationData locationData = this.GetLocationData(requestContext);
      return locationData.ServerAccessMapping != null ? locationData.ServerAccessMapping.Clone() : throw new AccessMappingNotRegisteredException(AccessMappingConstants.ServerAccessMappingMoniker);
    }

    public AccessMapping GetDefaultAccessMapping(IVssRequestContext requestContext) => LocationDataProvider.Clone(this.GetLocationData(requestContext).DefaultAccessMapping);

    public virtual AccessMapping DetermineAccessMapping(IVssRequestContext requestContext) => this.GetDefaultAccessMapping(requestContext);

    public AccessMapping GetAccessMapping(IVssRequestContext requestContext, string moniker)
    {
      this.ValidateRequestContext(requestContext);
      if (moniker == null)
        return (AccessMapping) null;
      LocationData locationData = this.GetLocationData(requestContext);
      AccessMapping accessMapping = (AccessMapping) null;
      if (locationData.AccessMappings.TryGetValue(moniker, out accessMapping))
        accessMapping = accessMapping.Clone();
      return accessMapping;
    }

    public IEnumerable<AccessMapping> GetAccessMappings(IVssRequestContext requestContext)
    {
      this.ValidateRequestContext(requestContext);
      return (IEnumerable<AccessMapping>) LocationDataProvider.Clone((IEnumerable<AccessMapping>) this.GetLocationData(requestContext).AccessMappings.Values);
    }

    public virtual void RemoveAccessMapping(
      IVssRequestContext requestContext,
      AccessMapping accessMapping)
    {
      throw new NotSupportedException();
    }

    public string GetSelfReferenceUrl(
      IVssRequestContext requestContext,
      AccessMapping accessMapping)
    {
      ArgumentUtility.CheckForNull<AccessMapping>(accessMapping, nameof (accessMapping));
      ServiceDefinition serviceDefinition = this.FindServiceDefinition(requestContext, "LocationService2", Microsoft.VisualStudio.Services.Location.LocationServiceConstants.SelfReferenceIdentifier);
      LocationData locationData = this.GetLocationData(requestContext);
      return this.LocationForAccessMappingInternal(requestContext, serviceDefinition, accessMapping, locationData.WebAppRelativeDirectory);
    }

    public ApiResourceLocationCollection GetResourceLocations(IVssRequestContext requestContext)
    {
      ApiResourceLocationCollection resourceLocations = (ApiResourceLocationCollection) null;
      LocationData locationData = this.GetLocationData(requestContext);
      LocationData inheritedLocationData = this.GetInheritedLocationData(requestContext);
      IEnumerable<ServiceDefinition> list = (IEnumerable<ServiceDefinition>) locationData.GetAllServiceDefinitions().Where<ServiceDefinition>((Func<ServiceDefinition, bool>) (x => x.IsResourceLocation())).Union<ServiceDefinition>(inheritedLocationData.GetAllServiceDefinitions().Where<ServiceDefinition>((Func<ServiceDefinition, bool>) (x => x.IsResourceLocation())), (IEqualityComparer<ServiceDefinition>) ServiceDefinitionComparer.Instance).ToList<ServiceDefinition>();
      if (!list.IsNullOrEmpty<ServiceDefinition>())
      {
        resourceLocations = new ApiResourceLocationCollection();
        foreach (ServiceDefinition definition in list)
          resourceLocations.AddResourceLocation(ApiResourceLocation.FromServiceDefinition(definition));
      }
      return resourceLocations;
    }

    public Uri GetResourceUri(
      IVssRequestContext requestContext,
      string serviceType,
      Guid identifier,
      object routeValues)
    {
      return this.GetResourceUri(requestContext, serviceType, identifier, routeValues, false, false);
    }

    public Uri GetResourceUri(
      IVssRequestContext requestContext,
      string serviceType,
      Guid identifier,
      object routeValues,
      bool requireExplicitRouteParams)
    {
      return this.GetResourceUri(requestContext, serviceType, identifier, routeValues, false, requireExplicitRouteParams);
    }

    public Uri GetResourceUri(
      IVssRequestContext requestContext,
      string serviceType,
      Guid identifier,
      object routeValues,
      bool appendUnusedAsQueryParams,
      bool requireExplicitRouteParams)
    {
      return this.GetResourceUri(requestContext, serviceType, identifier, routeValues, appendUnusedAsQueryParams, requireExplicitRouteParams, false);
    }

    public Uri GetResourceUri(
      IVssRequestContext requestContext,
      string serviceType,
      Guid identifier,
      object routeValues,
      bool appendUnusedAsQueryParams,
      bool requireExplicitRouteParams,
      bool wildcardAsQueryParams)
    {
      ServiceDefinition serviceDefinition = this.FindServiceDefinition(requestContext, serviceType, identifier);
      if (serviceDefinition == null)
        throw new VssResourceNotFoundException(identifier);
      Dictionary<string, object> routeDictionary = VssHttpUriUtility.ToRouteDictionary(routeValues, serviceType, serviceDefinition.DisplayName);
      serviceDefinition.RelativePath = VssHttpUriUtility.ReplaceRouteValues(serviceDefinition.RelativePath, routeDictionary, (RouteReplacementOptions) ((appendUnusedAsQueryParams ? 2 : 0) | (requireExplicitRouteParams ? 4 : 0) | (wildcardAsQueryParams ? 8 : 0)));
      return new Uri(this.LocationForAccessMapping(requestContext, serviceDefinition, this.DetermineAccessMapping(requestContext)));
    }

    public string GetWebApplicationRelativeDirectory(IVssRequestContext requestContext) => this.GetLocationData(requestContext).WebAppRelativeDirectory;

    public DateTime GetExpirationDate(IVssRequestContext requestContext) => this.GetLocationData(requestContext).CacheExpirationDate;

    public long GetLastChangeId(IVssRequestContext requestContext)
    {
      LocationData locationData = this.GetLocationData(requestContext);
      LocationData inheritedLocationData = this.GetInheritedLocationData(requestContext);
      return requestContext.ServiceHost.HostType.HasFlag((Enum) TeamFoundationHostType.Deployment) || inheritedLocationData == LocationData.Null ? locationData.LastChangeId : (long) ((uint) ((ulong) inheritedLocationData.LastChangeId & 1048575UL) << 12) + (locationData.LastChangeId & 4095L);
    }

    public virtual LocationData GetLocationData(IVssRequestContext requestContext)
    {
      LocationData locationData1 = this.LocationCache.GetLocationData(requestContext, this.CacheKey);
      requestContext.Trace(71550336, TraceLevel.Verbose, LocationDataProvider.s_area, LocationDataProvider.s_layer, "Location data from cache: {0}", (object) locationData1);
      if (locationData1 == null || locationData1.CacheExpirationDate < DateTime.UtcNow)
      {
        using (this.AcquireLoadSemaphore(requestContext))
        {
          Func<IVssRequestContext, string, LocationData> loadData = (Func<IVssRequestContext, string, LocationData>) ((request, key) =>
          {
            LocationData locationData2 = this.FetchLocationData(request);
            request.Trace(71550338, TraceLevel.Verbose, LocationDataProvider.s_area, LocationDataProvider.s_layer, "Location data from source: {0}", (object) locationData2);
            if (locationData2 != null)
            {
              locationData2 = this.PostprocessLocationData(request, locationData2);
              request.Trace(71550339, TraceLevel.Verbose, LocationDataProvider.s_area, LocationDataProvider.s_layer, "Postprocessed location data: {0}", (object) locationData2);
            }
            return locationData2;
          });
          locationData1 = this.LocationCache.GetLocationData(requestContext, this.CacheKey, loadData);
          requestContext.Trace(71550335, TraceLevel.Verbose, LocationDataProvider.s_area, LocationDataProvider.s_layer, "Location data from cache, attempt 2: {0}", (object) locationData1);
        }
      }
      return locationData1 ?? LocationData.Null;
    }

    protected virtual IDisposable AcquireLoadSemaphore(IVssRequestContext requestContext)
    {
      if (this.m_loadSemaphore.Wait(5000, requestContext.CancellationToken))
        return (IDisposable) new LocationDataProvider.SemaphoreReference(this.m_loadSemaphore);
      requestContext.Trace(71550337, TraceLevel.Error, LocationDataProvider.s_area, LocationDataProvider.s_layer, "Waited more than 5 seconds to acquire the load semaphore.");
      return (IDisposable) null;
    }

    protected internal abstract LocationData GetInheritedLocationData(
      IVssRequestContext requestContext);

    protected internal abstract LocationData FetchLocationData(IVssRequestContext requestContext);

    protected virtual LocationData PostprocessLocationData(
      IVssRequestContext requestContext,
      LocationData locationData)
    {
      return locationData;
    }

    protected List<ServiceDefinition> PostProcessAndCloneServiceDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<ServiceDefinition> definitionsToProcess,
      string webAppRelativeDirectory,
      Func<string, Guid, ServiceDefinition> definitionResolver = null)
    {
      LocationData deploymentLocationData = this.GetDeploymentLocationData(requestContext);
      return definitionsToProcess.Select<ServiceDefinition, ServiceDefinition>((Func<ServiceDefinition, ServiceDefinition>) (x => this.PostProcessAndCloneServiceDefinition(requestContext, x, webAppRelativeDirectory, deploymentLocationData, definitionResolver))).Where<ServiceDefinition>((Func<ServiceDefinition, bool>) (y => y != null)).ToList<ServiceDefinition>();
    }

    protected internal virtual ServiceDefinition PostProcessAndCloneServiceDefinition(
      IVssRequestContext requestContext,
      ServiceDefinition serviceDefinition,
      string webAppRelativeDirectory,
      Func<string, Guid, ServiceDefinition> definitionResolver = null)
    {
      return this.PostProcessAndCloneServiceDefinition(requestContext, serviceDefinition, webAppRelativeDirectory, this.GetDeploymentLocationData(requestContext), definitionResolver);
    }

    private List<ServiceDefinition> PostProcessAndCloneInheritedServiceDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<ServiceDefinition> definitionsToProcess,
      LocationData locationData,
      Func<string, Guid, ServiceDefinition> definitionResolver = null)
    {
      List<ServiceDefinition> serviceDefinitionList = this.PostProcessAndCloneServiceDefinitions(requestContext, definitionsToProcess, locationData.WebAppRelativeDirectory, definitionResolver);
      foreach (ServiceDefinition serviceDefinition in serviceDefinitionList)
        this.ProcessInheritedServiceDefinitionWithLocationData(serviceDefinition, locationData);
      return serviceDefinitionList;
    }

    private ServiceDefinition PostProcessAndCloneInheritedServiceDefinition(
      IVssRequestContext requestContext,
      ServiceDefinition serviceDefinition,
      LocationData locationData,
      Func<string, Guid, ServiceDefinition> definitionResolver = null)
    {
      ServiceDefinition serviceDefinition1 = this.PostProcessAndCloneServiceDefinition(requestContext, serviceDefinition, locationData.WebAppRelativeDirectory, definitionResolver);
      if (serviceDefinition1 != null)
        this.ProcessInheritedServiceDefinitionWithLocationData(serviceDefinition1, locationData);
      return serviceDefinition1;
    }

    private LocationData GetDeploymentLocationData(IVssRequestContext requestContext)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return (LocationData) null;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return ((IInternalLocationDataProvider) vssRequestContext.GetService<ILocationService>().GetLocationData(vssRequestContext, this.InstanceType == ServiceInstanceTypes.SPS ? ServiceInstanceTypes.SPS : this.DeploymentId)).GetLocationData(vssRequestContext);
    }

    internal ServiceDefinition PostProcessAndCloneServiceDefinition(
      IVssRequestContext requestContext,
      ServiceDefinition serviceDefinition,
      string webAppRelativeDirectory,
      LocationData deploymentLocationData,
      Func<string, Guid, ServiceDefinition> definitionResolver = null)
    {
      if (serviceDefinition == null)
        return (ServiceDefinition) null;
      ServiceDefinition definitionToAdd = serviceDefinition;
      bool flag = serviceDefinition.InheritLevel != 0;
      definitionToAdd = serviceDefinition.Clone();
      if (!string.IsNullOrEmpty(definitionToAdd.RelativePath))
      {
        if (definitionToAdd.RelativePath.Contains("{HostVDir}"))
          definitionToAdd.RelativePath = definitionToAdd.RelativePath.Replace("{HostVDir}/", webAppRelativeDirectory ?? string.Empty);
        if (string.Equals(definitionToAdd.RelativePath, "{selfreferenceidentifierpath}", StringComparison.Ordinal))
          definitionToAdd.RelativePath = !requestContext.ServiceHost.HostType.HasFlag((Enum) TeamFoundationHostType.ProjectCollection) ? Microsoft.TeamFoundation.Framework.Common.LocationServiceConstants.ApplicationLocationServiceRelativePath : Microsoft.TeamFoundation.Framework.Common.LocationServiceConstants.CollectionLocationServiceRelativePath;
        if (definitionToAdd.RelativePath.Contains("{hostrelativevdir}"))
          definitionToAdd.RelativePath = !requestContext.ServiceHost.HostType.HasFlag((Enum) TeamFoundationHostType.ProjectCollection) ? definitionToAdd.RelativePath.Replace("{hostrelativevdir}", "TeamFoundation/Administration") : definitionToAdd.RelativePath.Replace("{hostrelativevdir}", "Services");
      }
      if ((!(definitionToAdd.ParentIdentifier != Guid.Empty) || definitionToAdd.LocationMappings.Any<LocationMapping>() && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? 0 : (definitionToAdd.RelativeToSetting == Microsoft.VisualStudio.Services.Location.RelativeToSetting.FullyQualified ? 1 : (requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.LocationService.AllowRemoteServiceDefinitionsWithRelativePath") ? 1 : 0))) != 0)
      {
        requestContext.Trace(72100, TraceLevel.Info, LocationDataProvider.s_area, LocationDataProvider.s_layer, "Need to find instance for service definition {0}", (object) definitionToAdd.Identifier);
        ServiceDefinition parentDefinition = this.ResolveParentIdentifier(requestContext, definitionToAdd, deploymentLocationData, definitionResolver);
        if (parentDefinition == null)
        {
          requestContext.Trace(72102, TraceLevel.Info, LocationDataProvider.s_area, LocationDataProvider.s_layer, "Could not find parent for service definition {0}", (object) definitionToAdd.Identifier);
          if (!flag)
          {
            requestContext.TraceAlways(72103, TraceLevel.Info, LocationDataProvider.s_area, LocationDataProvider.s_layer, "Removing orphaned service definition {0}", (object) definitionToAdd.Identifier);
            requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, (object) new OrphanedServiceDefinition(definitionToAdd));
          }
          return (ServiceDefinition) null;
        }
        List<LocationMapping> locationMappingList;
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          locationMappingList = new List<LocationMapping>();
          LocationMapping locationMapping1 = parentDefinition.GetLocationMapping(AccessMappingConstants.PublicAccessMappingMoniker);
          if (locationMapping1 != null)
            locationMappingList.Add(locationMapping1.Clone());
          LocationMapping locationMapping2 = parentDefinition.GetLocationMapping(AccessMappingConstants.HostGuidAccessMappingMoniker);
          if (locationMapping2 != null)
            locationMappingList.Add(locationMapping2.Clone());
        }
        else
        {
          locationMappingList = this.GetInheritedLocationMappingsForServiceDefinition(requestContext, definitionToAdd, parentDefinition, deploymentLocationData, definitionResolver);
          if (locationMappingList == null)
            return (ServiceDefinition) null;
        }
        if (definitionToAdd.RelativeToSetting == Microsoft.VisualStudio.Services.Location.RelativeToSetting.Context || definitionToAdd.RelativeToSetting == Microsoft.VisualStudio.Services.Location.RelativeToSetting.WebApplication)
        {
          locationMappingList.ForEach((Action<LocationMapping>) (x => x.Location = TFCommonUtil.CombinePaths(x.Location, definitionToAdd.RelativePath)));
          definitionToAdd.RelativeToSetting = Microsoft.VisualStudio.Services.Location.RelativeToSetting.FullyQualified;
          definitionToAdd.RelativePath = (string) null;
        }
        definitionToAdd.LocationMappings = locationMappingList;
      }
      return definitionToAdd;
    }

    private void ProcessInheritedServiceDefinitionWithLocationData(
      ServiceDefinition serviceDefinition,
      LocationData locationData)
    {
      foreach (LocationMapping locationMapping in serviceDefinition.LocationMappings)
      {
        AccessMapping accessMapping;
        if (locationMapping.Location != null && locationMapping.Location.StartsWith("~/") && locationData.AccessMappings.TryGetValue(locationMapping.AccessMappingMoniker, out accessMapping))
        {
          UriBuilder uriBuilder = new UriBuilder(TFCommonUtil.CombinePaths(accessMapping.AccessPoint, accessMapping.VirtualDirectory));
          uriBuilder.Path = VirtualPathUtility.ToAbsolute(locationMapping.Location, uriBuilder.Path);
          locationMapping.Location = uriBuilder.ToString();
        }
      }
    }

    private List<LocationMapping> GetInheritedLocationMappingsForServiceDefinition(
      IVssRequestContext requestContext,
      ServiceDefinition definitionToAdd,
      ServiceDefinition parentDefinition,
      LocationData deploymentLocationData,
      Func<string, Guid, ServiceDefinition> definitionResolver = null)
    {
      List<LocationMapping> serviceDefinition1 = new List<LocationMapping>();
      LocationMapping locationMapping1 = (LocationMapping) null;
      LocationMapping locationMapping2 = (LocationMapping) null;
      LocationMapping locationMapping3 = (LocationMapping) null;
      LocationMapping locationMapping4 = (LocationMapping) null;
      LocationMapping locationMapping5 = (LocationMapping) null;
      ServiceDefinition serviceDefinition2;
      for (; parentDefinition != null && (locationMapping1 == null || locationMapping2 == null); parentDefinition = serviceDefinition2)
      {
        if (locationMapping4 == null)
          locationMapping4 = parentDefinition.GetLocationMapping(AccessMappingConstants.RootDomainMappingMoniker);
        if (locationMapping2 == null || locationMapping3 == null || parentDefinition.ParentServiceType == null || VssStringComparer.ServiceType.Equals(parentDefinition.ServiceType, "VsService"))
        {
          locationMapping2 = parentDefinition.GetLocationMapping(AccessMappingConstants.ServiceDomainMappingMoniker);
          locationMapping3 = parentDefinition.GetLocationMapping(AccessMappingConstants.ServicePathMappingMoniker);
        }
        if (locationMapping1 == null && (parentDefinition.ParentServiceType == null || VssStringComparer.ServiceType.Equals(parentDefinition.ParentServiceType, "VsService")))
          locationMapping1 = parentDefinition.GetLocationMapping(AccessMappingConstants.HostGuidAccessMappingMoniker);
        if (parentDefinition.ParentServiceType == null)
          locationMapping5 = parentDefinition.GetLocationMapping(AccessMappingConstants.PublicAccessMappingMoniker);
        if (!(parentDefinition.ParentIdentifier == Guid.Empty))
        {
          requestContext.Trace(72105, TraceLevel.Info, LocationDataProvider.s_area, LocationDataProvider.s_layer, "Could not find root mapping for service definition {0}, checking next parent", (object) definitionToAdd.Identifier);
          serviceDefinition2 = this.ResolveParentIdentifier(requestContext, parentDefinition, deploymentLocationData, definitionResolver);
          if (serviceDefinition2 == null)
            requestContext.Trace(72106, TraceLevel.Error, LocationDataProvider.s_area, LocationDataProvider.s_layer, "Could not find root service definition {0}", (object) definitionToAdd.Identifier);
          if (serviceDefinition2 == parentDefinition)
          {
            requestContext.Trace(72109, TraceLevel.Error, LocationDataProvider.s_area, LocationDataProvider.s_layer, "Detected cycle in the service definitions");
            break;
          }
        }
        else
          break;
      }
      if (locationMapping1 == null && locationMapping2 == null && locationMapping4 == null && locationMapping5 == null)
      {
        requestContext.Trace(72107, TraceLevel.Error, LocationDataProvider.s_area, LocationDataProvider.s_layer, "Could not find guid mapping or service mapping from service definition {0}", (object) definitionToAdd.Identifier);
        return (List<LocationMapping>) null;
      }
      if (!requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.LocationService.DontIncludeHostPublicMapping") && definitionToAdd.Identifier != ServiceInstanceTypes.SPS)
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        HostUriData hostUriData = (HostUriData) vssRequestContext.GetService<IInternalUrlHostResolutionService>().ResolveUriData(vssRequestContext, requestContext.ServiceHost.InstanceId);
        if (hostUriData != null)
        {
          LocationMapping locationMapping6 = hostUriData.AccessMappingMoniker == AccessMappingConstants.RootDomainMappingMoniker ? locationMapping4 : locationMapping3;
          if (locationMapping6 != null)
          {
            Uri uri = hostUriData.BuildUri(new Uri(locationMapping6.Location), true);
            LocationMapping locationMapping7 = new LocationMapping()
            {
              AccessMappingMoniker = AccessMappingConstants.ServerAccessMappingMoniker,
              Location = uri.AbsoluteUri
            };
            serviceDefinition1.Add(locationMapping7);
          }
        }
      }
      LocationMapping baseUrlMapping = (LocationMapping) null;
      if (locationMapping2 != null && requestContext.IsFeatureEnabled("VisualStudio.Services.Location.UseDevOpsDomainForS2S"))
        baseUrlMapping = locationMapping2;
      else if (locationMapping1 != null)
        baseUrlMapping = locationMapping1;
      else if (locationMapping5 != null)
        baseUrlMapping = locationMapping5;
      if (baseUrlMapping != null)
      {
        LocationMapping guidLocationMapping = LocationServiceHelper.GetHostGuidLocationMapping(requestContext, requestContext.ServiceHost.InstanceId, baseUrlMapping);
        serviceDefinition1.Add(guidLocationMapping);
      }
      return serviceDefinition1;
    }

    protected ServiceDefinition ResolveParentIdentifier(
      IVssRequestContext requestContext,
      ServiceDefinition definition)
    {
      return this.ResolveParentIdentifier(requestContext, definition, this.GetDeploymentLocationData(requestContext));
    }

    private ServiceDefinition ResolveParentIdentifier(
      IVssRequestContext requestContext,
      ServiceDefinition definition,
      LocationData deploymentLocationData,
      Func<string, Guid, ServiceDefinition> definitionResolver = null)
    {
      ServiceDefinition serviceDefinition;
      if (definitionResolver != null)
      {
        serviceDefinition = definitionResolver(definition.ParentServiceType, definition.ParentIdentifier);
      }
      else
      {
        LocationData locationData = this.GetLocationData(requestContext);
        serviceDefinition = locationData.FindServiceDefinition(definition.ParentServiceType, definition.ParentIdentifier);
        if (serviceDefinition != null)
          return serviceDefinition;
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && this.InstanceType == ServiceInstanceTypes.SPS)
        {
          List<ServiceDefinition> list = locationData.FindServiceDefinitionsByType("LocationService2").Where<ServiceDefinition>((Func<ServiceDefinition, bool>) (def => VssStringComparer.ServiceType.Equals(def.ParentServiceType, "VsService") && def.ParentIdentifier == definition.ParentIdentifier)).ToList<ServiceDefinition>();
          if (list.Count == 1)
            return list[0];
        }
      }
      if (deploymentLocationData != null)
        serviceDefinition = deploymentLocationData.FindServiceDefinition(definition.ParentServiceType, definition.ParentIdentifier);
      return serviceDefinition != null && (!VssStringComparer.ServiceType.Equals(serviceDefinition.ParentServiceType, "VsService") || definition.Identifier == serviceDefinition.ParentIdentifier) ? serviceDefinition : (ServiceDefinition) null;
    }

    protected Dictionary<string, AccessMapping> GetAccessMappings(
      IVssRequestContext requestContext,
      IEnumerable<AccessMapping> accessMappings,
      string defaultAccessMappingMoniker,
      string webAppRelativeDirectory,
      Guid serviceOwner,
      out AccessMapping defaultAccessMapping,
      out AccessMapping publicAccessMapping,
      out AccessMapping serverAccessMapping)
    {
      Dictionary<string, AccessMapping> accessMappings1 = new Dictionary<string, AccessMapping>((IEqualityComparer<string>) VssStringComparer.AccessMappingMoniker);
      defaultAccessMapping = (AccessMapping) null;
      publicAccessMapping = (AccessMapping) null;
      serverAccessMapping = (AccessMapping) null;
      foreach (AccessMapping accessMapping in accessMappings)
      {
        accessMapping.ServiceOwner = serviceOwner;
        if (accessMapping.VirtualDirectory == null)
          accessMapping.VirtualDirectory = LocationServiceHelper.ComputeVirtualDirectory(webAppRelativeDirectory);
        accessMappings1[accessMapping.Moniker] = accessMapping;
      }
      if (!string.IsNullOrEmpty(defaultAccessMappingMoniker))
        defaultAccessMapping = accessMappings1[defaultAccessMappingMoniker];
      if (!accessMappings1.TryGetValue(AccessMappingConstants.PublicAccessMappingMoniker, out publicAccessMapping))
        publicAccessMapping = defaultAccessMapping;
      if (!accessMappings1.TryGetValue(AccessMappingConstants.ServerAccessMappingMoniker, out serverAccessMapping))
        serverAccessMapping = publicAccessMapping;
      return accessMappings1;
    }

    protected internal virtual void ValidateRequestContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (this.m_serviceHost.InstanceId != requestContext.ServiceHost.InstanceId)
        throw new InvalidRequestContextHostException(FrameworkResources.LocationServiceRequestContextHostMessage((object) this.m_serviceHost.InstanceId, (object) requestContext.ServiceHost.InstanceId));
    }

    protected virtual bool CanFaultInDefinition(
      IVssRequestContext requestContext,
      string serviceType,
      Guid identifier)
    {
      return !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && this.InstanceType == ServiceInstanceTypes.SPS && VssStringComparer.ServiceType.Equals(serviceType, "LocationService2") && identifier != Microsoft.VisualStudio.Services.Location.LocationServiceConstants.RootIdentifier && identifier != Microsoft.VisualStudio.Services.Location.LocationServiceConstants.ApplicationIdentifier;
    }

    protected abstract Guid DeploymentId { get; }

    protected static AccessMapping Clone(AccessMapping accessMapping) => accessMapping?.Clone();

    protected static ServiceDefinition Clone(
      IVssRequestContext requestContext,
      ServiceDefinition serviceDefinition)
    {
      return serviceDefinition?.Clone();
    }

    protected static List<AccessMapping> Clone(IEnumerable<AccessMapping> accessMappings) => new List<AccessMapping>(accessMappings.Select<AccessMapping, AccessMapping>((Func<AccessMapping, AccessMapping>) (accessMapping => accessMapping.Clone())));

    protected static List<ServiceDefinition> Clone(
      IVssRequestContext requestContext,
      IEnumerable<ServiceDefinition> serviceDefinitions)
    {
      return new List<ServiceDefinition>(serviceDefinitions.Select<ServiceDefinition, ServiceDefinition>((Func<ServiceDefinition, ServiceDefinition>) (serviceDefinition => serviceDefinition.Clone())));
    }

    public void Dispose()
    {
      if (this.m_loadSemaphore == null)
        return;
      this.m_loadSemaphore.Dispose();
      this.m_loadSemaphore = (SemaphoreSlim) null;
    }

    protected class SemaphoreReference : IDisposable
    {
      private readonly SemaphoreSlim m_semaphore;

      public SemaphoreReference(SemaphoreSlim semaphore) => this.m_semaphore = semaphore;

      public void Dispose()
      {
        try
        {
          this.m_semaphore.Release();
        }
        catch (ObjectDisposedException ex)
        {
        }
      }
    }
  }
}
