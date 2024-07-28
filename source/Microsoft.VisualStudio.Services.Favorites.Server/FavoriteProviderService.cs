// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Favorites.Server.FavoriteProviderService
// Assembly: Microsoft.VisualStudio.Services.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EC45EF48-58C1-4A3C-8054-12C79AD0CC5B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Favorites.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Favorites.WebApi;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Favorites.Server
{
  public class FavoriteProviderService : IFavoriteProviderService, IVssFrameworkService
  {
    private const int locationServiceTimeoutInMilliseconds = 10000;
    private static readonly RegistryQuery locationServiceQuery = new RegistryQuery("/Configuration/Favorites/LocationServiceTimeout");
    private static readonly HashSet<string> ContributionsToFilter = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "ms.vss-favorites.favorite-provider-collection"
    };
    private static readonly HashSet<string> ContributionTypesToFilter = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "ms.vss-favorites.favorite-provider"
    };
    private readonly CommandPropertiesSetter _commandPropertiesForGetServiceUri = new CommandPropertiesSetter().WithCircuitBreakerForceClosed(true);
    private readonly CommandPropertiesSetter _commandPropertiesForGetResourceUri = new CommandPropertiesSetter().WithCircuitBreakerForceClosed(true);
    private const int DefaultHubGroupRenderOrder = 99;

    private TimeSpan GetLocationServiceTimeout(IVssRequestContext requestContext) => TimeSpan.FromMilliseconds((double) requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, in FavoriteProviderService.locationServiceQuery, true, 10000));

    public IEnumerable<FavoriteProvider> GetFavoriteProviders(
      IVssRequestContext requestContext,
      bool faultInMissingHost,
      ISet<string> artifactTypes = null)
    {
      using (TimedCiEvent timedCiEvent = new TimedCiEvent(requestContext, "Favorites", nameof (GetFavoriteProviders)))
      {
        timedCiEvent[nameof (faultInMissingHost)] = (object) faultInMissingHost;
        List<Contribution> providerContributions = FavoriteProviderService.GetFavoriteProviderContributions(requestContext);
        if (!providerContributions.Any<Contribution>())
          return (IEnumerable<FavoriteProvider>) new List<FavoriteProvider>();
        IList<FavoriteProvider> favoriteProviderList = FavoriteProviderService.ConvertContributionToFavoriteProvider((IEnumerable<Contribution>) providerContributions);
        if (artifactTypes != null)
          favoriteProviderList = (IList<FavoriteProvider>) favoriteProviderList.Where<FavoriteProvider>((Func<FavoriteProvider, bool>) (p => artifactTypes.Contains(p.ArtifactType))).ToList<FavoriteProvider>();
        Dictionary<Guid, FavoriteProviderService.ServiceInstanceInfo> serviceInstanceInfos = this.GetServiceInstanceInfos(requestContext, faultInMissingHost, (IEnumerable<FavoriteProvider>) favoriteProviderList);
        foreach (FavoriteProvider favoriteProvider in (IEnumerable<FavoriteProvider>) favoriteProviderList)
        {
          FavoriteProviderService.ServiceInstanceInfo serviceInstanceInfo;
          if (serviceInstanceInfos.TryGetValue(favoriteProvider.ServiceIdentifier, out serviceInstanceInfo))
          {
            favoriteProvider.ServiceUri = serviceInstanceInfo.ServiceUri;
            favoriteProvider.ArtifactUri = serviceInstanceInfo.ArtifactUri;
          }
        }
        IEnumerable<FavoriteProvider> source = favoriteProviderList.Where<FavoriteProvider>((Func<FavoriteProvider, bool>) (x => !string.IsNullOrEmpty(x.ServiceUri)));
        timedCiEvent["favoriteProvidersCount"] = (object) favoriteProviderList.Count<FavoriteProvider>();
        timedCiEvent["favoriteProvidersNotProvisioned"] = (object) (favoriteProviderList.Count<FavoriteProvider>() - source.GroupBy<FavoriteProvider, string>((Func<FavoriteProvider, string>) (x => x.ServiceUri)).Count<IGrouping<string, FavoriteProvider>>());
        timedCiEvent["favoriteTypesCount"] = (object) source.Count<FavoriteProvider>();
        return source;
      }
    }

    private Dictionary<Guid, FavoriteProviderService.ServiceInstanceInfo> GetServiceInstanceInfos(
      IVssRequestContext requestContext,
      bool faultInMissingHost,
      IEnumerable<FavoriteProvider> favoriteArtifactProviders)
    {
      HashSet<Guid> serviceIdentifiers = new HashSet<Guid>(favoriteArtifactProviders.Select<FavoriteProvider, Guid>((Func<FavoriteProvider, Guid>) (x => x.ServiceIdentifier)));
      return faultInMissingHost || requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? this.GetServiceAndArtifactUris(requestContext, (IEnumerable<Guid>) serviceIdentifiers) : this.GetServiceAndArtifactUriForProvisionedServices(requestContext, (ICollection<Guid>) serviceIdentifiers);
    }

    private Dictionary<Guid, FavoriteProviderService.ServiceInstanceInfo> GetServiceAndArtifactUris(
      IVssRequestContext requestContext,
      IEnumerable<Guid> serviceIdentifiers)
    {
      Dictionary<Guid, FavoriteProviderService.ServiceInstanceInfo> serviceAndArtifactUris = new Dictionary<Guid, FavoriteProviderService.ServiceInstanceInfo>();
      foreach (Guid serviceIdentifier in serviceIdentifiers)
      {
        FavoriteProviderService.ServiceInstanceInfo serviceInstanceInfo = new FavoriteProviderService.ServiceInstanceInfo()
        {
          ServiceUri = this.GetServiceUri(requestContext, serviceIdentifier),
          ArtifactUri = this.GetResourceUri(requestContext, serviceIdentifier)
        };
        serviceAndArtifactUris.Add(serviceIdentifier, serviceInstanceInfo);
      }
      return serviceAndArtifactUris;
    }

    private static IList<FavoriteProvider> ConvertContributionToFavoriteProvider(
      IEnumerable<Contribution> contributions)
    {
      return (IList<FavoriteProvider>) contributions.Select<Contribution, FavoriteProvider>((Func<Contribution, FavoriteProvider>) (contribution => new FavoriteProvider()
      {
        ContributionId = contribution.Id,
        ServiceIdentifier = FavoriteProviderService.GetPropertyValue<Guid>(contribution, ContributionConstants.ServiceInstanceTypeProperty),
        ArtifactType = FavoriteProviderService.GetPropertyValue<string>(contribution, ContributionConstants.ArtifactTypeProperty),
        PluralName = FavoriteProviderService.GetPropertyValue<string>(contribution, ContributionConstants.PluralNameProperty),
        Order = FavoriteProviderService.GetPropertyValue<int>(contribution, ContributionConstants.OrderProperty, 99),
        IconClass = FavoriteProviderService.GetPropertyValue<string>(contribution, ContributionConstants.IconClassProperty),
        IconName = FavoriteProviderService.GetPropertyValue<string>(contribution, ContributionConstants.IconNameProperty),
        ClientServiceIdentifier = FavoriteProviderService.GetPropertyValue<string>(contribution, ContributionConstants.ClientServiceIdentifierProperty)
      })).ToList<FavoriteProvider>();
    }

    private static List<Contribution> GetFavoriteProviderContributions(
      IVssRequestContext requestContext)
    {
      return requestContext.GetService<IContributionService>().QueryContributions(requestContext, (IEnumerable<string>) FavoriteProviderService.ContributionsToFilter, FavoriteProviderService.ContributionTypesToFilter, ContributionQueryOptions.IncludeChildren).ToList<Contribution>();
    }

    private Dictionary<Guid, FavoriteProviderService.ServiceInstanceInfo> GetServiceAndArtifactUriForProvisionedServices(
      IVssRequestContext requestContext,
      ICollection<Guid> serviceIdentifiers)
    {
      Dictionary<Guid, FavoriteProviderService.ServiceInstanceInfo> provisionedServices = new Dictionary<Guid, FavoriteProviderService.ServiceInstanceInfo>();
      ILocationService service = requestContext.GetService<ILocationService>();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      foreach (HostInstanceMapping hostInstanceMapping in (IEnumerable<HostInstanceMapping>) vssRequestContext.GetService<IInstanceManagementService>().GetHostInstanceMappings(vssRequestContext, service.HostId))
      {
        if (serviceIdentifiers.Contains(hostInstanceMapping.ServiceInstance.InstanceType))
        {
          FavoriteProviderService.ServiceInstanceInfo serviceInstanceInfo = new FavoriteProviderService.ServiceInstanceInfo()
          {
            ServiceUri = this.GetServiceUri(requestContext, hostInstanceMapping.ServiceInstance.InstanceType),
            ArtifactUri = this.GetResourceUri(requestContext, hostInstanceMapping.ServiceInstance.InstanceType)
          };
          provisionedServices.Add(hostInstanceMapping.ServiceInstance.InstanceType, serviceInstanceInfo);
        }
      }
      return provisionedServices;
    }

    private string GetResourceUri(IVssRequestContext requestContext, Guid serviceIdentifier)
    {
      Func<string> run = (Func<string>) (() =>
      {
        try
        {
          ILocationService service = requestContext.GetService<ILocationService>();
          using (requestContext.CreateAsyncTimeOutScope(this.GetLocationServiceTimeout(requestContext)))
            return service.GetLocationData(requestContext, serviceIdentifier).GetResourceUri(requestContext, "Favorite", FavoriteRestConstants.FavoriteResource.LocationId, (object) "{area}/{resource}/{favoriteId}").ToString();
        }
        catch (OperationCanceledException ex)
        {
          requestContext.TraceException(0, nameof (FavoriteProviderService), nameof (GetResourceUri), (Exception) ex);
          return (string) null;
        }
      });
      Func<string> fallback = (Func<string>) (() => (string) null);
      string str = FavoriteProviderService.SanitizeKeyName(string.Format("FavoriteProviders/ResourceUri/{0}", (object) serviceIdentifier));
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Favorite.").AndCommandKey((CommandKey) str).AndCommandPropertiesDefaults(this._commandPropertiesForGetResourceUri);
      return new CommandService<string>(requestContext, setter, run, fallback).Execute();
    }

    private string GetServiceUri(IVssRequestContext requestContext, Guid serviceIdentifier)
    {
      Func<string> run = (Func<string>) (() =>
      {
        ILocationService service = requestContext.GetService<ILocationService>();
        try
        {
          using (requestContext.CreateAsyncTimeOutScope(this.GetLocationServiceTimeout(requestContext)))
          {
            string locationServiceUrl = service.GetLocationServiceUrl(requestContext, serviceIdentifier, AccessMappingConstants.ClientAccessMappingMoniker);
            return string.IsNullOrWhiteSpace(locationServiceUrl) ? string.Empty : new Uri(locationServiceUrl).ToString();
          }
        }
        catch (OperationCanceledException ex)
        {
          requestContext.TraceException(0, nameof (FavoriteProviderService), nameof (GetServiceUri), (Exception) ex);
          return (string) null;
        }
      });
      Func<string> fallback = (Func<string>) (() => (string) null);
      string str = FavoriteProviderService.SanitizeKeyName(string.Format("FavoriteProviders/ServiceUri/{0}", (object) serviceIdentifier));
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Favorite.").AndCommandKey((CommandKey) str).AndCommandPropertiesDefaults(this._commandPropertiesForGetServiceUri);
      return new CommandService<string>(requestContext, setter, run, fallback).Execute();
    }

    private static T GetPropertyValue<T>(
      Contribution contribution,
      string property,
      T fallbackValue = null)
    {
      return contribution.GetProperty<T>(property, fallbackValue);
    }

    private static string SanitizeKeyName(string input) => !string.IsNullOrWhiteSpace(input) ? input.Replace('_', '-') : input;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private class ServiceInstanceInfo
    {
      public string ServiceUri { get; set; }

      public string ArtifactUri { get; set; }
    }
  }
}
