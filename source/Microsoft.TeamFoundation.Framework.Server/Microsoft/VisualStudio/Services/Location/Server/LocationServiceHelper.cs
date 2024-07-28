// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Server.LocationServiceHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Location.Client;
using Microsoft.VisualStudio.Services.Partitioning;
using Microsoft.VisualStudio.Services.Partitioning.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Microsoft.VisualStudio.Services.Location.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class LocationServiceHelper
  {
    internal static string GetLocationPointerUrl(
      IVssRequestContext requestContext,
      ILocationDataProvider provider,
      Guid identifier)
    {
      ServiceDefinition serviceDefinition = provider.FindServiceDefinition(requestContext, "LocationService2", identifier) ?? LocationServiceHelper.GetSingleParentDefinition(requestContext, identifier, provider);
      if (serviceDefinition == null)
        return (string) null;
      if (serviceDefinition.RelativeToSetting != Microsoft.VisualStudio.Services.Location.RelativeToSetting.FullyQualified)
        return provider.LocationForAccessMapping(requestContext, serviceDefinition, provider.GetAccessMapping(requestContext, AccessMappingConstants.HostGuidAccessMappingMoniker) ?? provider.GetPublicAccessMapping(requestContext));
      LocationMapping locationMapping1 = serviceDefinition.GetLocationMapping(AccessMappingConstants.HostGuidAccessMappingMoniker);
      if (locationMapping1 != null)
        return locationMapping1.Location;
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) || requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        LocationMapping locationMapping2 = serviceDefinition.GetLocationMapping(AccessMappingConstants.PublicAccessMappingMoniker);
        if (locationMapping2 != null)
          return locationMapping2.Location;
      }
      return (string) null;
    }

    internal static ServiceDefinition GetSingleParentDefinition(
      IVssRequestContext requestContext,
      Guid identifier,
      ILocationDataProvider provider)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && provider.InstanceType == ServiceInstanceTypes.SPS)
      {
        List<ServiceDefinition> list = provider.FindServiceDefinitions(requestContext, "LocationService2").Where<ServiceDefinition>((Func<ServiceDefinition, bool>) (def => VssStringComparer.ServiceType.Equals(def.ParentServiceType, "VsService") && def.ParentIdentifier == identifier)).ToList<ServiceDefinition>();
        if (list.Count == 1)
          return list[0];
      }
      return (ServiceDefinition) null;
    }

    internal static AccessMapping GetHostPublicAccessMapping(
      IVssRequestContext requestContext,
      Guid hostId)
    {
      requestContext.CheckHostedDeployment();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return !(vssRequestContext.GetService<IInternalUrlHostResolutionService>().ResolveUriData(vssRequestContext, hostId) is IInternalHostUriData internalHostUriData) ? (AccessMapping) null : internalHostUriData.BuildAccessMapping(vssRequestContext, AccessMappingConstants.PublicAccessMappingMoniker, TFCommonResources.PublicAccessMappingDisplayName());
    }

    public static LocationMapping ToLocationMapping(this AccessMapping accessMapping) => new LocationMapping()
    {
      AccessMappingMoniker = accessMapping.Moniker,
      Location = VirtualPathUtility.AppendTrailingSlash(TFCommonUtil.CombinePaths(accessMapping.AccessPoint, accessMapping.VirtualDirectory))
    };

    internal static AccessMapping GetHostGuidAccessMapping(
      IVssRequestContext requestContext,
      Guid hostId,
      LocationMapping baseUrlMapping)
    {
      ArgumentUtility.CheckForNull<LocationMapping>(baseUrlMapping, nameof (baseUrlMapping));
      string str = baseUrlMapping.Location;
      bool flag = true;
      if (baseUrlMapping.AccessMappingMoniker.Equals(AccessMappingConstants.ServiceDomainMappingMoniker, StringComparison.OrdinalIgnoreCase))
      {
        str = UriUtility.CombinePath(str, "serviceHosts/" + hostId.ToString("D"));
        flag = false;
      }
      return LocationServiceHelper.GetHostGuidAccessMapping(requestContext, hostId, str, new bool?(flag));
    }

    internal static AccessMapping GetHostGuidAccessMapping(
      IVssRequestContext requestContext,
      Guid hostId,
      string baseUrl = null,
      bool? useAGuid = null,
      Guid? serviceIdentifier = null)
    {
      requestContext.CheckHostedDeployment();
      if (baseUrl == null)
      {
        if (useAGuid.HasValue)
          throw new InvalidOperationException("useAGuid parameter should not be supplied without baseUrl");
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.Location.UseDevOpsDomainForS2S"))
        {
          Uri serviceBaseUri = LocationServiceHelper.GetServiceBaseUri(requestContext.To(TeamFoundationHostType.Deployment), AccessMappingConstants.ServiceDomainMappingMoniker, serviceIdentifier.GetValueOrDefault(), false);
          if (serviceBaseUri != (Uri) null)
          {
            baseUrl = UriUtility.Combine(serviceBaseUri, "serviceHosts/" + hostId.ToString("D"), false).AbsoluteUri;
            useAGuid = new bool?(false);
          }
        }
        if (baseUrl == null)
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          baseUrl = vssRequestContext.GetService<ILocationService>().GetLocationServiceUrl(vssRequestContext, serviceIdentifier ?? Microsoft.VisualStudio.Services.Location.LocationServiceConstants.SelfReferenceIdentifier, AccessMappingConstants.PublicAccessMappingMoniker);
          useAGuid = new bool?(true);
        }
      }
      else if (!useAGuid.HasValue)
        throw new InvalidOperationException("useAGuid parameter must be supplied with baseUrl");
      if (hostId == Guid.Empty)
        hostId = requestContext.ServiceHost.InstanceId;
      return new AccessMapping()
      {
        Moniker = AccessMappingConstants.HostGuidAccessMappingMoniker,
        DisplayName = "Host Guid Access Mapping",
        AccessPoint = baseUrl,
        VirtualDirectory = useAGuid.Value ? string.Format("A{0}", (object) hostId.ToString("D")) : (string) null
      };
    }

    internal static LocationMapping GetHostGuidLocationMapping(
      IVssRequestContext requestContext,
      Guid hostId,
      LocationMapping baseUrlMapping)
    {
      requestContext.CheckHostedDeployment();
      return LocationServiceHelper.GetHostGuidAccessMapping(requestContext, hostId, baseUrlMapping).ToLocationMapping();
    }

    internal static LocationMapping GetHostGuidLocationMapping(
      IVssRequestContext requestContext,
      Guid hostId,
      string baseUrl = null,
      bool? useAGuid = null)
    {
      requestContext.CheckHostedDeployment();
      return LocationServiceHelper.GetHostGuidAccessMapping(requestContext, hostId, baseUrl, useAGuid).ToLocationMapping();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetRootLocationServiceUrl(
      IVssRequestContext requestContext,
      Guid hostId,
      bool throwOnMissingHost = true,
      bool forceDevOpsDomainUrls = false)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Location.UseDevOpsDomainForS2S") | forceDevOpsDomainUrls)
      {
        ILocationService service = vssRequestContext.GetService<ILocationService>();
        string part1 = (string) null;
        if (requestContext.ServiceInstanceType() == ServiceInstanceTypes.SPS)
        {
          AccessMapping accessMapping = service.GetAccessMapping(vssRequestContext, AccessMappingConstants.ServiceDomainMappingMoniker);
          if (accessMapping != null)
            part1 = accessMapping.AccessPoint;
        }
        else
        {
          LocationMapping locationMapping = service.FindServiceDefinition(vssRequestContext, "LocationService2", ServiceInstanceTypes.SPS)?.GetLocationMapping(AccessMappingConstants.ServiceDomainMappingMoniker);
          if (locationMapping != null)
            part1 = locationMapping.Location;
        }
        if (part1 != null)
          return UriUtility.CombinePath(part1, "serviceHosts/" + hostId.ToString("D"));
      }
      Partition partition = vssRequestContext.GetService<IPartitioningService>().QueryPartition<Guid>(vssRequestContext.Elevate(), hostId, ServiceInstanceTypes.SPS);
      if (partition != null)
        return UriUtility.CombinePath(partition.Container.Address, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "A{0}", (object) hostId.ToString("D")));
      if (throwOnMissingHost)
        throw new PartitionNotFoundException(string.Format("Could not find partition for hostId: {0}.", (object) hostId));
      return (string) null;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static ServiceDefinition GetSpsDefinitionForHost(
      IVssRequestContext requestContext,
      Guid hostId,
      bool includeLocationMappings = false)
    {
      ServiceDefinition serviceDefinition = (ServiceDefinition) null;
      IVssRequestContext rootContext = requestContext.RootContext;
      if (!rootContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && (rootContext.ServiceHost.InstanceId == hostId || rootContext.ServiceHost.OrganizationServiceHost.InstanceId == hostId))
        serviceDefinition = rootContext.GetService<ILocationService>().FindServiceDefinition(rootContext, "LocationService2", ServiceInstanceTypes.SPS);
      return serviceDefinition ?? LocationServiceHelper.CreateSpsServiceDefinition(requestContext, hostId, includeLocationMappings);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static void UpdateSpsServiceLocationRaw(IVssRequestContext hostContext)
    {
      if (hostContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(hostContext.ServiceHost.HostType);
      ServiceDefinition serviceDefinition = LocationServiceHelper.CreateSpsServiceDefinition(hostContext, hostContext.ServiceHost.InstanceId);
      using (LocationComponent component = hostContext.CreateComponent<LocationComponent>())
      {
        component.RemoveServiceDefinitions((IEnumerable<ServiceDefinition>) new ServiceDefinition[1]
        {
          serviceDefinition
        }, false);
        component.SaveServiceDefinitions((IEnumerable<ServiceDefinition>) new ServiceDefinition[1]
        {
          serviceDefinition
        });
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static void ConfigureSpsServiceLocation(IVssRequestContext hostContext)
    {
      if (hostContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(hostContext.ServiceHost.HostType);
      ServiceDefinition serviceDefinition = LocationServiceHelper.CreateSpsServiceDefinition(hostContext, hostContext.ServiceHost.InstanceId);
      hostContext.GetService<ILocationService>().SaveServiceDefinitions(hostContext, (IEnumerable<ServiceDefinition>) new ServiceDefinition[1]
      {
        serviceDefinition
      });
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static ServiceDefinition CreateSpsServiceDefinition(
      IVssRequestContext requestContext,
      Guid hostId,
      bool includeLocationMappings = false)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      Partition partition = vssRequestContext.GetService<IPartitioningService>().QueryPartition<Guid>(vssRequestContext, hostId, ServiceInstanceTypes.SPS);
      if (partition == null)
        throw new PartitionNotFoundException(string.Format("Could not find partition for hostId: {0}.", (object) hostId));
      LocationServiceHelper.EnsureSpsInstanceServiceDefinitionExists(vssRequestContext, partition.Container.ContainerId, partition.Container.Address);
      if (includeLocationMappings)
      {
        PropertiesCollection propertiesCollection = new PropertiesCollection();
        propertiesCollection.Add(InstanceManagementPropertyConstants.HostedServiceName, (object) partition.Container.Name);
        List<LocationMapping> locationMappingList = new List<LocationMapping>()
        {
          new LocationMapping()
          {
            AccessMappingMoniker = AccessMappingConstants.PublicAccessMappingMoniker,
            Location = partition.Container.Address
          },
          new LocationMapping()
          {
            AccessMappingMoniker = AccessMappingConstants.AzureInstanceMappingMoniker,
            Location = partition.Container.InternalAddress
          }
        };
        return new ServiceDefinition("LocationService2", ServiceInstanceTypes.SPS, "SPS Location Service", (string) null, Microsoft.VisualStudio.Services.Location.RelativeToSetting.FullyQualified, (string) null, "Framework")
        {
          ParentServiceType = "LocationService2",
          ParentIdentifier = partition.Container.ContainerId,
          LocationMappings = locationMappingList,
          Properties = propertiesCollection
        };
      }
      return new ServiceDefinition("LocationService2", ServiceInstanceTypes.SPS, "SPS Location Service", (string) null, Microsoft.VisualStudio.Services.Location.RelativeToSetting.FullyQualified, (string) null, "Framework")
      {
        ParentServiceType = "LocationService2",
        ParentIdentifier = partition.Container.ContainerId
      };
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static void EnsureSpsInstanceServiceDefinitionExists(
      IVssRequestContext deploymentContext,
      Guid spsInstanceId,
      string spsInstanceUrl)
    {
      deploymentContext.CheckDeploymentRequestContext();
      ILocationService service = deploymentContext.GetService<ILocationService>();
      if (service.FindServiceDefinition(deploymentContext, "LocationService2", spsInstanceId) != null)
        return;
      if (spsInstanceUrl == null)
        spsInstanceUrl = (deploymentContext.GetService<IPartitioningService>().GetPartitionContainer(deploymentContext, spsInstanceId) ?? throw new InvalidOperationException(string.Format("Could not find SPS partition container: {0}.", (object) spsInstanceId))).Address;
      LocationMapping locationMapping1 = (LocationMapping) null;
      LocationMapping locationMapping2 = (LocationMapping) null;
      if (deploymentContext.ServiceInstanceType() == ServiceInstanceTypes.SPS)
      {
        AccessMapping accessMapping1 = service.GetAccessMapping(deploymentContext, AccessMappingConstants.ServiceDomainMappingMoniker);
        AccessMapping accessMapping2 = service.GetAccessMapping(deploymentContext, AccessMappingConstants.ServicePathMappingMoniker);
        if (accessMapping2 != null && accessMapping1 != null)
        {
          locationMapping1 = accessMapping1.ToLocationMapping();
          locationMapping2 = accessMapping2.ToLocationMapping();
        }
      }
      else
      {
        ServiceDefinition serviceDefinition = service.FindServiceDefinition(deploymentContext, "LocationService2", ServiceInstanceTypes.SPS);
        locationMapping1 = serviceDefinition != null ? serviceDefinition.GetLocationMapping(AccessMappingConstants.ServiceDomainMappingMoniker) : throw new InvalidOperationException("No SPS service definition was installed.");
        locationMapping2 = serviceDefinition.GetLocationMapping(AccessMappingConstants.ServicePathMappingMoniker);
      }
      if (locationMapping1 == null ^ locationMapping2 == null)
        throw new InvalidOperationException("Must register both ServiceDomainAccessMapping and ServicePathAccessMapping.");
      ServiceDefinition serviceDefinition1 = new ServiceDefinition("LocationService2", spsInstanceId, "SPS Location Service", (string) null, Microsoft.VisualStudio.Services.Location.RelativeToSetting.FullyQualified, (string) null, "Framework");
      if (locationMapping2 != null && locationMapping1 != null && deploymentContext.IsFeatureEnabled("VisualStudio.Services.Location.UseDevOpsDomainForS2S"))
      {
        serviceDefinition1.LocationMappings.Add(new LocationMapping(AccessMappingConstants.PublicAccessMappingMoniker, spsInstanceUrl));
        serviceDefinition1.LocationMappings.Add(locationMapping1.Clone());
        serviceDefinition1.LocationMappings.Add(locationMapping2.Clone());
      }
      else
        serviceDefinition1.LocationMappings.Add(new LocationMapping(AccessMappingConstants.HostGuidAccessMappingMoniker, spsInstanceUrl));
      service.SaveServiceDefinitions(deploymentContext, (IEnumerable<ServiceDefinition>) new ServiceDefinition[1]
      {
        serviceDefinition1
      });
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static LocationHttpClient GetSpsLocationClient(
      IVssRequestContext requestContext,
      Guid hostId)
    {
      string locationServiceUrl = LocationServiceHelper.GetRootLocationServiceUrl(requestContext, hostId);
      return !string.IsNullOrEmpty(locationServiceUrl) ? LocationServiceHelper.CreateLocationClient(requestContext, locationServiceUrl) : throw new ArgumentNullException("spsLocationService");
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static LocationHttpClient CreateLocationClient(
      IVssRequestContext requestContext,
      string locationServiceUrl,
      bool requiresResourceLocations = true)
    {
      return (requestContext.ClientProvider as ICreateClient).CreateClient<LocationHttpClient>(requestContext, new Uri(locationServiceUrl), "LocationService", (ApiResourceLocationCollection) null, requiresResourceLocations);
    }

    public static int GetClientCacheTimeToLive(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.LocationClientCacheTimeToLive, true, requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? 86400 : 3600);

    internal static string ComputeWebApplicationRelativeDirectory(IVssRequestContext requestContext)
    {
      string str1 = requestContext.WebApplicationPath();
      string str2 = requestContext.VirtualPath().Substring(str1.Length);
      if (string.IsNullOrEmpty(str2))
        return (string) null;
      return str2.TrimStart('~', '/');
    }

    internal static string ComputeVirtualDirectory(string webApplicationRelativeDirectory)
    {
      if (webApplicationRelativeDirectory == null)
        return string.Empty;
      return webApplicationRelativeDirectory.TrimEnd('/');
    }

    internal static string GetWebApplicationRelativeDirectory(IVssRequestContext requestContext)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return service.GetWebApplicationRelativeDirectory(requestContext);
      string relativeDirectory = (string) null;
      AccessMapping accessMapping = service.DetermineAccessMapping(requestContext);
      if (!string.IsNullOrEmpty(accessMapping.VirtualDirectory))
        relativeDirectory = VirtualPathUtility.AppendTrailingSlash(accessMapping.VirtualDirectory);
      return relativeDirectory;
    }

    internal static void FixVirtualServiceDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<ServiceDefinition> serviceDefinitions)
    {
      if (serviceDefinitions == null || !requestContext.ExecutionEnvironment.IsHostedDeployment || !requestContext.RootContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return;
      bool flag = !requestContext.IsDevOpsDomainRequest() && !requestContext.RootContext.VirtualPath().EndsWith("/DefaultCollection/", StringComparison.OrdinalIgnoreCase) && LocationServiceHelper.UseLegacyDefaultCollectionRouting(requestContext);
      foreach (ServiceDefinition serviceDefinition in serviceDefinitions)
      {
        if (flag && serviceDefinition.Identifier == requestContext.ServiceHost.InstanceId && VssStringComparer.ServiceType.Equals(serviceDefinition.ServiceType, "LocationService"))
        {
          serviceDefinition.RelativeToSetting = Microsoft.VisualStudio.Services.Location.RelativeToSetting.WebApplication;
          serviceDefinition.RelativePath = UriUtility.CombinePath(UriUtility.CombinePath(requestContext.RootContext.VirtualPath(), "DefaultCollection"), Microsoft.TeamFoundation.Framework.Common.LocationServiceConstants.CollectionLocationServiceRelativePath);
        }
        if (serviceDefinition.Identifier == Microsoft.TeamFoundation.Framework.Common.LocationServiceConstants.SelfReferenceLocationServiceIdentifier && VssStringComparer.ServiceType.Equals(serviceDefinition.ServiceType, "LocationService") && requestContext.RequestPath().StartsWith(UriUtility.CombinePath(requestContext.RootContext.VirtualPath(), Microsoft.TeamFoundation.Framework.Common.LocationServiceConstants.ApplicationLocationServiceRelativePath), StringComparison.OrdinalIgnoreCase))
          serviceDefinition.RelativePath = Microsoft.TeamFoundation.Framework.Common.LocationServiceConstants.ApplicationLocationServiceRelativePath;
      }
    }

    internal static bool UseLegacyDefaultCollectionRouting(IVssRequestContext requestContext)
    {
      IVssRequestContext rootContext = requestContext.RootContext;
      string userAgent = rootContext.UserAgent;
      return userAgent != null && userAgent.StartsWith("Team Foundation (", StringComparison.OrdinalIgnoreCase) && rootContext.GetService<IVssRegistryService>().GetValue<bool>(rootContext, in UrlHostResolutionConstants.VsUsesLegacyDefaultCollectionRoutingQuery, false);
    }

    internal static bool ShouldRemoveApplicationDefinitionForDev12(IVssRequestContext requestContext)
    {
      string str = requestContext.UserAgent ?? string.Empty;
      return requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && requestContext.ServiceInstanceType() == ServiceInstanceTypes.TFS && requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.LocationService.RemoveApplicationDefinitionForDev12") && str.StartsWith("VSServices/12.", StringComparison.Ordinal);
    }

    internal static void PostProcessRemoteDefinitions(
      IEnumerable<ServiceDefinition> definitions,
      Guid serviceOwner)
    {
      foreach (ServiceDefinition definition in definitions)
      {
        definition.ServiceOwner = serviceOwner;
        definition.ToolId = "Framework";
      }
    }

    public static Uri GetServiceBaseUri(
      IVssRequestContext requestContext,
      Guid serviceIdentifier = default (Guid),
      bool throwOnMissing = true)
    {
      return LocationServiceHelper.GetServiceBaseUri(requestContext.To(TeamFoundationHostType.Deployment), requestContext.UseDevOpsDomainUrls() ? AccessMappingConstants.ServicePathMappingMoniker : AccessMappingConstants.RootDomainMappingMoniker, serviceIdentifier, throwOnMissing);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Uri GetServiceBaseUri(
      IVssRequestContext deploymentContext,
      string accessMappingMoniker,
      Guid serviceIdentifier = default (Guid),
      bool throwOnMissing = true)
    {
      if (deploymentContext.ExecutionEnvironment.IsOnPremisesDeployment || serviceIdentifier == Guid.Empty || serviceIdentifier == deploymentContext.ServiceInstanceType())
      {
        ILocationService service = deploymentContext.GetService<ILocationService>();
        AccessMapping accessMapping = service.GetAccessMapping(deploymentContext, accessMappingMoniker);
        return new Uri(service.GetSelfReferenceUrl(deploymentContext, accessMapping));
      }
      ILocationService service1 = deploymentContext.GetService<ILocationService>();
      ILocationDataProvider locationDataProvider = service1.GetLocationData(deploymentContext, ServiceInstanceTypes.SPS, false) ?? service1.GetLocationData(deploymentContext, Guid.Empty);
      if (serviceIdentifier == ServiceInstanceTypes.SPS)
      {
        AccessMapping accessMapping = locationDataProvider.GetAccessMapping(deploymentContext, accessMappingMoniker);
        return new Uri(locationDataProvider.GetSelfReferenceUrl(deploymentContext, accessMapping));
      }
      ServiceDefinition serviceDefinition = locationDataProvider.FindServiceDefinition(deploymentContext, "VsService", serviceIdentifier);
      if (serviceDefinition == null)
      {
        if (throwOnMissing)
          throw new ServiceOwnerNotFoundException(string.Format("There is no service type registered for '{0}'.", (object) serviceIdentifier));
        return (Uri) null;
      }
      LocationMapping locationMapping = serviceDefinition.GetLocationMapping(accessMappingMoniker);
      if (locationMapping != null)
        return new Uri(locationMapping.Location);
      if (throwOnMissing)
        throw new LocationMappingDoesNotExistException(serviceDefinition.ServiceType, serviceDefinition.Identifier.ToString(), accessMappingMoniker);
      return (Uri) null;
    }

    internal static void SetLocationMappings(
      List<ServiceDefinition> serviceDefinitions,
      List<LocationMappingData> locationMappings)
    {
      foreach ((ServiceDefinition, LocationMappingData) tuple in serviceDefinitions.Join<ServiceDefinition, LocationMappingData, (string, Guid), (ServiceDefinition, LocationMappingData)>((IEnumerable<LocationMappingData>) locationMappings, (Func<ServiceDefinition, (string, Guid)>) (sd => (sd.ServiceType, sd.Identifier)), (Func<LocationMappingData, (string, Guid)>) (lm => (lm.ServiceType, lm.ServiceIdentifier)), (Func<ServiceDefinition, LocationMappingData, (ServiceDefinition, LocationMappingData)>) ((sd, lm) => (sd, lm)), (IEqualityComparer<(string, Guid)>) LocationServiceHelper.ServiceTypeServiceIdentifierEqualityComparer.Instance))
        tuple.Item1.LocationMappings.Add(new LocationMapping()
        {
          AccessMappingMoniker = tuple.Item2.AccessMappingMoniker,
          Location = tuple.Item2.Location
        });
    }

    private class ServiceTypeServiceIdentifierEqualityComparer : 
      IEqualityComparer<(string Type, Guid Identifier)>
    {
      public static readonly LocationServiceHelper.ServiceTypeServiceIdentifierEqualityComparer Instance = new LocationServiceHelper.ServiceTypeServiceIdentifierEqualityComparer();

      private ServiceTypeServiceIdentifierEqualityComparer()
      {
      }

      public bool Equals((string Type, Guid Identifier) x, (string Type, Guid Identifier) y) => VssStringComparer.ServiceType.Equals(x.Type, y.Type) && x.Identifier == y.Identifier;

      public int GetHashCode((string Type, Guid Identifier) obj) => VssStringComparer.ServiceType.GetHashCode(obj.Type) ^ obj.Identifier.GetHashCode();
    }
  }
}
