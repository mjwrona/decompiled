// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ClientProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using Microsoft.TeamFoundation.Framework.Server.Internal;
using Microsoft.TeamFoundation.Framework.Server.OAuth2;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ClientProvider : IDisposable, IVssHttpClientProvider, ICreateClient
  {
    private bool disposed;
    private readonly Dictionary<ClientProvider.ClientCacheKey, IVssHttpClient> m_clients = new Dictionary<ClientProvider.ClientCacheKey, IVssHttpClient>(ClientProvider.ClientCacheKey.Comparer);
    private const string s_Area = "HostManagement";
    private const string s_Layer = "ClientProvider";
    private const string c_FindServiceDefinitionWithFaultInCC = "VisualStudio.Services.Framework.UseFindServiceDefinitionWithFaultIn.M230";
    private static readonly IConfigPrototype<bool> useFindServiceDefinitionWithFaultInPrototype = ConfigPrototype.Create<bool>("VisualStudio.Services.Framework.UseFindServiceDefinitionWithFaultIn.M230", false);
    private static readonly IConfigQueryable<bool> useFindServiceDefinitionWithFaultIn = ConfigProxy.Create<bool>(ClientProvider.useFindServiceDefinitionWithFaultInPrototype);
    private static Guid s_TokenServicePrincipal = new Guid("00000052-0000-8888-8000-000000000000");

    public void Dispose() => this.Dispose(true);

    private void Dispose(bool disposing)
    {
      if (this.disposed)
        return;
      if (disposing)
        this.ClearClients();
      this.disposed = true;
    }

    private void ClearClients()
    {
      foreach (KeyValuePair<ClientProvider.ClientCacheKey, IVssHttpClient> client in this.m_clients)
        client.Value.Dispose();
      this.m_clients.Clear();
    }

    public T GetClient<T>(IVssRequestContext requestContext, VssHttpClientOptions httpClientOptions = null) where T : class, IVssHttpClient
    {
      Type type = typeof (T);
      ResourceAreaAttribute[] customAttributes = (ResourceAreaAttribute[]) type.GetCustomAttributes(typeof (ResourceAreaAttribute), true);
      if (customAttributes.Length == 0)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "No ResourceAreaAttribute for requested type {0}", (object) type.Name));
      if (!requestContext.ServiceHost.IsProduction && InstanceManagementHelper.IsValidServiceInstanceType(customAttributes[0].AreaId))
        throw new ArgumentException("You may not use a Service InstanceType guid in the [ResourceArea] attribute! Please generate a unique area identifier for your resources (and register it in your IVssApiResourceProvider plugin). \r\nUse this wiki for more information: https://aka.ms/resource-area-troubleshooting");
      return this.GetClientImpl<T>(requestContext, customAttributes[0].AreaId, Guid.Empty, httpClientOptions);
    }

    public T GetClient<T>(
      IVssRequestContext requestContext,
      Guid serviceAreaId,
      Guid serviceIdentifier,
      VssHttpClientOptions httpClientOptions = null)
      where T : class, IVssHttpClient
    {
      return this.GetClientImpl<T>(requestContext, serviceAreaId, serviceIdentifier, httpClientOptions);
    }

    private T GetClientImpl<T>(
      IVssRequestContext requestContext,
      Guid serviceAreaId,
      Guid serviceIdentifier,
      VssHttpClientOptions httpClientOptions)
      where T : IVssHttpClient
    {
      Type type = typeof (T);
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        long version = vssRequestContext.GetService<IOAuth2SettingsService>().GetVersion(vssRequestContext);
        ClientProvider.ClientCacheKey key = new ClientProvider.ClientCacheKey(requestContext, type, serviceAreaId, serviceIdentifier, version, httpClientOptions);
        IVssHttpClient client;
        if (!this.m_clients.TryGetValue(key, out client))
        {
          client = this.CreateClient<T>(requestContext, type, serviceAreaId, serviceIdentifier, httpClientOptions);
          this.m_clients[key] = client;
        }
        return (T) client;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, "HostManagement", nameof (ClientProvider), ex);
        if (!(ex is DatabaseConnectionException))
          TeamFoundationEventLog.Default.LogException(requestContext, FrameworkResources.GetClientFailed((object) type.FullName), ex, TeamFoundationEventId.ApplicationInitialization, EventLogEntryType.Error);
        throw;
      }
    }

    private IVssHttpClient CreateClient<T>(
      IVssRequestContext requestContext,
      Type requestedType,
      Guid serviceAreaId,
      Guid serviceIdentifier,
      VssHttpClientOptions httpClientOptions = null)
      where T : IVssHttpClient
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        WebApiConfiguration.Initialize(requestContext);
      else
        ArgumentUtility.CheckForEmptyGuid(serviceAreaId, nameof (serviceAreaId));
      if (serviceIdentifier == Guid.Empty)
        serviceIdentifier = LocationServiceConstants.SelfReferenceIdentifier;
      bool isProduction = requestContext.ServiceHost.IsProduction;
      string uriString = (string) null;
      ApiResourceLocationCollection locationCollection = (ApiResourceLocationCollection) null;
      ILocationService service = requestContext.GetService<ILocationService>();
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) || requestContext.ExecutionEnvironment.IsOnPremisesDeployment || serviceIdentifier != LocationServiceConstants.SelfReferenceIdentifier || serviceAreaId == LocationServiceConstants.RootIdentifier)
      {
        ILocationDataProvider locationData = service.GetLocationData(requestContext, serviceAreaId, false);
        if (locationData != null)
        {
          ServiceDefinition serviceDefinition = locationData.FindServiceDefinition(requestContext, "LocationService2", serviceIdentifier);
          if (serviceDefinition != null)
            uriString = locationData.LocationForAccessMapping(requestContext, serviceDefinition, locationData.GetAccessMapping(requestContext, AccessMappingConstants.HostGuidAccessMappingMoniker) ?? locationData.GetPublicAccessMapping(requestContext));
          if (serviceIdentifier == LocationServiceConstants.SelfReferenceIdentifier)
            locationCollection = locationData.GetResourceLocations(requestContext);
        }
      }
      else
      {
        ILocationDataProvider locationData = service.GetLocationData(requestContext, LocationServiceConstants.RootIdentifier);
        ServiceDefinition serviceDefinition1 = !ClientProvider.useFindServiceDefinitionWithFaultIn.QueryByCtx<bool>(requestContext) ? locationData.FindServiceDefinition(requestContext, "LocationService2", serviceAreaId) : locationData.FindServiceDefinitionWithFaultIn(requestContext, "LocationService2", serviceAreaId, false);
        if (serviceDefinition1 != null)
        {
          uriString = locationData.LocationForAccessMapping(requestContext, serviceDefinition1, locationData.GetAccessMapping(requestContext, AccessMappingConstants.HostGuidAccessMappingMoniker));
          if (serviceDefinition1.ParentIdentifier == Guid.Empty)
          {
            locationCollection = locationData.GetResourceLocations(requestContext);
          }
          else
          {
            ServiceDefinition serviceDefinition2;
            for (; !InstanceManagementHelper.IsValidServiceInstanceType(serviceDefinition1.Identifier); serviceDefinition1 = serviceDefinition2)
            {
              serviceDefinition2 = !ClientProvider.useFindServiceDefinitionWithFaultIn.QueryByCtx<bool>(requestContext) || !(serviceDefinition1.ParentIdentifier == ClientProvider.s_TokenServicePrincipal) ? locationData.FindServiceDefinition(requestContext, serviceDefinition1.ParentServiceType, serviceDefinition1.ParentIdentifier) : locationData.FindServiceDefinitionWithFaultIn(requestContext, serviceDefinition1.ParentServiceType, serviceDefinition1.ParentIdentifier, false);
              if (serviceDefinition2 == null)
              {
                this.TraceError(requestContext, string.Format("Why could we not retrieve a parent definition for definition - {0}:{1}, parent-{2}:{3}", (object) serviceDefinition1.ServiceType, (object) serviceDefinition1.Identifier, (object) serviceDefinition1.ParentServiceType, (object) serviceDefinition1.ParentIdentifier), 239059932, (isProduction ? 1 : 0) != 0);
                goto label_22;
              }
            }
            if (serviceDefinition1.ParentIdentifier != Guid.Empty)
            {
              IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
              List<ApiResourceLocation> list = vssRequestContext.GetService<IInheritedLocationDataService>().GetData(vssRequestContext, serviceDefinition1.ParentIdentifier, TeamFoundationHostTypeHelper.NormalizeHostType(requestContext.ServiceHost.HostType)).GetAllServiceDefinitions().Where<ServiceDefinition>((Func<ServiceDefinition, bool>) (def => def.IsResourceLocation())).Select<ServiceDefinition, ApiResourceLocation>((Func<ServiceDefinition, ApiResourceLocation>) (def => ApiResourceLocation.FromServiceDefinition(def))).ToList<ApiResourceLocation>();
              if (list.Count > 0)
              {
                locationCollection = new ApiResourceLocationCollection();
                locationCollection.AddResourceLocations((IEnumerable<ApiResourceLocation>) list);
              }
            }
            else
              this.TraceError(requestContext, string.Format("Definition {0}:{1} has an empty parent for service area {2}", (object) serviceDefinition1.ServiceType, (object) serviceDefinition1.Identifier, (object) serviceAreaId), 607362150, isProduction);
label_22:
            if (locationCollection == null)
              this.TraceError(requestContext, string.Format("Why could we not retrieve Resource Locations for {0}", (object) serviceAreaId), 1623981867, isProduction);
          }
        }
      }
      if (uriString == null)
      {
        if (!isProduction)
          throw new ServiceOwnerNotFoundException(string.Format("The resource area '{0}' could not be resolved for service host '{1}'.\r\n                                        Either the resource area has not been properly registered or an instance allocation can not be made.\r\n                                        Use this wiki for troubleshooting tips: https://aka.ms/resource-area-troubleshooting", (object) serviceAreaId, (object) requestContext.ServiceHost.InstanceId));
        throw new ServiceOwnerNotFoundException(serviceAreaId.ToString("D"), requestContext.ServiceHost.InstanceId);
      }
      try
      {
        Uri uri = new Uri(uriString);
        return this.CreateClient(new ClientProvider.CreateClientParams()
        {
          RequestContext = requestContext,
          RequestedType = requestedType,
          BaseUri = uri,
          LogAs = requestedType.Name,
          ResourceLocations = locationCollection,
          RequiresResourceLocations = true,
          TargetServicePrincipal = Guid.Empty,
          CustomDelegatingHandlers = ClientProviderHelper.GetCustomHandlersFromType(requestContext, requestedType, "HostManagement", nameof (ClientProvider)),
          HttpClientOptions = httpClientOptions
        });
      }
      catch (MissingMethodException ex)
      {
        TeamFoundationTrace.TraceException((Exception) ex);
        throw new ArgumentException(FrameworkResources.GetClientArgumentError((object) requestedType), (Exception) ex);
      }
    }

    private void TraceError(
      IVssRequestContext requestContext,
      string message,
      int tracePoint,
      bool isProduction)
    {
      if (!isProduction)
        throw new InvalidOperationException(message);
      requestContext.Trace(tracePoint, TraceLevel.Error, "HostManagement", nameof (ClientProvider), message);
    }

    T ICreateClient.CreateClient<T>(
      IVssRequestContext requestContext,
      Uri baseUri,
      string logAs,
      ApiResourceLocationCollection resourceLocations,
      bool requiresResourceLocations,
      Guid targetServicePrincipal,
      VssHttpClientOptions httpClientOptions)
    {
      Type type = typeof (T);
      if (httpClientOptions == null)
        httpClientOptions = requestContext.GetHttpClientOptions();
      return (T) this.CreateClient(new ClientProvider.CreateClientParams()
      {
        RequestContext = requestContext,
        RequestedType = type,
        BaseUri = baseUri,
        LogAs = logAs,
        ResourceLocations = resourceLocations,
        RequiresResourceLocations = requiresResourceLocations,
        TargetServicePrincipal = targetServicePrincipal,
        CustomDelegatingHandlers = ClientProviderHelper.GetCustomHandlersFromType(requestContext, type, "HostManagement", nameof (ClientProvider)),
        HttpClientOptions = httpClientOptions
      });
    }

    private IVssHttpClient CreateClient(ClientProvider.CreateClientParams parameters)
    {
      IVssRequestContext vssRequestContext = parameters.RequestContext.To(TeamFoundationHostType.Deployment).Elevate();
      vssRequestContext.GetService<IVssRegistryService>();
      if (parameters.RequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        if (vssRequestContext.ExecutionEnvironment.IsSslOnly && parameters.BaseUri.Scheme != Uri.UriSchemeHttps)
          throw new TeamFoundationServiceException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The Service URL does not meet the SSL restriction. URL: {0}", (object) parameters.BaseUri.AbsoluteUri));
        if (!vssRequestContext.GetService<IInstanceManagementService>().IsRegisteredServiceDomain(vssRequestContext, parameters.BaseUri.Host))
          throw new TeamFoundationServiceException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The Service URL had an untrusted host domain. URL: {0}", (object) parameters.BaseUri.AbsoluteUri));
      }
      HttpMessageHandler handler = parameters.RequestContext.GetService<IVssHttpMessageHandlerService>().GetHandler(vssRequestContext, parameters.BaseUri, parameters.TargetServicePrincipal);
      List<DelegatingHandler> delegatingHandlers = ClientProviderHelper.GetDelegatingHandlers(parameters.RequestContext, parameters.RequestedType, parameters.BaseUri, ClientProviderHelper.Options.CreateDefault(parameters.RequestContext), parameters.LogAs, parameters.CustomDelegatingHandlers, parameters.HttpClientOptions);
      IVssHttpClient instance = (IVssHttpClient) Activator.CreateInstance(parameters.RequestedType, (object) parameters.BaseUri, (object) HttpClientFactory.CreatePipeline(handler, (IEnumerable<DelegatingHandler>) delegatingHandlers), (object) false);
      if (parameters.ResourceLocations != null)
        instance.SetResourceLocations(parameters.ResourceLocations);
      else if (parameters.RequiresResourceLocations && instance is VssHttpClientBase vssHttpClientBase)
        vssHttpClientBase.EnsureResourceLocationsPopulated(cancellationToken: parameters.RequestContext.CancellationToken).SyncResult();
      return instance;
    }

    private struct ClientCacheKey
    {
      public readonly IVssRequestContext RequestContext;
      public readonly Type Type;
      public readonly Guid ServiceIdentifier;
      public readonly Guid ServiceAreaId;
      public readonly long CacheVersion;
      public readonly VssHttpClientOptions HttpClientOptions;
      public static readonly IEqualityComparer<ClientProvider.ClientCacheKey> Comparer = (IEqualityComparer<ClientProvider.ClientCacheKey>) new ClientProvider.ClientCacheKey.ClientCacheKeyComparer();

      public ClientCacheKey(
        IVssRequestContext requestContext,
        Type type,
        Guid serviceAreaId,
        Guid serviceIdentifier,
        long cacheVersion,
        VssHttpClientOptions httpClientOptions)
      {
        this.RequestContext = requestContext;
        this.Type = type;
        this.ServiceIdentifier = serviceIdentifier;
        this.ServiceAreaId = serviceAreaId;
        this.CacheVersion = cacheVersion;
        this.HttpClientOptions = httpClientOptions;
      }

      private sealed class ClientCacheKeyComparer : IEqualityComparer<ClientProvider.ClientCacheKey>
      {
        public bool Equals(ClientProvider.ClientCacheKey x, ClientProvider.ClientCacheKey y)
        {
          if (x.RequestContext != y.RequestContext || !x.ServiceIdentifier.Equals(y.ServiceIdentifier) || !x.ServiceAreaId.Equals(y.ServiceAreaId) || !x.Type.Equals(y.Type) || !x.CacheVersion.Equals(y.CacheVersion))
            return false;
          if (x.HttpClientOptions == y.HttpClientOptions)
            return true;
          VssHttpClientOptions httpClientOptions = x.HttpClientOptions;
          // ISSUE: explicit non-virtual call
          return httpClientOptions != null && __nonvirtual (httpClientOptions.Equals(y.HttpClientOptions));
        }

        public int GetHashCode(ClientProvider.ClientCacheKey obj)
        {
          int num = obj.RequestContext.GetHashCode() ^ obj.Type.GetHashCode() ^ obj.ServiceAreaId.GetHashCode() ^ obj.ServiceIdentifier.GetHashCode() ^ obj.CacheVersion.GetHashCode();
          VssHttpClientOptions httpClientOptions = obj.HttpClientOptions;
          int hashCode = httpClientOptions != null ? httpClientOptions.GetHashCode() : 0;
          return num ^ hashCode;
        }
      }
    }

    private sealed class CreateClientParams
    {
      public IVssRequestContext RequestContext { get; set; }

      public Type RequestedType { get; set; }

      public Uri BaseUri { get; set; }

      public string LogAs { get; set; }

      public ApiResourceLocationCollection ResourceLocations { get; set; }

      public bool RequiresResourceLocations { get; set; }

      public Guid TargetServicePrincipal { get; set; }

      public VssHttpClientOptions HttpClientOptions { get; set; }

      public IEnumerable<DelegatingHandler> CustomDelegatingHandlers { get; set; }
    }
  }
}
