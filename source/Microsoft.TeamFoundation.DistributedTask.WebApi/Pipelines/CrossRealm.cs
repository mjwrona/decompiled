// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.CrossRealm
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Client;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  public sealed class CrossRealm
  {
    private readonly Func<Guid, Uri> m_getOAuth2TokenUri;
    private readonly Uri m_mpsUri;
    private readonly Uri m_mmsUri;
    private readonly Func<CrossRealm.OAuth2Settings> m_getOAuth2Settings;
    private readonly CrossRealm.CacheValue<VssOAuthAccessToken> m_initialToken;
    private readonly CrossRealm.CacheValue<IReadOnlyDictionary<Guid, ServiceDefinition>> m_LocationService2DefsByAreaId;
    private readonly ConcurrentDictionary<Guid, CrossRealm.CacheValue<ApiResourceLocationCollection>> m_resourceLocationsByAreaId;
    private readonly ConcurrentDictionary<Guid, CrossRealm.CacheValue<HttpMessageHandler>> m_messageHandlersByInstanceType;
    private static readonly Guid m_MMSInstanceTypeGuid = new Guid("00000040-0000-8888-8000-000000000000");

    public CrossRealm(
      Func<Guid, Uri> getOAuth2TokenUri,
      Uri mpsUri,
      Uri mmsUri,
      Func<CrossRealm.OAuth2Settings> getOAuth2Settings)
    {
      this.m_getOAuth2TokenUri = getOAuth2TokenUri;
      this.m_mpsUri = mpsUri;
      this.m_mmsUri = mmsUri;
      this.m_getOAuth2Settings = getOAuth2Settings;
      this.m_initialToken = new CrossRealm.CacheValue<VssOAuthAccessToken>();
      this.m_LocationService2DefsByAreaId = new CrossRealm.CacheValue<IReadOnlyDictionary<Guid, ServiceDefinition>>();
      this.m_resourceLocationsByAreaId = new ConcurrentDictionary<Guid, CrossRealm.CacheValue<ApiResourceLocationCollection>>();
      this.m_messageHandlersByInstanceType = new ConcurrentDictionary<Guid, CrossRealm.CacheValue<HttpMessageHandler>>();
    }

    public async Task<TClient> GetClientAsync<TClient>(
      CrossRealm.RequestContextAdapter requestContext,
      Guid serviceAreaId,
      CancellationToken cancellationToken = default (CancellationToken))
      where TClient : class, IVssHttpClient
    {
      IVssHttpClient clientAsync;
      if (requestContext.ClientCache.TryGetValue((serviceAreaId, typeof (TClient)), out clientAsync))
        return (TClient) clientAsync;
      TClient client1 = await this.CreateClientAsync<TClient>(requestContext, serviceAreaId);
      Func<Task<LocationHttpClient>> getLocationHttpClient = (Func<Task<LocationHttpClient>>) (async () => await this.CreateClientAsync<LocationHttpClient>(requestContext, serviceAreaId));
      this.m_resourceLocationsByAreaId.GetOrAdd(serviceAreaId, (Func<Guid, CrossRealm.CacheValue<ApiResourceLocationCollection>>) (key => new CrossRealm.CacheValue<ApiResourceLocationCollection>()));
      TClient client2 = client1;
      client2.SetResourceLocations(await this.EnsureResourceLocations(requestContext, serviceAreaId, getLocationHttpClient, cancellationToken));
      client2 = default (TClient);
      requestContext.ClientCache.Add((serviceAreaId, typeof (TClient)), (IVssHttpClient) client1);
      return client1;
    }

    public async Task<Uri> GetResourceUri(
      CrossRealm.RequestContextAdapter requestContext,
      Guid serviceAreaId,
      Guid resourceId,
      object routeValues,
      bool appendUnusedAsQueryParams,
      bool requireExplicitRouteParams,
      bool wildcardAsQueryParams,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      Func<Task<LocationHttpClient>> getLocationHttpClient = (Func<Task<LocationHttpClient>>) (async () =>
      {
        Type type = typeof (LocationHttpClient);
        object mmsUri = (object) this.m_mmsUri;
        HttpMessageHandler messageHandlerAsync = await this.GetMessageHandlerAsync(requestContext, CrossRealm.m_MMSInstanceTypeGuid);
        List<DelegatingHandler> delegatingHandlersAsync = await requestContext.CreateDelegatingHandlersAsync<LocationHttpClient>();
        return (LocationHttpClient) Activator.CreateInstance(type, mmsUri, (object) HttpClientFactory.CreatePipeline(messageHandlerAsync, (IEnumerable<DelegatingHandler>) delegatingHandlersAsync), (object) false);
      });
      ApiResourceLocation locationById = (await this.EnsureResourceLocations(requestContext, serviceAreaId, getLocationHttpClient, cancellationToken)).TryGetLocationById(resourceId);
      if (locationById == null)
        throw new VssResourceNotFoundException(resourceId);
      Dictionary<string, object> routeDictionary = VssHttpUriUtility.ToRouteDictionary(routeValues, locationById.Area, locationById.ResourceName);
      return VssHttpUriUtility.ConcatUri(this.m_mmsUri, VssHttpUriUtility.ReplaceRouteValues(locationById.RouteTemplate, routeDictionary, (RouteReplacementOptions) ((appendUnusedAsQueryParams ? 2 : 0) | (requireExplicitRouteParams ? 4 : 0) | (wildcardAsQueryParams ? 8 : 0))));
    }

    public async Task<VssOAuthCredential> CreateOAuthCreds(
      CrossRealm.RequestContextAdapter requestContext,
      Guid instanceType,
      CrossRealm.OAuth2Settings oAuth2Settings = null)
    {
      oAuth2Settings = oAuth2Settings ?? this.m_getOAuth2Settings();
      VssSigningCredentials signingCredentials = VssSigningCredentials.Create(oAuth2Settings.Cert);
      Uri oAuth2TokenUri = this.m_getOAuth2TokenUri(instanceType);
      VssOAuthJwtBearerClientCredential clientCredential = new VssOAuthJwtBearerClientCredential(oAuth2Settings.ClientId, oAuth2TokenUri.AbsoluteUri, signingCredentials);
      VssOAuthAccessToken accessToken = (VssOAuthAccessToken) null;
      if (requestContext.UseCachedAccessToken)
        accessToken = await this.m_initialToken.GetOrSetAsync((Func<Task<(VssOAuthAccessToken, Func<bool>)>>) (async () =>
        {
          try
          {
            VssOAuthCredential credential = new VssOAuthCredential(oAuth2TokenUri, (VssOAuthGrant) VssOAuthGrant.ClientCredentials, (VssOAuthClientCredential) clientCredential);
            VssOAuthAccessToken issuedToken = await new VssOAuthTokenProvider(credential, credential.AuthorizationUrl).GetTokenAsync((IssuedToken) null, new CancellationToken()) as VssOAuthAccessToken;
            return (issuedToken, (Func<bool>) (() => issuedToken.ValidTo > DateTime.UtcNow + TimeSpan.FromMinutes(5.0)));
          }
          catch (Exception ex)
          {
            return ((VssOAuthAccessToken) null, (Func<bool>) (() => false));
          }
        }));
      return new VssOAuthCredential(oAuth2TokenUri, (VssOAuthGrant) VssOAuthGrant.ClientCredentials, (VssOAuthClientCredential) clientCredential, accessToken: accessToken);
    }

    private async Task<ApiResourceLocationCollection> EnsureResourceLocations(
      CrossRealm.RequestContextAdapter requestContext,
      Guid serviceAreaId,
      Func<Task<LocationHttpClient>> getLocationHttpClient,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.m_resourceLocationsByAreaId.GetOrAdd(serviceAreaId, (Func<Guid, CrossRealm.CacheValue<ApiResourceLocationCollection>>) (key => new CrossRealm.CacheValue<ApiResourceLocationCollection>())).GetOrSetAsync((Func<Task<(ApiResourceLocationCollection, Func<bool>)>>) (async () =>
      {
        ConnectionData connectionDataAsync = await (await getLocationHttpClient()).GetConnectionDataAsync(ConnectOptions.IncludeServices, 0L, cancellationToken);
        ApiResourceLocationCollection locationCollection = new ApiResourceLocationCollection();
        locationCollection.AddResourceLocations(connectionDataAsync.LocationServiceData.ServiceDefinitions.Select<ServiceDefinition, ApiResourceLocation>((Func<ServiceDefinition, ApiResourceLocation>) (x => ApiResourceLocation.FromServiceDefinition(x))));
        DateTime expiration = DateTime.UtcNow.AddSeconds((double) connectionDataAsync.LocationServiceData.ClientCacheTimeToLive);
        return (locationCollection, (Func<bool>) (() => DateTime.UtcNow < expiration));
      }));
    }

    public Task<IReadOnlyDictionary<Guid, ServiceDefinition>> GetCachedLocationService2DefinitionsByAreaIdAsync(
      CrossRealm.RequestContextAdapter requestContext)
    {
      return this.m_LocationService2DefsByAreaId.GetOrSetAsync((Func<Task<(IReadOnlyDictionary<Guid, ServiceDefinition>, Func<bool>)>>) (async () =>
      {
        LocationHttpClient clientAsync = await this.CreateClientAsync<LocationHttpClient>(requestContext, ServiceInstanceTypes.MPS);
        DateTime expiration = DateTime.UtcNow.AddHours(1.0);
        return ((IReadOnlyDictionary<Guid, ServiceDefinition>) (await clientAsync.GetServiceDefinitionsAsync("LocationService2")).ToDictionary<ServiceDefinition, Guid>((Func<ServiceDefinition, Guid>) (x => x.Identifier)), (Func<bool>) (() => DateTime.UtcNow < expiration));
      }));
    }

    private async Task<TClient> CreateClientAsync<TClient>(
      CrossRealm.RequestContextAdapter requestContext,
      Guid serviceAreaId)
      where TClient : IVssHttpClient
    {
      Uri uri;
      Guid instanceType;
      if (typeof (TClient) == typeof (LocationHttpClient) && serviceAreaId == ServiceInstanceTypes.MPS)
      {
        uri = this.m_mpsUri;
        instanceType = ServiceInstanceTypes.MPS;
      }
      else
      {
        ServiceDefinition serviceDefinition;
        if (!(await this.GetCachedLocationService2DefinitionsByAreaIdAsync(requestContext)).TryGetValue(serviceAreaId, out serviceDefinition))
          throw new ServiceDefinitionDoesNotExistException(string.Format("The service definition with service type '{0}' and identifier '{1}' does not exist.", (object) "LocationService2", (object) serviceAreaId));
        uri = new Uri(serviceDefinition.GetLocationMapping(AccessMappingConstants.PublicAccessMappingMoniker).Location);
        instanceType = serviceDefinition.ParentIdentifier;
      }
      Type type = typeof (TClient);
      object obj = (object) uri;
      HttpMessageHandler messageHandlerAsync = await this.GetMessageHandlerAsync(requestContext, instanceType);
      List<DelegatingHandler> delegatingHandlersAsync = await requestContext.CreateDelegatingHandlersAsync<TClient>();
      return (TClient) Activator.CreateInstance(type, obj, (object) HttpClientFactory.CreatePipeline(messageHandlerAsync, (IEnumerable<DelegatingHandler>) delegatingHandlersAsync), (object) false);
    }

    public async Task<HttpMessageHandler> GetMessageHandlerAsync(
      CrossRealm.RequestContextAdapter requestContext,
      Guid instanceType)
    {
      return await this.m_messageHandlersByInstanceType.GetOrAdd(instanceType, (Func<Guid, CrossRealm.CacheValue<HttpMessageHandler>>) (key => new CrossRealm.CacheValue<HttpMessageHandler>())).GetOrSetAsync((Func<Task<(HttpMessageHandler, Func<bool>)>>) (async () =>
      {
        CrossRealm.OAuth2Settings oAuth2Settings = this.m_getOAuth2Settings();
        return ((HttpMessageHandler) new VssHttpMessageHandler((VssCredentials) (FederatedCredential) await this.CreateOAuthCreds(requestContext, instanceType, oAuth2Settings), new VssHttpRequestSettings()), (Func<bool>) (() => this.m_getOAuth2Settings() == oAuth2Settings));
      }));
    }

    private class CacheValue<T> where T : class
    {
      private readonly CrossRealm.AsyncLock m_lock = new CrossRealm.AsyncLock();
      private T m_value;
      private Func<bool> m_isValid;

      public async Task<T> GetOrSetAsync(Func<Task<(T value, Func<bool> isValid)>> onSet)
      {
        T orSetAsync;
        using (await this.m_lock.LockAsync())
        {
          if ((object) this.m_value == null || !this.m_isValid())
            (this.m_value, this.m_isValid) = await onSet();
          orSetAsync = this.m_value;
        }
        return orSetAsync;
      }
    }

    public abstract class RequestContextAdapter : IDisposable
    {
      protected RequestContextAdapter() => this.ClientCache = new Dictionary<(Guid, Type), IVssHttpClient>();

      public void Dispose()
      {
        foreach (KeyValuePair<(Guid serviceAreaId, Type type), IVssHttpClient> keyValuePair in this.ClientCache)
          keyValuePair.Value.Dispose();
      }

      public bool UseCachedAccessToken { get; set; }

      public Dictionary<(Guid serviceAreaId, Type type), IVssHttpClient> ClientCache { get; }

      public abstract Task<List<DelegatingHandler>> CreateDelegatingHandlersAsync<TClient>() where TClient : IVssHttpClient;
    }

    public class OAuth2Settings
    {
      public OAuth2Settings(string clientId, X509Certificate2 cert)
      {
        this.ClientId = clientId;
        this.Cert = cert;
      }

      public override string ToString() => "(ClientId: " + this.ClientId + ", Thumbprint: " + this.Cert.Thumbprint + ")";

      public string ClientId { get; }

      public X509Certificate2 Cert { get; }
    }

    private sealed class AsyncLock
    {
      private readonly SemaphoreSlim m_semaphore = new SemaphoreSlim(1, 1);
      private readonly CrossRealm.AsyncLock.Releaser m_releaser;
      private readonly Task<IDisposable> m_releaserTask;

      public AsyncLock()
      {
        this.m_releaser = new CrossRealm.AsyncLock.Releaser(this);
        this.m_releaserTask = Task.FromResult<IDisposable>((IDisposable) this.m_releaser);
      }

      public Task<IDisposable> LockAsync(CancellationToken cancellationToken = default (CancellationToken))
      {
        Task task1 = this.m_semaphore.WaitAsync();
        return task1.IsCompleted ? this.m_releaserTask : task1.ContinueWith<IDisposable>((Func<Task, object, IDisposable>) ((task, state) => (IDisposable) state), (object) this.m_releaser, cancellationToken, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
      }

      private sealed class Releaser : IDisposable
      {
        private readonly CrossRealm.AsyncLock m_toRelease;

        internal Releaser(CrossRealm.AsyncLock toRelease) => this.m_toRelease = toRelease;

        public void Dispose() => this.m_toRelease.m_semaphore.Release();
      }
    }
  }
}
