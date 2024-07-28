// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.CrossRealm
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

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

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  public sealed class CrossRealm
  {
    private readonly Func<Guid, Uri> m_getOAuth2TokenUri;
    private readonly Uri m_mpsUri;
    private readonly Func<CrossRealm.OAuth2Settings> m_getOAuth2Settings;
    private readonly CrossRealm.CacheValue<IReadOnlyDictionary<Guid, ServiceDefinition>> m_LocationService2DefsByAreaId;
    private readonly ConcurrentDictionary<Guid, CrossRealm.CacheValue<ApiResourceLocationCollection>> m_resourceLocationsByAreaId;
    private readonly ConcurrentDictionary<Guid, CrossRealm.CacheValue<HttpMessageHandler>> m_messageHandlersByInstanceType;

    public CrossRealm(
      Func<Guid, Uri> getOAuth2TokenUri,
      Uri mpsUri,
      Func<CrossRealm.OAuth2Settings> getOAuth2Settings)
    {
      this.m_getOAuth2TokenUri = getOAuth2TokenUri;
      this.m_mpsUri = mpsUri;
      this.m_getOAuth2Settings = getOAuth2Settings;
      this.m_LocationService2DefsByAreaId = new CrossRealm.CacheValue<IReadOnlyDictionary<Guid, ServiceDefinition>>();
      this.m_resourceLocationsByAreaId = new ConcurrentDictionary<Guid, CrossRealm.CacheValue<ApiResourceLocationCollection>>();
      this.m_messageHandlersByInstanceType = new ConcurrentDictionary<Guid, CrossRealm.CacheValue<HttpMessageHandler>>();
    }

    public async Task<TClient> GetClientAsync<TClient>(
      CrossRealm.RequestContextAdapter requestContext,
      Guid serviceAreaId,
      CancellationToken cancellationToken = default (CancellationToken))
      where TClient : VssHttpClientBase
    {
      VssHttpClientBase clientAsync;
      if (requestContext.ClientCache.TryGetValue((serviceAreaId, typeof (TClient)), out clientAsync))
        return (TClient) clientAsync;
      TClient client1 = await this.CreateClientAsync<TClient>(requestContext, serviceAreaId);
      CrossRealm.CacheValue<ApiResourceLocationCollection> orAdd = this.m_resourceLocationsByAreaId.GetOrAdd(serviceAreaId, (Func<Guid, CrossRealm.CacheValue<ApiResourceLocationCollection>>) (key => new CrossRealm.CacheValue<ApiResourceLocationCollection>()));
      TClient client2 = client1;
      Func<Task<(ApiResourceLocationCollection, Func<bool>)>> onSet = (Func<Task<(ApiResourceLocationCollection, Func<bool>)>>) (async () =>
      {
        ConnectionData connectionDataAsync = await (await this.CreateClientAsync<LocationHttpClient>(requestContext, serviceAreaId)).GetConnectionDataAsync(ConnectOptions.IncludeServices, 0L, cancellationToken);
        ApiResourceLocationCollection locationCollection = new ApiResourceLocationCollection();
        locationCollection.AddResourceLocations(connectionDataAsync.LocationServiceData.ServiceDefinitions.Select<ServiceDefinition, ApiResourceLocation>((Func<ServiceDefinition, ApiResourceLocation>) (x => ApiResourceLocation.FromServiceDefinition(x))));
        DateTime expiration = DateTime.UtcNow.AddSeconds((double) connectionDataAsync.LocationServiceData.ClientCacheTimeToLive);
        return (locationCollection, (Func<bool>) (() => DateTime.UtcNow < expiration));
      });
      client2.SetResourceLocations(await orAdd.GetOrSetAsync(onSet));
      client2 = default (TClient);
      requestContext.ClientCache.Add((serviceAreaId, typeof (TClient)), (VssHttpClientBase) client1);
      return client1;
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
      where TClient : VssHttpClientBase
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
          throw new ServiceDefinitionDoesNotExistException(MachineManagementResources.ServiceDefinitionDoesNotExist((object) "LocationService2", (object) serviceAreaId));
        uri = new Uri(serviceDefinition.GetLocationMapping(AccessMappingConstants.PublicAccessMappingMoniker).Location);
        instanceType = serviceDefinition.ParentIdentifier;
      }
      Type type = typeof (TClient);
      object obj = (object) uri;
      HttpMessageHandler messageHandlerAsync = await this.GetMessageHandlerAsync(instanceType);
      List<DelegatingHandler> delegatingHandlersAsync = await requestContext.CreateDelegatingHandlersAsync<TClient>();
      return (TClient) Activator.CreateInstance(type, obj, (object) HttpClientFactory.CreatePipeline(messageHandlerAsync, (IEnumerable<DelegatingHandler>) delegatingHandlersAsync), (object) false);
    }

    private async Task<HttpMessageHandler> GetMessageHandlerAsync(Guid instanceType) => await this.m_messageHandlersByInstanceType.GetOrAdd(instanceType, (Func<Guid, CrossRealm.CacheValue<HttpMessageHandler>>) (key => new CrossRealm.CacheValue<HttpMessageHandler>())).GetOrSetAsync((Func<Task<(HttpMessageHandler, Func<bool>)>>) (() =>
    {
      CrossRealm.OAuth2Settings oAuth2Settings = this.m_getOAuth2Settings();
      VssSigningCredentials signingCredentials = VssSigningCredentials.Create(oAuth2Settings.Cert);
      Uri authorizationUrl = this.m_getOAuth2TokenUri(instanceType);
      VssOAuthJwtBearerClientCredential clientCredential = new VssOAuthJwtBearerClientCredential(oAuth2Settings.ClientId, authorizationUrl.AbsoluteUri, signingCredentials);
      return Task.FromResult<(HttpMessageHandler, Func<bool>)>(((HttpMessageHandler) new VssHttpMessageHandler((VssCredentials) (FederatedCredential) new VssOAuthCredential(authorizationUrl, (VssOAuthGrant) VssOAuthGrant.ClientCredentials, (VssOAuthClientCredential) clientCredential), new VssHttpRequestSettings()), (Func<bool>) (() => this.m_getOAuth2Settings() == oAuth2Settings)));
    }));

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
      protected RequestContextAdapter() => this.ClientCache = new Dictionary<(Guid, Type), VssHttpClientBase>();

      public void Dispose()
      {
        foreach (KeyValuePair<(Guid serviceAreaId, Type type), VssHttpClientBase> keyValuePair in this.ClientCache)
          keyValuePair.Value.Dispose();
      }

      public Dictionary<(Guid serviceAreaId, Type type), VssHttpClientBase> ClientCache { get; }

      public abstract Task<List<DelegatingHandler>> CreateDelegatingHandlersAsync<TClient>() where TClient : VssHttpClientBase;
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
